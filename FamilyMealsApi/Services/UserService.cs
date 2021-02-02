using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using FamilyMealsApi.Models;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json.Linq;

namespace FamilyMealsApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Ingredient> _ingredients;
        public UserService(IIngredientsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>(settings.UsersCollectionName);
            _ingredients = database.GetCollection<Ingredient>(settings.IngredientsCollectionName);
        }

        public async Task<User> CreateDbUser(string userId)
        {
            var user = new User
            {
                //UserId = null,
                //UserIngredients = null,
                AuthId = userId
            };

            try
            {
                await _users.InsertOneAsync(user);
                return user;
            }
            catch(MongoException)
            {
                return null;
            }
            
            
        }

        public async Task<User> GetUserByIdAsync(string authId)
        {
            var user = await _users.Find(user => user.AuthId == authId).FirstAsync();
            //if (!string.IsNullOrEmpty(user.UserId)) return user;
            //return null;
            return user;

        }

        public async Task<List<Ingredient>> GetUserIngredientsAsync(string authId)
        {
            var user = await _users.Find(user => user.AuthId == authId).FirstAsync();
            List<Ingredient> populatedIngredients = new List<Ingredient>();

            if (user.UserIngredients.Count > 0)
            {
                foreach (var ingredientId in user.UserIngredients)
                {
                    var getIngredient = await _ingredients.FindAsync(i => i.Id == ingredientId);
                    Ingredient foundIngredient = getIngredient.First();
                    populatedIngredients.Add(foundIngredient);
                }
            }

            return populatedIngredients;
        }

        public async Task<User> UpdateUserAsync(string userId, string ingredientId)
        {
            var parsedIngredientId = ObjectId.Parse(ingredientId);
            var filter = Builders<User>.Filter.Eq(u => u.UserId, userId);
            var update = Builders<User>.Update
                    .AddToSet("userIngredients", parsedIngredientId);
            User updatedUser = await _users.FindOneAndUpdateAsync(filter, update);
            return updatedUser;
        }

        
    }
}
