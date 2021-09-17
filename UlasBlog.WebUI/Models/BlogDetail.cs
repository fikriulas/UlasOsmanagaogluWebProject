using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Entity;

namespace UlasBlog.WebUI.Models
{
    public class BlogDetail
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string HtmlContent { get; set; }
        public short Vote { get; set; }
        public DateTime DateAdded { get; set; }
        public string AuthorId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAppproved { get; set; }
        public int totalComment { get; set; }
        public string SlugUrl { get; set; }
        public int ViewCount { get; set; }
        public bool IsHome { get; set; }
        public bool IsSlider { get; set; }
        public bool SliderSettings { get; set; }
        public List<Category> Categories { get; set; }
        public List<Comment> Comments { get; set; }
        public Comment Comment { get; set; }
    }
    
}
