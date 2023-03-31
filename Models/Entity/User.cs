using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bookinist.Models.Entity
{
    public class User : IdentityUser<int>
    {
        public virtual ICollection<Book> Books { get; set; } 
        public virtual ICollection<AudioBook> AudioBooks { get; set; }
    }
}
