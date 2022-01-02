using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;
using UlasBlog.Entity;
using UlasBlog.WebUI.Models;

namespace UlasBlog.WebUI.Components
{
    public class FooterText : ViewComponent
    {
        private IUnitOfWork uow;
        public FooterText(IUnitOfWork _unitOfWork)
        {
            uow = _unitOfWork;
        }
        public IViewComponentResult Invoke()
        {
            var footer = uow.Settings.GetAll()
                .Where(i => i.Id == 1)
                .Select(i => new Settings()
                {
                    FooterText = i.FooterText
                }).FirstOrDefault();
            return View(footer);
        }
    }
}
