﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Common.Models.Warehouse
{
    public record OrderReserve
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }

        [Required]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid OrderId { get; set; }

        [Required]
        public List<ReservedStockItem> ReservedStockItems { get; set; } = new List<ReservedStockItem>();
    }
}
