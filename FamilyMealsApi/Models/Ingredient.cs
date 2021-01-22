#nullable enable
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace FamilyMealsApi.Models
{
    public class Ingredient
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; internal set; }
        [BsonElement("Owner")]
        public string? Owner { get; set; } // Will receive string authId from JWT
        [BsonElement("Details")]
        public Details? Details { get; set; }

        [BsonDateTimeOptions(Kind=DateTimeKind.Local)]
        [BsonElement("createdAt")]
        private DateTime _CreatedAt;

        public DateTime CreatedAt
        {
            get => _CreatedAt;
            
            internal set 
            {
                if (CreatedAt > DateTime.Now)
                {
                    _CreatedAt = DateTime.Now;
                }
                else
                {
                    _CreatedAt = CreatedAt;
                }
            }
        }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; internal set; }

        [BsonElement("__v")]
        public int __v { get;}

        
    }

    public class Details
    {
        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("quantity")]
        public double Quantity { get; set; }

        [BsonElement("quantityType")]
        public string? QuantityType { get; set; }

        [BsonElement("keptAt")]
        public string? KeptAt { get; set; }

        [BsonElement("useByDate")]
        public DateTime UseByDate { get; set; }
    }
}
