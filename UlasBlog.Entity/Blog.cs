using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UlasBlog.Entity
{
    public class Blog
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Title")]
        [StringLength(150, ErrorMessage = "Blog Adı 35 Karakterden Uzun Olamaz.")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Description")]
        [StringLength(150, ErrorMessage = "Blog Açıklaması 35 Karakterden Uzun Olamaz.")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Content")]
        public string HtmlContent { get; set; }
        public short Vote { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public string AuthorId { get; set; }
        public bool IsAppproved { get; set; }
        public bool IsHome { get; set; }
        public bool IsSlider { get; set; }
        public string SlugUrl { get; set; }
        public int ViewCount { get; set; }
        public List<BlogCategory> BlogCategories { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
