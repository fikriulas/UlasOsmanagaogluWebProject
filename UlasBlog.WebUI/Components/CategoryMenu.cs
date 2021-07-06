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
        private IUnitOfWork uow;
        public CategoryMenu(IUnitOfWork _unitOfWork)
        {
            uow = _unitOfWork;
        }
        public IViewComponentResult Invoke()
        {
            var categories = uow.Categories.GetAll();
            return View(categories);
        }
    }
}
