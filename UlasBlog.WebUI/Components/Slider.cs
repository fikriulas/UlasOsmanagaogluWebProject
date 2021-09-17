﻿using Microsoft.AspNetCore.Mvc;
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
    public class Slider : ViewComponent
    {        
        private IUnitOfWork uow;
        public Slider(IUnitOfWork _unitOfWork)
        {
            uow = _unitOfWork;
        }
        public IViewComponentResult Invoke()
        {
            var setting= uow.Settings.GetAll()
                .Where(i => i.Id == 5)
                .Select(i => new Settings()
                {
                    Slider = i.Slider
                }).FirstOrDefault();
            bool slidersettings = setting.Slider;
            var blogs = uow.Blogs.GetAll()
                .Where(i => i.IsSlider)
                .Where(i => i.IsAppproved)                
                .Include(i => i.Comments)
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.Category)
                .Select(i => new BlogDetail()
                {
                    SliderSettings = slidersettings,
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    DateAdded = i.DateAdded,
                    Vote = i.Vote,
                    SlugUrl = i.SlugUrl,
                    ImageUrl = i.ImageUrl,
                    AuthorId = i.AuthorId,                    
                    totalComment = i.Comments.Count(),
                    Categories = i.BlogCategories.Select(b => b.Category).ToList()
                }).ToList();
            return View(blogs);
        }
    }
}
