using ClipShare.Core.Entities;
using ClipShare.DataAccess.Data;
using ClipShare.Utility;
using ClipShare.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClipShare.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly Context _context;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            Context context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            var loginVM = new Login_vm()
            {
                ReturnUrl = returnUrl
            };

            return View(loginVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login_vm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.ReturnUrl = model.ReturnUrl ?? Url.Content("~/");

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(model.UserName);
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password. Please try again.");
                return View(model);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded)
            {
                await HandleSignInUserAsync(user);
                return LocalRedirect(model.ReturnUrl);
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, $"Your account has been locked. You should wait until {user.LockoutEnd} (UTC time) to be able to login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password. Please try again.");
                }

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register_vm model)
        {
            if (ModelState.IsValid)
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("ConfirmPassword", "Confirm password does not match password.");

                    return View(model);
                }

                if (await CheckEmailExistsAsync(model.Email))
                {
                    ModelState.AddModelError("Email", $"Email address of {model.Email} is taken. Please try using another email address");

                    return View(model);
                }

                if (await CheckNameExistsAsync(model.Name))
                {
                    ModelState.AddModelError("Name", $"The name of '{model.Name}' is taken. Please try another name.");

                    return View(model);
                }

                var userToAdd = new AppUser
                {
                    Name = model.Name,
                    UserName = model.Name.ToLower(),
                    Email = model.Email.ToLower()
                };

                var result = await _userManager.CreateAsync(userToAdd, model.Password);
                await _userManager.AddToRoleAsync(userToAdd, SD.UserRole);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }

                await HandleSignInUserAsync(userToAdd);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        #region Private Methods
        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }

        private async Task<bool> CheckNameExistsAsync(string name)
        {
            return await _userManager.Users.AnyAsync(x => x.Name.ToLower() == name.ToLower());
        }
        private async Task HandleSignInUserAsync(AppUser user)
        {
            var userChannelId = await _context.Channel
              .Where(x => x.AppUserId == user.Id)
              .Select(x => x.Id)
              .FirstOrDefaultAsync();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Sid, userChannelId.ToString()),
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);
        }
        #endregion
    }
}
