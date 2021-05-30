using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UlasBlog.Entity
{
    public class Comment
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="İsim Alanı Gereklidir.")]
        [Display(Name = "İsim")]
        [StringLength(35, ErrorMessage = "İsim Alanı 35 Karakterden Uzun Olamaz.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "E-Mail Alanı Gereklidir.")]
        [Display(Name = "E-Mail")]
        [StringLength(50, ErrorMessage = "E-Mail Alanı 50 Karakterden Uzun Olamaz.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mesaj Alanı Gereklidir.")]
        [Display(Name = "Mesaj")]
        [StringLength(500, ErrorMessage = "Mesaj Alanı 500 Karakterden Uzun Olamaz.")]
        public string Message { get; set; }
        public DateTime dateAdded { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
