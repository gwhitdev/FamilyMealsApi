using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyMealsApi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }
        [BsonElement("authId")]
        public string AuthId { get; set; }
        [BsonElement("userIngredients")]
        public List<Ingredient> UserIngredients { get; set; }
    }
}
