namespace FamilyMealsApi.Models
{
    public class IngredientsDatabaseSettings : IIngredientsDatabaseSettings
    {
        public string IngredientsCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IIngredientsDatabaseSettings
    {
        string IngredientsCollectionName { get; set; }
        string UsersCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
