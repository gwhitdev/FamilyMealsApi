using System.Linq;
using MongoDB.Driver;
using FamilyMealsApi.Models;
using System.Collections.Generic;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;
using static System.Console;
using Microsoft.Extensions.Logging;

namespace FamilyMealsApi.Services
{
    public class IngredientsService
    {
        private readonly IMongoCollection<Ingredient> _ingredients;
        private readonly IMongoCollection<User> _users;
        private readonly ILogger _logger;

        public IngredientsService(IIngredientsDatabaseSettings settings, ILoggerFactory loggerFactory)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _ingredients = database.GetCollection<Ingredient>(settings.IngredientsCollectionName);
            _users = database.GetCollection<User>(settings.UsersCollectionName);
            _logger = loggerFactory.CreateLogger<IngredientsService>();
        }

        public List<Ingredient> Get(string authId)
        {
            var result = _ingredients.Find(ingredient => true).ToList(); //CHANGE TO USER SERVICE, SEARCH FOR ID AND THEN POPULATE THE INGREDIENTS?

            // TODO: FILTER DB RESULTS AGAINST AUTHID FROM JWT
            if (result.Count > 0)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
            
        public Ingredient GetById(string id)
        {
            if (ObjectId.TryParse(id, out _))
            {
                try
                {
                    return _ingredients.Find<Ingredient>(ingredient => ingredient.Id == id).FirstOrDefault();
                }
                catch (MongoException)
                {
                    WriteLine("A MongoDB exception has caused an error.");
                    return null;
                }
            }
            else
            {
                WriteLine("The ID provided is not a valid ObjectID.");
                return null;
            }
        }
            
        public List<Ingredient> GetIngredientsByName(string name)
        {
            List<Ingredient> listofIngredients = _ingredients.Find(ingredient => true).ToList();
            List<Ingredient> result = (from ingredient in listofIngredients where ingredient.Details.Name.ToLower() == name select ingredient).ToList();
            return result;
        }
            
        public Ingredient Create(Ingredient ingredient)
        {
            _ingredients.InsertOne(ingredient);

            return ingredient;
        }

        public void Update(string id, Details detailsIn)
        {
            var filter = Builders<Ingredient>.Filter.Eq(s => s.Id, id);
            var update = Builders<Ingredient>.Update
                .Set(ingredient => ingredient.Details, detailsIn)
                .CurrentDate(s => s.UpdatedAt);
            try
            {
                _ingredients.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                WriteLine("Update exception: " + ex.Message);
            }
        }

        public async Task<bool> Remove(Ingredient ingredientIn)
        {
            DeleteResult removed = await _ingredients.DeleteOneAsync(ingredient => ingredient.Id == ingredientIn.Id);
            return removed.DeletedCount == 1;
        }
            

        public async Task<bool> Remove(string id)
        {
            DeleteResult removed = await _ingredients.DeleteOneAsync(ingredient => ingredient.Id == id);
            return removed.DeletedCount == 1;
        }
            
    }
}
