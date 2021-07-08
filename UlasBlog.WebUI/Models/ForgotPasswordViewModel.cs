using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UlasBlog.WebUI.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "E-Mail Alanı Gereklidir")]
        [Display(Name = "E-Mail")]
        [EmailAddress(ErrorMessage = "E-Mail Adresinin Formatı Doğru Değil")]
        [StringLength(50, ErrorMessage = "E-Mail 50 Karakterden Uzun Olamaz.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Şifre Alanı Gereklidir")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        [StringLength(15, ErrorMessage = "Şifre 15 Karakterden Uzun Olamaz.")]
        public string NewPassword { get; set; }
    }
}
