using System.Collections.Generic;

namespace FamilyMealsApi.Models
{
    public class ResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Data Data { get; set; }
        public string Instance { get; set; }
    }

    public class Data
    {
        public List<Ingredient> Ingredients { get; set; }
        public User User { get; set; }

    }
}
