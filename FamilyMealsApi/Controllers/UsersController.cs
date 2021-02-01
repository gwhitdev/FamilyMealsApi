using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FamilyMealsApi.Models;
using FamilyMealsApi.Services;
using System.Security.Claims;

namespace FamilyMealsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserService _userService;
        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var authId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var response = await _userService.GetUserById(authId);
            if (response) return Ok(new[] { response });
            return NotFound(new[] { "false" });
        }

        [HttpGet("createuser")]
        public async Task<IActionResult> CreateUser()
        {
            var authId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var response = await _userService.CreateDbUser(authId);
            if (response != null) return Ok(new[] { response });
            return BadRequest();
        }
    }
}
