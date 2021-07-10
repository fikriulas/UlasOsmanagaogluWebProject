using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UlasBlog.WebUI.Models
{
    public class PasswordChange
    {
        [Display(Name = "Eski Şifre")]
        [Required(ErrorMessage = "Eski Şifre Alanı Gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage = "Şifre En Az 6 Karakterden Oluşmalıdır")]
        public string OldPassword { get; set; }
        [Display(Name = "Yeni Şifre")]
        [Required(ErrorMessage = "Yeni Şifre Alanı Gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Yeni Şifre En Az 6 Karakterden Oluşmalıdır")]        
        public string NewPassword { get; set; }
        [Display(Name = "Yeni Şifre Tekrar")]
        [Required(ErrorMessage = "Yeni Şifre Tekrar Alanı Gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Yeni Şifre Tekrar En Az 6 Karakterden Oluşmalıdır")]
        [Compare("NewPassword",ErrorMessage = "Şifreler Uyuşmuyor")]
        public string ConfirmPassword { get; set; }
    }
}
