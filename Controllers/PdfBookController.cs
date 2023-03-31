    using Bookinist.Context;
using Bookinist.Models.Entity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bookinist.Controllers
{
    public class PdfBookController : Controller
    {

        private readonly BookinistContext _context;
        private IWebHostEnvironment _env;
        public PdfBookController(BookinistContext context, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
        }
        /// /////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> Index()
        {
            var pdfbook = await GetAll();
            return View(pdfbook);
        }
        [NonAction]
        public async Task<List<PdfBook>> GetAll()
        {
            var pdfbook = await _context.PdfBooks.Select(p => new PdfBook
            {
                Id = p.Id,
                Name = p.Name,
                PathVal = p.PathVal,
                UserId = p.User.Id,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UserName = p.User.UserName,
                Status = p.Status
            }).Where(p => p.Status == true).ToListAsync();
            return pdfbook;
        }
        // /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(PdfBook model, IFormFile file)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string fileName = DateTime.Now.ToString("dd/MM/yy/HH/mm/ss") + ".pdf";
            var dir = _env.WebRootPath;
            var fullpath = Path.Combine(dir, "PDF", fileName);

            using (var fileStream = new FileStream(Path.Combine(dir, "PDF", fileName), FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }


            //Это создает экземпляр нашей модели песни, сохраняет его как переменную и сохраняет в базе данных
            var pdfBook = new PdfBook
            {
                Name = model.Name,
                UserId = int.Parse(currentUserId),
                PathVal = fullpath,
                Description = model.Description,
                Status = false,
                CreatedAt = DateTime.Now.ToString("dd/MM/yy/HH/mm/ss")
            };

            _context.PdfBooks.Add(pdfBook);
            await _context.SaveChangesAsync();

            return RedirectToAction("GetMyPdfBooks");
        }
        // //////////////////////////////////////////////////////////////////////////////////////////////

        public async Task<IActionResult> Delete(int id)
        {
            var pdf = await _context.PdfBooks.FindAsync(id);
            _context.PdfBooks.Remove(pdf);
            await _context.SaveChangesAsync();
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("GetAllPdfBooks");
            }
            else
            {
                return RedirectToAction("GetMyPdfBooks");
            }
        }
        /// ///////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> Edit(int id)
        {

            var pdf = await _context.PdfBooks.FindAsync(id);
            if (pdf == null)
            {
                return NotFound();
            }
            return View(pdf);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PdfBook model)
        {
            var pdfbook = await _context.PdfBooks.FindAsync(model.Id);
            if (pdfbook == null)
            {
                return RedirectToAction("Index");
            }
            pdfbook.Name = model.Name;
            pdfbook.Description = model.Description;

            if (User.IsInRole("Admin"))
            {
                pdfbook.Status = model.Status;
            }
            else
            {
                pdfbook.Status = false;
            }
            await _context.SaveChangesAsync();
            if (User.IsInRole("User"))
            {
                return RedirectToAction("GetMyPdfBooks");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        // //////////////////////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> GetMyPdfBooks()
        {
            var pdfbooks = await GetMypdfBook();

            return View(pdfbooks);

        }
        [NonAction]
        public async Task<List<PdfBook>> GetMypdfBook()
        {
            var book = await _context.PdfBooks.Select(p => new PdfBook
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status,
                UserId = p.UserId,
                UserName = p.User.UserName,
                CreatedAt = p.CreatedAt,
            }).Where(p => p.UserName == User.Identity.Name).ToListAsync();
            return book;
        }
        public async Task<IActionResult> GetAllPdfBooks()
        {
            var pdfbook = await GetAllPdf();
            return View(pdfbook);
        }
        [NonAction]
        public async Task<List<PdfBook>> GetAllPdf()
        {
            var pdfbook = await _context.PdfBooks.Select(p => new PdfBook
            {
                Id = p.Id,
                Name = p.Name,
                PathVal = p.PathVal,
                UserId = p.User.Id,
                UserName = p.User.UserName,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                Status = p.Status
            }).ToListAsync();
            return pdfbook;
        }
    }
}
