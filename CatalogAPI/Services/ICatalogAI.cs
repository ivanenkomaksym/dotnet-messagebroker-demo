using Common.Models;

namespace CatalogAPI.Services;

public interface ICatalogAI
{
    /// <summary>Gets whether the AI system is enabled.</summary>
    bool IsEnabled { get; }

    /// <summary>Gets an embedding vector for the specified text.</summary>
    ValueTask<float[]> GetEmbeddingAsync(string text);

    /// <summary>Gets an embedding vector for the specified catalog item.</summary>
    ValueTask<float[]> GetEmbeddingAsync(Product item);

    /// <summary>Gets embedding vectors for the specified catalog items.</summary>
    ValueTask<IReadOnlyList<float[]>> GetEmbeddingsAsync(IEnumerable<Product> item);
}