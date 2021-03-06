﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asmin.Business.Abstract;
using Asmin.Core.Entities.Concrete;
using Asmin.Entities.DTO;
using Asmin.WebMVC.Constants;
using Asmin.WebMVC.Filters;
using Asmin.WebMVC.Services.Session;
using Microsoft.AspNetCore.Mvc;

namespace Asmin.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private IUserManager _userManager;
        private ISessionService _sessionService;
        public User CurrentUser => _sessionService.GetObject<User>(SessionKey.CURRENT_USER);

        public HomeController(IUserManager userManager, ISessionService sessionService)
        {
            _userManager = userManager;
            _sessionService = sessionService;
        }

        [CheckSessionFilter]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            if (CurrentUser != null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto user)
        {
            var checkUser = await _userManager.Login(user);

            if (!checkUser.IsSuccess)
            {
                ModelState.AddModelError("Error", checkUser.Message);
                return View(user);
            }

            if (checkUser.Data == null)
            {
                ModelState.AddModelError("InvalidUser", "Invalid email or password.");
                return View(user);
            }

            _sessionService.SetObject(SessionKey.CURRENT_USER, checkUser.Data);

            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            _sessionService.Remove(SessionKey.CURRENT_USER);
            return RedirectToAction("Login");
        }
    }
}