using System.Net;
using Catalog.API.Repositories.Interfaces;
using CatalogAPI.Data;
using CatalogAPI.Services;
using Common.Models;
using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ICatalogContext _catalogContext;
        private readonly ICatalogAI _catalogAI;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ICatalogContext catalogContext, ICatalogAI catalogAI, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
            _catalogAI = catalogAI ?? throw new ArgumentNullException(nameof(catalogAI));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(Guid id)
        {
            var product = await _repository.GetProduct(id);

            if (product == null)
            {
                _logger.LogError($"Product with id: {id}, not found.");
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// Basic filtering and exact matches
        /// </summary>
        /// <param name="searchTerm">Term to search</param>
        /// <param name="page">Page numebr</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        [HttpGet("search/{searchTerm}", Name = "search")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<List<Product>> SearchProducts(string searchTerm, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Handle case with no search term (e.g., return all products or recent products)
                return await _catalogContext.Products.Find(_ => true).Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync();
            }

            var searchStage = BsonDocument.Parse($@"
            {{
                $search: {{
                    index: ""default"",
                    text: {{
                        query: ""{searchTerm}"",
                        path: [""Name"", ""Summary""],
                        fuzzy: {{
                            maxEdits: 1
                        }}
                    }}
                }}
            }}");

            var results = await _catalogContext.Products.Aggregate()
                                                .AppendStage<Product>(searchStage)
                                                .Skip((page - 1) * pageSize)
                                                .Limit(pageSize)
                                                .ToListAsync();
            return results;
        }

        /// <summary>
        /// Authocomplete
        /// </summary>
        /// <param name="query">Query to search</param>
        /// <returns></returns>
        [HttpGet("autocomplete/{query}", Name = "autocomplete")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<List<Product>> AutocompleteProducts(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<Product>();
            }

            var autocompleteStage = BsonDocument.Parse($@"
            {{
                $search: {{
                    index: ""autocomplete"",
                    autocomplete: {{
                        query: ""{query}"",
                        path:""Name""
                    }}
                }}
            }}");

            // 1. Add Score field
            var addScoreFieldStage = BsonDocument.Parse(@"
            {
                $addFields: {
                    Score: { $meta: ""searchScore"" }
                }
            }");

            // 2. Filter by the projected score
            var filterByScoreStage = BsonDocument.Parse(@"
            {
                $match: {
                    Score: { $gt: 0.5 }
                }
            }");

            var results = await _catalogContext.Products.Aggregate()
                                                        .AppendStage<Product>(autocompleteStage)
                                                        .AppendStage<Product>(addScoreFieldStage)
                                                        .AppendStage<Product>(filterByScoreStage)
                                                        .Limit(10) // Limit the number of suggestions
                                                        .ToListAsync();

            return results;
        }

        /// <summary>
        /// Finds products semantically relevant to the input text using vector search.
        /// </summary>
        /// <param name="text">The input text for semantic search.</param>
        /// <returns>A list of semantically relevant products.</returns>
        [HttpGet("findwithsemanticrelevance/{text}", Name = "WithSemanticRelevance")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)] // Potentially from fallback
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)] // For embedding generation failure
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)] // For unexpected errors
        public async Task<ActionResult<IEnumerable<Product>>> WithSemanticRelevance(string text)
        {
            if (!_catalogAI.IsEnabled)
            {
                _logger.LogInformation("Catalog AI is not enabled, falling back to GetProductByName for '{text}'.", text);
                return await GetProductByName(text);
            }

            // Now, GetEmbeddingAsync directly returns float[], simplifying the code
            float[] queryVector = await _catalogAI.GetEmbeddingAsync(text);

            if (queryVector == null || !queryVector.Any()) // Check if the array is null or empty
            {
                _logger.LogWarning("Failed to generate embedding for text: '{text}' or generated an empty embedding.", text);
                return BadRequest("Could not generate a valid embedding for the provided text.");
            }

            // Define your VectorSearchOptions
            var vectorSearchOptions = new VectorSearchOptions<Product>()
            {
                // THIS IS CRUCIAL: Match the name of the index you created in mongosh
                IndexName = "vector_index",

                // This controls how many potential candidates the search engine considers
                // Increasing this can improve recall (finding more relevant results)
                // Common values are 100, 150, 200, or higher depending on dataset size
                NumberOfCandidates = 100 // Start with 100-200, adjust as needed
            };

            // Ensure Product has a 'double Score { get; set; }' property for mapping
            // And adjust ProductDocument if it also needs 'float[] Embedding'
            var pipeline = _catalogContext.Products.Aggregate()
                .VectorSearch(
                    document => document.Embedding, // The field in MongoDB holding the embeddings
                    queryVector,                    // The query vector (now directly float[])
                    limit: 10,                      // Limit the number of results from VectorSearch
                    vectorSearchOptions
                )
                .Project<Product>(Builders<Product>.Projection
                    .Include(p => p.Id)
                    .Include(p => p.Name)
                    .Include(p => p.Author)
                    .Include(p => p.Category)
                    .Include(p => p.Embedding)
                    .Meta("Score", "vectorSearchScore")
                );

            var results = await pipeline.ToListAsync();

            if (results == null || !results.Any())
            {
                // Return an empty list if no semantically relevant products are found
                // This is generally better than NotFound() for search results
                return Ok(Enumerable.Empty<Product>());
            }

            // Optional: Log the top results and their scores for debugging/analysis
            _logger.LogInformation("Found {Count} semantically relevant products. Top 3: {TopProducts}",
                results.Count,
                string.Join(", ", results.Take(3).Select(p => $"{p.Name} (Score: p.Score:F4)")));

            return Ok(results);
        }

        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var products = await _repository.GetProductByCategory(category);
            return Ok(products);
        }

        [Route("[action]/{name}", Name = "GetProductByName")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByName(string name)
        {
            var items = await _repository.GetProductByName(name);
            if (items == null)
            {
                _logger.LogError($"Products with name: {name} not found.");
                return NotFound();
            }
            return Ok(items);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _repository.CreateProduct(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            return Ok(await _repository.UpdateProduct(product));
        }

        [HttpDelete("{id}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductById(Guid id)
        {
            return Ok(await _repository.DeleteProduct(id));
        }
    }
}