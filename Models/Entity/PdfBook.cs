using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bookinist.Models.Entity
{
    public class PdfBook
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string PathVal { get; set; }

        public string Description { get; set; }
        public string CreatedAt { get; set; }
        public bool Status { get; set; }



        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public string UserName { get; internal set; }
    }
}
