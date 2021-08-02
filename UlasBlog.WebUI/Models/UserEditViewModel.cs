using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UlasBlog.WebUI.Models
{
    public class UserEditViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage ="Kullanıcı Adı Alanı Gereklidir")]
        [Display(Name ="Kullanıcı Adı")]
        [StringLength(35, ErrorMessage = "Kullanıcı Adı 35 Karakterden Uzun Olamaz.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "E-Mail Alanı Gereklidir")]
        [Display(Name = "E-Mail")]
        [EmailAddress(ErrorMessage ="E-Mail Adresinin Formatı Doğru Değil")]
        [StringLength(50, ErrorMessage = "E-Mail 50 Karakterden Uzun Olamaz.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "İsim Alanı Gereklidir")]
        [Display(Name = "İsim")]
        [StringLength(20, ErrorMessage = "Kullanıcı Adı 35 Karakterden Uzun Olamaz.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyisim Alanı Gereklidir")]
        [Display(Name = "Soyisim")]
        [StringLength(35, ErrorMessage = "Kullanıcı Adı 35 Karakterden Uzun Olamaz.")]
        public string Surname { get; set; }
    }
}
