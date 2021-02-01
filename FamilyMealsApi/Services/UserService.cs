using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using FamilyMealsApi.Models;
using MongoDB.Bson;

namespace FamilyMealsApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        public UserService(IIngredientsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>(settings.UsersCollectionName);
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

        public bool UpdateUser(string userId, string ingredientId)
        {
            var filter = Builders<User>.Filter.Eq(s => s.AuthId, userId);
            var update = Builders<User>.Update
                    .AddToSet("userIngredients", ingredientId);
            var response = _users.UpdateOne(filter, update);

            return response.IsAcknowledged;

        }

        public async Task<bool> GetUserById(string authId)
        {
            var response = await _users.Find<User>(user => user.AuthId == authId).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(response.UserId)) return true;
            return false;

        }
    }
}
