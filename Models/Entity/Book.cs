using Bookinist.Models.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bookinist.Models.Entity
{
    public class Book
    {
        
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Author { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int CategoryId { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
    }
}
