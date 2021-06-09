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
    public class PopulerPost : ViewComponent
    {
        /*
         * ana sayfada sağ taraftaki sidebarı temsil eder.
         * burada 3 adet blog ve tüm kategoriler listeleneceği için blogandcategory modeli
         * kullanılmıştır. 
         * */
        private IUnitOfWork uow;
        public PopulerPost(IUnitOfWork _unitOfWork)
        {
            uow = _unitOfWork;
        }
        public IViewComponentResult Invoke()
        {
            // vote'a göre ilk 3 bloğu alır. 
            var blogs = uow.Blogs.GetAll()
                .Where(i => i.IsAppproved == true)
                .Include(i => i.Comments)
                .Select(i => new BlogDetail()
                {
                    Id = i.Id,
                    Title = i.Title,
                    DateAdded = i.DateAdded,
                    SlugUrl = i.SlugUrl,
                    ViewCount = i.ViewCount,
                    ImageUrl = i.ImageUrl,
                    totalComment = i.Comments.Count(),
                }).ToList().OrderByDescending(i => i.ViewCount).Take(3);
            return View(blogs);
        }
    }
}
