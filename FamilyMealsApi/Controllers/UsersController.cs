using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FamilyMealsApi.Models;
using FamilyMealsApi.Services;

namespace FamilyMealsApi.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _userService;
        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(string authId)
        {
            await _userService.CreateDbUser(authId);

            return View();
        }
    }
}
