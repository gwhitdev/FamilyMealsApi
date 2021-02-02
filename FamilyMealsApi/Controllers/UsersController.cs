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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var authId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var user = await _userService.GetUserByIdAsync(authId);
            bool idsMatch = user.AuthId == authId;
            
            ResponseModel responseModel = new ResponseModel();

            if (idsMatch)
            {
                responseModel.Success = true;
                responseModel.Message = "User found.";
                responseModel.Data = new Data { User = user };
            }
            else
            {
                responseModel.Success = false;
                responseModel.Message = "User not found.";
                responseModel.Data = new Data { User = null };
            }
            
            if (idsMatch) return Ok(new[] { responseModel });
            return NotFound(new[] { responseModel });
        }

        [HttpGet("CreateUser")]
        public async Task<IActionResult> CreateUser()
        {
            var authId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userService.CreateDbUser(authId);

            var success = new ResponseModel
            {
                Success = true,
                Message = "User created.",
                Data = new Data { User = user }
            };

            if (user != null) return Ok(new[] { success });
            return BadRequest();
        }

        [HttpPut("UpdateUserIngredientList")]
        public async Task<IActionResult> UpdateUserIngredientList(User user)
        {
            string userId = user.UserId;
            
            var ingredientId = user.UserIngredients[0].ToString();
            
            var userUpdated = await _userService.UpdateUserAsync(userId, ingredientId);

            ResponseModel responseModel = new ResponseModel();
            
            if(userUpdated.UserIngredients == null)
            {
                responseModel.Success = false;
                responseModel.Message = "User not updated.";
                responseModel.Data = new Data { User = null };

                return NotFound(new[] { responseModel });
            }

            responseModel.Success = true;
            responseModel.Message = "User updated.";
            responseModel.Data = new Data { User = userUpdated };

            return Ok(new[] { responseModel });
        }
    }
}
