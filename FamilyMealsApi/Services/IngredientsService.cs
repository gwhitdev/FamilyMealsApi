using System.Linq;
using MongoDB.Driver;
using FamilyMealsApi.Models;
using System.Collections.Generic;
using MongoDB.Bson;
using System;
using static System.Console;

namespace FamilyMealsApi.Services
{
    public class IngredientsService
    {
        private readonly IMongoCollection<Ingredient> _ingredients;

        public IngredientsService(IIngredientsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _ingredients = database.GetCollection<Ingredient>(settings.IngredientsCollectionName);
        }

        public List<Ingredient> Get()
        {
            var result = _ingredients.Find(ingredient => true).ToList();
            if (result.Count > 0)
            {
                foreach (var item in result)
                {
                    WriteLine(item.Details.Name);
                }
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

        public void Remove(Ingredient ingredientIn) =>
            _ingredients.DeleteOne(ingredient => ingredient.Id == ingredientIn.Id);

        public void Remove(string id) =>
            _ingredients.DeleteOne(ingredient => ingredient.Id == id);
    }
}
