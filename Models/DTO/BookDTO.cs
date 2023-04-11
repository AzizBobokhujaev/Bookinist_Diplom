using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bookinist.Models.Entity;

namespace Bookinist.Models.DTO
{
    public class BookDTO
    {
        
        public int Id { get; set; }
        [Display(Name = "Имя книги")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Имя автора")]
        [Required]
        public string Author { get; set; }
        [Display(Name = "Цена")]
        public decimal? Price { get; set; }
        [Display(Name = "Обмен на:")]
        public string? Exchange { get; set; }
        [Display(Name = "Картинка")]
        [DataType(DataType.Upload)]
        public string Image { get; set; }
        [Display(Name = "Краткое описание ")]
        [Required]
        public string ShortDesc { get; set; }
        [Display(Name = "Подробное описание")]
        [Required]
        public string LongDesc { get; set; }
        [Display(Name = "Категория")]
        [Required]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        [Display(Name = "Статус")]
        public bool Status { get; set; }
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Types { get; set; } = new List<SelectListItem>();
        public int UserId { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }

        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        [Display(Name = "Тип")]
        public int TypeId { get; set; }


        public static BookDTO FromEntity(Book entity)
        {
            return new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Author = entity.Author,
                Exchange = entity.Exchange,
                Price = entity.Price,
                ShortDesc = entity.ShortDesc,
                LongDesc = entity.LongDesc,
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category.Name,
                Status = entity.Status,
                UserId = entity.UserId,
                UserName = entity.User.UserName,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                TypeId = entity.TypeId
            };
        }
    }
}
