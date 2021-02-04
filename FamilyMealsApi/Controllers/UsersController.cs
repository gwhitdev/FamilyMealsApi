using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FamilyMealsApi.Models;
using FamilyMealsApi.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FamilyMealsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserService _userService;
        private readonly ILogger _logger;
        public UsersController(UserService userService, ILoggerFactory loggerFactory)
        {
            _userService = userService;
            _logger = loggerFactory.CreateLogger<UsersController>();
        }

        [HttpGet("CreateUser")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUser()
        {
            var responseModel = new ResponseModel();

            var authId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userService.CreateDbUser(authId);

            if (user != null && user.AuthId == authId)
            {
                responseModel.Success = true;
                responseModel.Message = "User created.";
                responseModel.Data = new Data { User = user };
                return Ok(new[] { responseModel });
            }

            responseModel.Success = false;
            responseModel.Message = "Error.";
            responseModel.Data = new Data { User = null };
            return BadRequest(new[] { responseModel });
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get()
        {
            var authId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var user = await _userService.GetUserByIdAsync(authId);
            bool idsMatch = user.AuthId == authId;
            
            ResponseModel responseModel = new ResponseModel();

            if (!idsMatch || user == null)
            {
                responseModel.Success = false;
                responseModel.Message = "User not found.";
                responseModel.Data = new Data { User = null };
                return NotFound(new[] { responseModel });
            }

            responseModel.Success = true;
            responseModel.Message = "User found.";
            responseModel.Data = new Data { User = user };
            return Ok(new[] { responseModel });
        }

        [HttpGet("GetUserIngredients")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserIngredients()
        {
            ResponseModel responseModel = new ResponseModel();

            var authId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            List<Ingredient> userIngredients = await _userService.GetUserIngredientsAsync(authId);

            if (userIngredients.Count > 0)
            {
                responseModel.Success = true;
                responseModel.Message = "User ingredients found.";
                responseModel.Data = new Data { Ingredients = userIngredients };
                return Ok(new[] { responseModel });
            }

            if (userIngredients != null && userIngredients.Count == 0)
            {
                responseModel.Success = true;
                responseModel.Message = "User returned correctly but 0 ingredients present.";
                responseModel.Data = new Data { Ingredients = userIngredients };
                return Ok(new[] { responseModel });
            }

            responseModel.Success = false;
            responseModel.Message = "Error. User ingredients is null.";
            responseModel.Data = null;
            return NotFound(new[] { responseModel });

        }

        [HttpPut("UpdateUserIngredientList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUserIngredientList(User user)
        {
            ResponseModel responseModel = new ResponseModel();

            string userId = user.UserId;
            var ingredientId = user.UserIngredients[0].ToString();
            var userUpdated = await _userService.UpdateUserAsync(userId, ingredientId);
            
            if(userUpdated.UserIngredients.Count > 0 && userUpdated.UserIngredients != null)
            {
                responseModel.Success = true;
                responseModel.Message = "User ingredients updated.";
                responseModel.Data = new Data { User = userUpdated };
                return Ok(new[] { responseModel });
            }

            responseModel.Success = false;
            responseModel.Message = "Error. User ingredients could not be updated.";
            responseModel.Data = new Data { User = userUpdated };
            return NotFound(new[] { responseModel });
        }
    }
}
