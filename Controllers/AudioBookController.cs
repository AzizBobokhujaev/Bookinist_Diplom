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
    public class AudioBookController : Controller
    {
        private  readonly BookinistContext _context;
        private IWebHostEnvironment _env;
        public AudioBookController(BookinistContext context, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
        }
        /// /////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> Index()
        {
            var audiobook = await GetAll();
            return View(audiobook);
        }
        [NonAction]
        public async Task<List<AudioBook>> GetAll()
        {
            var audiobook = await _context.AudioBooks.Select(p => new AudioBook
            {
                Id = p.Id,
                Name = p.Name,
                PathVal = p.PathVal,
                UserId = p.User.Id,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UserName=p.User.UserName,
                Status=p.Status
            }).Where(p=>p.Status==true).ToListAsync();
            return audiobook;
        }
        // /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create( AudioBook model, IFormFile file)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string fileName = DateTime.Now.ToString("dd/MM/yy/HH/mm/ss") + ".mp3";
            var dir = _env.WebRootPath;
            var fullpath = Path.Combine(dir, "Music", fileName);

            using (var fileStream = new FileStream(Path.Combine(dir, "Music", fileName), FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }


            //Это создает экземпляр нашей модели песни, сохраняет его как переменную и сохраняет в базе данных
            var audioBook = new AudioBook {
                Name = model.Name,
                UserId = int.Parse(currentUserId),
                PathVal = fullpath,
                Description = model.Description,
                Status = false,
                CreatedAt = DateTime.Now.ToString("dd/MM/yy/HH/mm/ss")
            };

            _context.AudioBooks.Add(audioBook);
            await _context.SaveChangesAsync();

            return RedirectToAction("GetMyAudioBooks");
        }
        // //////////////////////////////////////////////////////////////////////////////////////////////

        public async Task<IActionResult> Delete(int id)
        {
            var song = await _context.AudioBooks.FindAsync(id);
            _context.AudioBooks.Remove(song);
            await _context.SaveChangesAsync();
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("GetAllAudioBooks");
            }
            else
            {
                return RedirectToAction("GetMyAudioBooks");
            }
        }
        /// ///////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> Edit(int id)
        {

            var song = await _context.AudioBooks.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            return View(song);
        }

        // POST: Songs/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AudioBook model)
        {
            var audiobook = await _context.AudioBooks.FindAsync(model.Id);
            if (audiobook==null)
            {
                return RedirectToAction("Index");
            }
            audiobook.Name = model.Name;
            audiobook.Description = model.Description;
            
            if (User.IsInRole("Admin"))
            {
                audiobook.Status = model.Status;
            }
            else
            {
                audiobook.Status = false;
            }
            await _context.SaveChangesAsync();
            if (User.IsInRole("User"))
            {
                return RedirectToAction("GetMyAudioBooks");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        // //////////////////////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> GetMyAudioBooks()
        {
            var audiobooks = await GetMyBook();

            return View(audiobooks);

        }
        [NonAction]
        public async Task<List<AudioBook>> GetMyBook()
        {
            var book = await _context.AudioBooks.Select(p => new AudioBook
            {
                Id = p.Id,
                Name = p.Name,
                Description=p.Description,
                Status = p.Status,
                UserId = p.UserId,
                UserName = p.User.UserName,
                CreatedAt = p.CreatedAt,
            }).Where(p => p.UserName == User.Identity.Name).ToListAsync();
            return book;
        }
        public async Task<IActionResult> GetAllAudioBooks()
        {
            var audiobook = await GetAllAudio();
            return View(audiobook);
        }
        [NonAction]
        public async Task<List<AudioBook>> GetAllAudio()
        {
            var audiobook = await _context.AudioBooks.Select(p => new AudioBook
            {
                Id = p.Id,
                Name = p.Name,
                PathVal = p.PathVal,
                UserId = p.User.Id,
                UserName=p.User.UserName,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                Status = p.Status
            }).ToListAsync();
            return audiobook;
        }

    }
}
