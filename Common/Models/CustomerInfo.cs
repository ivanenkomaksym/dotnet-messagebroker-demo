﻿using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Models
{
    public record CustomerInfo
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid CustomerId { get; init; }

        [Required]
        public required string FirstName { get; init; }

        [Required]
        public required string LastName { get; init; }

        [Required]
        public required string Email { get; init; }
    }
}