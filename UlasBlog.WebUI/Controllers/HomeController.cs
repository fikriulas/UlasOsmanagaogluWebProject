using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;
using UlasBlog.Entity;
using UlasBlog.WebUI.Models;
using X.PagedList;

namespace UlasBlog.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private IUnitOfWork uow;
        public HomeController(IUnitOfWork _uow)
        {
            uow = _uow;
        }
        [Route("/{page?}")]
        public IActionResult Index(int page = 1)
        {
            var blogs = uow.Blogs.GetAll()
                .Include(i => i.Comments)
                .Select(i => new BlogDetail()
                {
                    Id = i.Id,
                    SlugUrl = i.SlugUrl,
                    Title = i.Title,
                    ImageUrl = i.ImageUrl,
                    DateAdded = i.DateAdded,
                    totalComment = i.Comments.Count
                }).AsQueryable().ToPagedList(page, 5);
            if (blogs != null)
            {
                return View(blogs);
            }
            return View(); // error page;            
        }
        [Route("Blog/{SlugUrl}")]
        public IActionResult Blog(string SlugUrl)
        {
            var blog = uow.Blogs.GetAll()
                .Include(i => i.Comments)
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.Category)
                .Where(i => i.SlugUrl == SlugUrl)
                .Where(i => i.IsAppproved)
                .Select(i => new BlogDetail()
                {
                    Id = i.Id,
                    SlugUrl = i.SlugUrl,
                    Title = i.Title,
                    Description = i.Description,
                    HtmlContent = i.HtmlContent,
                    DateAdded = i.DateAdded,
                    AuthorId = i.AuthorId,
                    Vote = i.Vote,
                    ViewCount = i.ViewCount,
                    ImageUrl = i.ImageUrl,
                    Comments = i.Comments.Select(b => new Comment()
                    {
                        Name = b.Name,
                        Email = b.Email,
                        Message = b.Message,
                        Id = b.Id,
                        dateAdded = b.dateAdded
                    }).ToList(),
                    totalComment = i.Comments.Count(),
                    Categories = i.BlogCategories.Select(c => c.Category).ToList()
                }).FirstOrDefault();

            var blogview = uow.Blogs.GetAll()
               .Where(i => i.SlugUrl == SlugUrl)
               .FirstOrDefault();

            blogview.ViewCount += 1;
            uow.Blogs.Edit(blogview);
            uow.SaveChanges();


            return View(blog);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.dateAdded = DateTime.Now;
                uow.Comments.Add(comment);
                uow.SaveChanges();
                return Ok(comment); // success çalıştır.                
            }
            return BadRequest();
        }
        [Route("/{SlugUrl}/{page}")]
        public IActionResult Blogs(string SlugUrl,int Id, int page = 1)
        {
            var blogs = uow.Blogs.GetAll()
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.Category)
                .Where(i => i.IsAppproved)
                .Select(i => new BlogDetail()
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    DateAdded = i.DateAdded,
                    AuthorId = i.AuthorId,
                    SlugUrl = i.SlugUrl,
                    Vote = i.Vote,
                    ImageUrl = i.ImageUrl,
                    Comments = i.Comments.Select(b => new Comment()
                    {
                        Id = b.Id
                    }).ToList(),
                    totalComment = i.Comments.Count(),
                    Categories = i.BlogCategories.Select(c => c.Category).ToList()
                });

            var yeni = blogs
                .Where(i => i.Categories.Any(b => b.SlugUrl == SlugUrl)).AsQueryable().ToPagedList(page, 1);

            return View(yeni);
        }



    }
}
