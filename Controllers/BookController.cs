using Bookinist.Context;
using Bookinist.Models.DTO;
using Bookinist.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Bookinist.Controllers
{
    public class BookController:Controller
    {
        private readonly BookinistContext _bookinistContext;
        private IWebHostEnvironment _env;

        public BookController(BookinistContext bookinistContext, IWebHostEnvironment env)
        {
            _bookinistContext = bookinistContext;
            _env = env;            
        }
        // Home
        [HttpGet]
        public async Task<IActionResult> Home()
        {
            var books = _bookinistContext.Books.ToList();

            var res = books.Select(BookDTO.FromEntity).ToList();
            var result = books.Select(BookDTO.FromEntity).Where(p=>p.Status == true && p.TypeId == 2).ToList();

            return View(result);
        }
        
        // Get my Books
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyBooks()
        {
            var books = await _bookinistContext.Books.ToListAsync();
            var result = books.Select(BookDTO.FromEntity)
                .Where(p => p.UserName == User.Identity.Name).ToList();

            return View(result);
        }
        
        // Get ForSale
        [HttpGet]
        public async Task<IActionResult> ForSale()
        {
            var books = await _bookinistContext.Books.ToListAsync();

            var res = books.Select(BookDTO.FromEntity).ToList();
            var result = books.Select(BookDTO.FromEntity).Where(p=>p.Status == true && p.TypeId == 1).ToList();

            return View(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var book = new BookDTO
            {
                Categories = await _bookinistContext
                    .Categories.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                    .ToListAsync(),
                Types = await _bookinistContext.BookTypes
                    .Select(type => new SelectListItem { Value = type.Id.ToString(), Text = type.Type }).ToListAsync()
            };

            return View(book);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(BookDTO model,IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _bookinistContext
                    .Categories
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToListAsync();
                model.Types = await _bookinistContext.BookTypes
                    .Select(type => new SelectListItem { Value = type.Id.ToString(), Text = type.Type }).ToListAsync();
                return View(model);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var filename = DateTime.Now.ToString("dd/MM/yy/HH/mm/ss") + ".jpg";
            var dir = _env.WebRootPath;
            var fullPath = Path.Combine(dir, "image", filename);


            using (var fileStream = new FileStream(Path.Combine(dir, "image", filename), FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }

            var book = new Book
            {
                Name = model.Name,
                Author = model.Author,
                Price = model.Price,
                Exchange = model.Exchange,
                Image = fullPath,
                CategoryId = model.CategoryId,
                TypeId = model.TypeId,
                ShortDesc = model.ShortDesc,
                LongDesc = model.LongDesc,
                Status = false,
                UserId = int.Parse(currentUserId),
                CreatedAt = DateTime.Now.ToString("dd/MM/yy/HH/mm/ss")
            };

            _bookinistContext.Books.Add(book);
            await _bookinistContext.SaveChangesAsync();


            return RedirectToAction("Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookinistContext.Books.FindAsync(id);
            if (book==null)
            {
                return RedirectToAction("Home");
            }
            var result = new BookDTO
            {   
                Id = book.Id,
                Name = book.Name,
                Author = book.Author,
                Price = book.Price,
                Image=book.Image,
                ShortDesc = book.ShortDesc,
                LongDesc = book.LongDesc,
                UserId = book.UserId,
                Status = book.Status,
                CategoryId = book.CategoryId,
                Categories = await _bookinistContext.Categories
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToListAsync()
            };
            return View(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(BookDTO model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _bookinistContext.Categories
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToListAsync();
                return View(model);
            }

            var book = await _bookinistContext.Books.FindAsync(model.Id);
            if (book == null)
            {
                return RedirectToAction("Home");
            }

            book.Name = model.Name;
            book.Author = model.Author;
            book.Price = model.Price;
            book.ShortDesc = model.ShortDesc;
            book.LongDesc = model.LongDesc;
            book.CategoryId = model.CategoryId;
            book.Image = model.Image;
            if (User.IsInRole("Admin"))
            {
                book.Status = model.Status;
            }
            else
            {
                book.Status = false;
            }
            book.UpdatedAt = DateTime.Now.ToString("dd/MM/yy/HH/mm/ss");

            await _bookinistContext.SaveChangesAsync();

            return RedirectToAction(User.IsInRole("User") ? "GetMyBooks" : "Index");
        }
        
        
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookinistContext.Books.FindAsync(id);
            if (book == null)
            {
                RedirectToAction("Home");
            }
            _bookinistContext.Remove(book);
            
            await _bookinistContext.SaveChangesAsync();
            return RedirectToAction("Home");
        }
        
        //index----------------------------------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var books = await _bookinistContext.Books.ToListAsync();
            var result = books.Select(BookDTO.FromEntity).ToList();

            return View(result);
        } 
        
        //Details---------------------------------------------------------------------------------------------

        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookinistContext.Books.ToListAsync();
            var firstOrDefault = book
                .Select(BookDTO.FromEntity).FirstOrDefault(p => p.Id == id);

            return View(firstOrDefault);
        }
    }
}
