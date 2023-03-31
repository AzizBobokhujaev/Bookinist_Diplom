using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bookinist.Models.DTO
{
    public class RegisterDTO
    {
        [Required]
        [Display(Name = "Логин")]
        public string UserName { get; set; }
        public string UserRole { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Почта")]
        public string Email { get; set; }
        [Required]
        [Display(Name ="Номер телефона")]
        public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
