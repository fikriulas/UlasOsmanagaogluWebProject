using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Entity;

namespace UlasBlog.WebUI.Models
{
    public class BlogEdit
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Title")]
        [StringLength(150, ErrorMessage = "Kategori Adı 150 Karakterden Uzun Olamaz.")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Description")]
        [StringLength(150, ErrorMessage = "Blog Açıklaması 150 Karakterden Uzun Olamaz.")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Content")]
        public string HtmlContent { get; set; }
        public short Vote { get; set; }
        public DateTime DateAdded { get; set; }
        public int AuthorId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAppproved { get; set; }
        public int totalComment { get; set; }
        public string SlugUrl { get; set; }
        public int ViewCount { get; set; }
        public bool IsHome { get; set; }
        public bool IsSlider { get; set; }
        public List<Category> Categories { get; set; }
        public List<Comment> Comments { get; set; }
        public Comment Comment { get; set; }
    }
    
}
