using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IEmailService emailService;

        public HomeController(
            UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager,
            IEmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                var result = await signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new IdentityUser
            {
                UserName = username,
                Email = ""
            };
            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(VerifyEmail), "Home", new { userId = user.Id, code = code }, Request.Scheme, Request.Host.ToString());
                await emailService.SendAsync("noahvandenbergh@hotmail.nl", "email verify", $"<a href=\"{link}\">verify email</a>", true);
                return RedirectToAction("EmailVerification");
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest();
            var result = await userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return View();
            }
            return BadRequest();
        }
        public IActionResult EmailVerification() => View();
    }
}
