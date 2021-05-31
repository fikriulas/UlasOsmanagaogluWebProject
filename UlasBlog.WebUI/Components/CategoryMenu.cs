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
    public class CategoryMenu : ViewComponent
    {
        /*
         * ana sayfada sağ taraftaki sidebarı temsil eder.
         * burada 3 adet blog ve tüm kategoriler listeleneceği için blogandcategory modeli
         * kullanılmıştır. 
         * */
        private IUnitOfWork uow;
        public CategoryMenu(IUnitOfWork _unitOfWork)
        {
            uow = _unitOfWork;
        }
        public IViewComponentResult Invoke()
        {
            // vote'a göre ilk 3 bloğu alır. 
            var categories = uow.Categories.GetAll();
            return View(categories);
        }
    }
}
