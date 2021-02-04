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
using Microsoft.Extensions.Logging;

namespace FamilyMealsApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Ingredient> _ingredients;
        private readonly ILogger _logger;
        public UserService(IIngredientsDatabaseSettings settings, ILoggerFactory loggerFactory)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>(settings.UsersCollectionName);
            _ingredients = database.GetCollection<Ingredient>(settings.IngredientsCollectionName);
            _logger = loggerFactory.CreateLogger<UserService>();
        }

        public async Task<User> CreateDbUser(string userId)
        {
            var user = new User
            {
                AuthId = userId,
                CreatedAt = DateTime.Now
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
            _logger.LogDebug($"AUTH ID = {authId}");
            _logger.LogDebug("GETTING USER...");
            var user = await _users.Find(user => user.AuthId == authId).FirstAsync();
            
            _logger.LogDebug($"NUMBER OF INGREDIENTS FOUND: {user.UserIngredients.Count}");
            for (var i = 0; i < user.UserIngredients.Count; i++)
            {
                _logger.LogDebug($"{user.UserIngredients[i]}");
            }
            List<Ingredient> populatedIngredients = new List<Ingredient>();

            if (user.UserIngredients.Count > 0)
            {
                foreach (var ingredientId in user.UserIngredients)
                {
                    IAsyncCursor<Ingredient> getIngredient = await _ingredients.FindAsync(i => i.Id == ingredientId);
                    Ingredient foundIngredient = getIngredient.FirstOrDefault();
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

        public async Task<User> RemoveIngredientFromUser(string ownerId, string ingredientId)
        {
            _logger.LogDebug($"REMOVING INGREDIENT {ingredientId} FROM USER {ownerId} INGREDIENTS...");
            var ingredientToRemove = ObjectId.Parse(ingredientId);
            _logger.LogDebug($"PARSED ID = {ingredientToRemove}");


            var filter = Builders<User>.Filter.Eq(u => u.AuthId, ownerId);
            var update = Builders<User>.Update.Pull(u => u.UserIngredients, ingredientId);
            var updatedUser = await _users.FindOneAndUpdateAsync(filter, update);
            _logger.LogDebug($"REMOVED ELEMENT FROM USER INGREDIENTS? = {updatedUser.UserIngredients.Count}");
            return updatedUser;
        }
        
    }
}
