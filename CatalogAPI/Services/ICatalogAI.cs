﻿using Common.Models;
using Pgvector;

namespace CatalogAPI.Services;

public interface ICatalogAI
{
    /// <summary>Gets whether the AI system is enabled.</summary>
    bool IsEnabled { get; }

    /// <summary>Gets an embedding vector for the specified text.</summary>
    ValueTask<Vector> GetEmbeddingAsync(string text);

    /// <summary>Gets an embedding vector for the specified catalog item.</summary>
    ValueTask<Vector> GetEmbeddingAsync(Product item);

    /// <summary>Gets embedding vectors for the specified catalog items.</summary>
    ValueTask<IReadOnlyList<Vector>> GetEmbeddingsAsync(IEnumerable<Product> item);
}