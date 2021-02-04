using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FamilyMealsApi.Services;
using FamilyMealsApi.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FamilyMealsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IngredientsService _ingredientsService;
        private readonly UserService _userService;
        private readonly ILogger _logger;
        public IngredientsController(IngredientsService ingredientsService, UserService userService, ILoggerFactory loggerFactory)
        {
            _ingredientsService = ingredientsService;
            _userService = userService;
            _logger = loggerFactory.CreateLogger<IngredientsController>();
        }

        // GET: api/ingredients
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Ingredient>))]
        [ProducesResponseType(404)]
        public IActionResult Get(string name)
        {
            
            var authId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (string.IsNullOrWhiteSpace(name))
            {
                var fetchAll = new ResponseModel
                {
                    Success = true,
                    Message = "Found ingredients.",
                    Data = new Data { Ingredients = _ingredientsService.Get(authId) }
                    
                };
                /*
                var successResponse = new[]
                {
                    fetchAll
                };*/
                return Ok(new[] { fetchAll });
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
                /*
                var successResponse = new []
                {
                    success
                };*/
                return Ok(new[] { success });
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
        public ActionResult<string> Create([FromBody] Ingredient ingredient)
        {
            var authId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //string ownerId = "";
            Ingredient ingredientToCreate = new Ingredient();
            if (ingredient.Owner == authId) // validate owner Id matches with tokenId
            {
                //ownerId = ingredient.Owner;
                _logger.LogDebug("TRYING TO CREATE INGREDIENT...");
                ingredientToCreate = _ingredientsService.Create(ingredient);
                _logger.LogDebug($"INGREDIENT CREATED: {ingredientToCreate.Id}");
            }
            else
            {
                return NotFound();
            }
            _logger.LogDebug($"NEW INGREDIENT ID: {ingredientToCreate.Id}");
            return CreatedAtRoute(
                routeName: nameof(GetById),
                routeValues: new { id = ingredientToCreate.Id.ToString() },
                value: ingredientToCreate);
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
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(string id)
        {
            var responseModel = new ResponseModel();
            var ingredientConfirmedToExist = _ingredientsService.GetById(id);
            var ownerId = ingredientConfirmedToExist.Owner;
            _logger.LogDebug($"USER ID: {ownerId}");

            if (ingredientConfirmedToExist == null)
            {
                responseModel.Success = false;
                responseModel.Message = "Ingredient not found.";
                responseModel.Data = new Data { };
                responseModel.Instance = HttpContext.Request.Path;
                return NotFound(new[] { responseModel });
            }

            bool ingredientRemoved = await _ingredientsService.Remove(ingredientConfirmedToExist.Id);
            _logger.LogDebug($"INGREDIENT REMOVED FROM DATABASE: {ingredientRemoved}");
            //User updatedUser = new User();
            bool updatedUser = false;
            if (ingredientRemoved)
            {
                _logger.LogDebug("ATTEMPTING TO REMOVE INGREDIENT FROM USER INGREDIENTS...");
                updatedUser = _userService.RemoveIngredientFromUser(ownerId, id);
                //_logger.LogDebug($"updatedUser: {updatedUser.UserId}");
                //_logger.LogDebug($"updatedUserIngredients.Count = {updatedUser.UserIngredients.Count}");
                _logger.LogDebug($"User updated?: {updatedUser}");

            }
            _logger.LogDebug($"REMOVED INGREDIENT TRUE?: {ingredientRemoved}");
            //_logger.LogDebug($"USER CONTAINS INGREDIENT ID?: {updatedUser.UserIngredients.Contains(id)}");
            if (!ingredientRemoved || !updatedUser)
            {
                responseModel.Success = false;
                responseModel.Message = "Something went wrong.";
                responseModel.Data = new Data { };
                responseModel.Instance = HttpContext.Request.Path;
                return BadRequest(new[] { responseModel });
            }

            responseModel.Success = true;
            responseModel.Message = "Successfully deleted ingredient.";
            responseModel.Data = new Data { };
            return Ok(new[] { responseModel });
        }
    }
}
