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
    public class HeaderSocialMedia : ViewComponent
    {
        private IUnitOfWork uow;
        public HeaderSocialMedia(IUnitOfWork _unitOfWork)
        {
            uow = _unitOfWork;
        }
        public IViewComponentResult Invoke()
        {
            var socialMedias = uow.Settings.GetAll()
                .Where(i => i.Id == 1)
                .Select(i => new Settings()
                {
                    Github = i.Github,
                    Facebook = i.Facebook,
                    Instagram = i.Instagram,
                    Linkedin = i.Linkedin,
                    Twitter = i.Twitter,
                    Youtube = i.Youtube
                }).FirstOrDefault();
            return View(socialMedias);
        }
    }
}
