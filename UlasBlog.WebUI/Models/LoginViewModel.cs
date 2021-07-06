using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UlasBlog.WebUI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-mail Alanı Gereklidir")]
        [Display(Name = "E-Mail")]
        [EmailAddress]
        [StringLength(50, ErrorMessage = "E-Mail 50 Karakterden Uzun Olamaz")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Şifre Alanı Gereklidir")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Şifre 15 Karakterden Uzun Olamaz")]
        [MinLength(6,ErrorMessage = "Şifre En Az 6 Karakterden Oluşmalıdır")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
