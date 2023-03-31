using Bookinist.Context;
using Bookinist.Models.DTO;
using Bookinist.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookinist.Controllers
{
    public class AccountController : Controller
    {
        private readonly BookinistContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(BookinistContext context,SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _context.Users.ToListAsync();
            return View(result);
        }
        
        public async Task<IActionResult> UserInfo()
        {
            var user = await GetUser();
            return View(user);
        }
        
        [NonAction]
        public async Task<List<User>> GetUser()
        {
            var user = await _context.Users.Select(p => new User
            {
                Id = p.Id,
                UserName = p.UserName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,


            }).Where(p=>p.UserName==User.Identity.Name).ToListAsync();
            return user;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("LoginFail", "Неверный логин или пароль");
                return View(model);
            }

            return RedirectToAction("Home", "Book");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userManager.CreateAsync(new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber=model.PhoneNumber,
                
            },model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return View(model);
            }
            else
            {
                await _userManager.AddToRoleAsync(await _userManager.FindByEmailAsync(model.Email), "User");
            }
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var account = await _context.Users.FindAsync(id);
            if (account == null)
            {
                return RedirectToAction("Index");
            }
            return View(account);
        }
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(User model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _context.Users.FindAsync(model.Id);
            if (user ==null)
            {
                return RedirectToAction("Index");
            }

            user.UserName = model.UserName;
            user.NormalizedUserName = model.UserName.ToUpper();
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.NormalizedEmail = model.Email.ToUpper();

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var account = await _context.Users.FindAsync(id);

            if (account == null)
            {
                return RedirectToAction("Index");
            }
            _context.Users.Remove(account);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        

    }
}
