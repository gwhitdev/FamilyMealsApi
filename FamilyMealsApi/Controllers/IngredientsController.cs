using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FamilyMealsApi.Services;
using FamilyMealsApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FamilyMealsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IngredientsService _ingredientsService;

        public IngredientsController(IngredientsService ingredientsService)
        {
            _ingredientsService = ingredientsService;
        }

        // GET: api/ingredients
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Ingredient>))]
        [ProducesResponseType(404)]
        public IActionResult Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Ok(_ingredientsService.Get());
            }
            else
            {
                name = name.ToLower();
                IEnumerable<Ingredient> ingredients = (IEnumerable<Ingredient>)_ingredientsService.GetIngredientsByName(name);
                if (ingredients == null)
                {
                    return NotFound();
                }
                return Ok(ingredients);
            }
        }

        // GET api/ingredients/5
        [HttpGet("{id}", Name = nameof(GetById))]
        [ProducesResponseType(200, Type = typeof(Ingredient))]
        [ProducesResponseType(400)]
        public IActionResult GetById(string id)
        {
            var ingredient = _ingredientsService.GetById(id);

            if (ingredient == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://localhost:5001/ingredients/id-not-valid",
                    Title = $"Ingredient ID {id} provided is not valid.",
                    Detail = "Please make sure you have provided a corrent MongoDB ObjectID.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            return Ok(ingredient);
        }

        // POST api/ingredients
        [HttpPost]
        public ActionResult<Ingredient> Create([FromBody] Ingredient ingredient)
        {
            
            Ingredient added = _ingredientsService.Create(ingredient);

            return CreatedAtRoute(
                routeName: nameof(GetById),
                routeValues: new { id = added.Id.ToString() },
                value: added);
        }

        // PUT api/ingredients/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(string id, [FromBody] Ingredient ingredientIn)
        {
            id = id.ToLower();

            if (ingredientIn == null || ingredientIn.Id != id)
            {
                return BadRequest(); // 400 Bad Request
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 Bad Request
            }

            var existing = _ingredientsService.GetById(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource Not Found
            }

            _ingredientsService.Update(id, ingredientIn);

            var updatedIngredient = _ingredientsService.GetById(id);

            return Ok(updatedIngredient); // 200 Success
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ActionResult<IEnumerable<Message>> Delete(string id)
        {
            var ingredient = _ingredientsService.GetById(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            _ingredientsService.Remove(ingredient.Id);

            return new []
            {
                new Message { Body = "Successfully removed ingredient with ID: {id}" }
            };
            
        }
    }
}
