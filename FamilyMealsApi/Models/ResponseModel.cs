using System.Collections.Generic;

namespace FamilyMealsApi.Models
{
    public class ResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<Ingredient> Data { get; set; }
        public string Instance { get; set; }
    }
}
