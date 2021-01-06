using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FamilyMealsApi.Services;
using FamilyMealsApi.Models;
using System;

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
                var fetchAll = new ResponseModel
                {
                    Success = true,
                    Message = "Found ingredients.",
                    Data = new Data { Ingredients = _ingredientsService.Get() }
                    
                };
                var successResponse = new[]
                {
                    fetchAll
                };
                return Ok(successResponse);
            }
            else
            {

                
                name = name.ToLower();
                List<Ingredient> ingredients = _ingredientsService.GetIngredientsByName(name);
                if (ingredients == null || ingredients.Count == 0)
                {
                    var notFound = new ResponseModel
                    {
                        Success = false,
                        Message = "Not found",
                        Data = null,
                        Instance = HttpContext.Request.Path
                    };

                    var notFoundResponse = new[]
                    {
                        notFound
                    };
                
                    return NotFound(notFoundResponse);
                }

                var success = new ResponseModel
                {
                    Success = true,
                    Message = "Found named ingredients.",
                    Data = new Data {Ingredients = ingredients}
                };

                var successResponse = new []
                {
                    success
                };
                return Ok(successResponse);
            }
        }

        // GET api/ingredients/5
        [HttpGet("{id}", Name = nameof(GetById))]
        [ProducesResponseType(200, Type = typeof(Ingredient))]
        [ProducesResponseType(400)]
        public IActionResult GetById(string id)
        { 
            var result = _ingredientsService.GetById(id);
            List<Ingredient> ingredient = new List<Ingredient>() { result } ;
           
            if (result == null)
            {
                var badRequest = new ResponseModel
                {
                    Success = false,
                    Message = "Bad Request.",
                    Data = null,
                    Instance = HttpContext.Request.Path
                };

                var badRequestResponse = new[]
                {
                    badRequest
                };
                return BadRequest(badRequestResponse);
            }

            var success = new ResponseModel
            {
                Success = true,
                Message = $"Found ingredient by ID: {id}.",
                Data = new Data { Ingredients = ingredient }
            };

            var successResponse = new[]
            {
                success
            };

            return Ok(successResponse);
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
        public IActionResult Update(string id, [FromBody] Details detailsIn)
        {
            id = id.ToLower();

            if (detailsIn == null)
            {
                var badRequest = new ResponseModel
                {
                    Success = false,
                    Message = "The details provided are null.",
                    Data = null,
                    Instance = HttpContext.Request.Path
                };

                var badRequestResponse = new[]
                {
                    badRequest
                };

                return BadRequest(badRequestResponse); // 400 Bad Request
            }

            if (!ModelState.IsValid)
            {
                var invalidModelState = new ResponseModel
                {
                    Success = false,
                    Message = "Not all fields were supplied: {ModelState}",
                    Data = null,
                    Instance = HttpContext.Request.Path
                };

                var invalidModelStateResponse = new[]
                {
                    invalidModelState
                };

                return BadRequest(invalidModelStateResponse); // 400 Bad Request
            }

            var existing = _ingredientsService.GetById(id);

            if (existing == null)
            {
                var notFound = new ResponseModel
                {
                    Success = false,
                    Message = "Ingredient not found",
                    Data = null,
                    Instance = HttpContext.Request.Path
                };

                var notFoundResponse = new[]
                {
                    notFound
                };

                return NotFound(notFoundResponse); // 404 Resource Not Found
            }

            _ingredientsService.Update(id, detailsIn);

            var updatedIngredient = _ingredientsService.GetById(id);
            List<Ingredient> ingredient = new List<Ingredient>() { updatedIngredient };
            
            var success = new ResponseModel
            {
                Success = true,
                Message = $"Ingredient with Id: {id} found.",
                Data = new Data { Ingredients = ingredient }
            };

            var successResponse = new[]
            {
                success
            };

            return Ok(successResponse); // 200 Success
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ActionResult<IEnumerable<ResponseModel>> Delete(string id)
        {
            var ingredient = _ingredientsService.GetById(id);

            if (ingredient == null)
            {
                var notFound = new ResponseModel
                {
                    Success = false,
                    Message = "Not found",
                    Data = null,
                    Instance = HttpContext.Request.Path
                };

                var notFoundResponse = new[]
                {
                    notFound
                };

                return NotFound(notFoundResponse);
            }

            _ingredientsService.Remove(ingredient.Id);

            var success = new ResponseModel
            {
                Success = true,
                Message = "Successfully deleted ingredient.",
                Data = null
            };

            var successResponse = new[]
            {
                success
            };

            return Ok(successResponse);
            
        }
    }
}
