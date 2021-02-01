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
        public string UserId { get; set; }
        [BsonElement("authId")]
        public string AuthId { get; set; }
        [BsonElement("userIngredients")]
        //[BsonRepresentation(BsonType.ObjectId)]
        public List<ObjectId> UserIngredients { get; set; } = new List<ObjectId>();
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = new DateTime();
        

        
    }

    public class UserIngredients
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string IngredientId { get; set; }
    }
}
