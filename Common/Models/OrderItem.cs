﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    [Serializable]
    public record OrderItem
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; init; } = Guid.NewGuid();

        [Required]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid ProductId { get; init; }

        public required string ProductName { get; init; }

        [Required]
        public decimal ProductPrice { get; init; }

        [Required]
        public ushort Quantity { get; set; }

        public string? ImageFile { get; set; }
    }
}
