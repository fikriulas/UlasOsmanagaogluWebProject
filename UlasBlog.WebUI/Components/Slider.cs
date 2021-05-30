using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;
using UlasBlog.WebUI.Models;

namespace UlasBlog.WebUI.Components
{
    public class Slider : ViewComponent
    {
        /*
         * ana sayfada sağ taraftaki sidebarı temsil eder.
         * burada 3 adet blog ve tüm kategoriler listeleneceği için blogandcategory modeli
         * kullanılmıştır. 
         * */
        private IUnitOfWork uow;
        public Slider(IUnitOfWork _unitOfWork)
        {
            uow = _unitOfWork;
        }
        public IViewComponentResult Invoke()
        {
            // vote'a göre ilk 3 bloğu alır. 
            var blogs = uow.Blogs.GetAll()
                .Where(i => i.IsSlider == true)
                .Include(i => i.Comments)
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.Category)
                .Select(i => new BlogDetail()
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    DateAdded = i.DateAdded,
                    Vote = i.Vote,
                    ImageUrl = i.ImageUrl,
                    totalComment = i.Comments.Count(),
                    Categories = i.BlogCategories.Select(b => b.Category).ToList()
                }).ToList();
            return View(blogs);
        }
    }
}
