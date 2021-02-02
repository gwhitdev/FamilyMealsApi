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

        public async Task<User> UpdateUserAsync(string userId, string ingredientId)
        {
            var filter = Builders<User>.Filter.Eq(s => s.UserId, userId);
            var update = Builders<User>.Update
                    .AddToSet("userIngredients", ingredientId);
            User updatedUser = await _users.FindOneAndUpdateAsync(filter, update);
            return updatedUser;
        }

        public async Task<User> GetUserByIdAsync(string authId)
        {
            var user = await _users.Find<User>(user => user.AuthId == authId).FirstAsync();
            //if (!string.IsNullOrEmpty(user.UserId)) return user;
            //return null;
            return user;

        }
    }
}
