﻿using System.Diagnostics;
using Common.Models;
using Microsoft.Extensions.AI;

namespace CatalogAPI.Services;

public sealed class CatalogAI : ICatalogAI
{
    private const int EmbeddingDimensions = 384;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    /// <summary>The web host environment.</summary>
    private readonly IWebHostEnvironment _environment;
    /// <summary>Logger for use in AI operations.</summary>
    private readonly ILogger _logger;

    public CatalogAI(IWebHostEnvironment environment, ILogger<CatalogAI> logger, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = null)
    {
        _embeddingGenerator = embeddingGenerator;
        _environment = environment;
        _logger = logger;
    }

    /// <inheritdoc/>
    public bool IsEnabled => _embeddingGenerator is not null;

    /// <inheritdoc/>
    public ValueTask<float[]> GetEmbeddingAsync(Product item) =>
        IsEnabled ?
            GetEmbeddingAsync(CatalogItemToString(item)) :
            ValueTask.FromResult<float[]>(null);

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<float[]>> GetEmbeddingsAsync(IEnumerable<Product> items)
    {
        if (IsEnabled)
        {
            long timestamp = Stopwatch.GetTimestamp();

            GeneratedEmbeddings<Embedding<float>> embeddings = await _embeddingGenerator.GenerateAsync(items.Select(CatalogItemToString));
            var results = embeddings.Select(m => m.Vector.Slice(0, EmbeddingDimensions).ToArray()).ToList();

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Generated {EmbeddingsCount} embeddings in {ElapsedMilliseconds}s", results.Count, Stopwatch.GetElapsedTime(timestamp).TotalSeconds);
            }

            return results;
        }

        return null;
    }

    /// <inheritdoc/>
    public async ValueTask<float[]> GetEmbeddingAsync(string text)
    {
        if (IsEnabled)
        {
            long timestamp = Stopwatch.GetTimestamp();

            var embedding = await _embeddingGenerator.GenerateVectorAsync(text);
            embedding = embedding[0..EmbeddingDimensions];

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Generated embedding in {ElapsedMilliseconds}s: '{Text}'", Stopwatch.GetElapsedTime(timestamp).TotalSeconds, text);
            }

            return embedding.ToArray();
        }

        return null;
    }

    private static string CatalogItemToString(Product item) => $"{item.Name} {item.Summary}";
}

