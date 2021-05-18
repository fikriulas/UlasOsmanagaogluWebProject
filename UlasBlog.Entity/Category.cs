using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UlasBlog.Entity
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Kategori Adı")]        
        [StringLength(35, ErrorMessage = "Kategori Adı 35 Karakterden Uzun Olamaz.")]
        public string Name { get; set; }
        public List<BlogCategory> BlogCategories { get; set; }
    }
}
