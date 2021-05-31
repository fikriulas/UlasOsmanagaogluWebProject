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

namespace UlasBlog.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private IUnitOfWork uow;
        public HomeController(IUnitOfWork _uow)
        {
            uow = _uow;
        }

        public IActionResult Index()
        {
            var blogs = uow.Blogs.GetAll()
                .Include(i => i.Comments)
                .Select(i => new BlogDetail()
                {
                    Id = i.Id,
                    Title = i.Title,
                    ImageUrl = i.ImageUrl,
                    DateAdded = i.DateAdded,
                    totalComment = i.Comments.Count
                }).AsQueryable();
            if (blogs != null)
            {
                return View(blogs);
            }
            return View(); // error page;            
        }
        public IActionResult Blog(int Id)
        {
            var blog = uow.Blogs.GetAll()
                .Include(i => i.Comments)
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.Category)
                .Where(i => i.Id == Id)
                .Where(i => i.IsAppproved)
                .Select(i => new BlogDetail()
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    HtmlContent = i.HtmlContent,
                    DateAdded = i.DateAdded,
                    AuthorId = i.AuthorId,
                    Vote = i.Vote,
                    ImageUrl = i.ImageUrl,
                    Comments = i.Comments.Select(b => new Comment()
                    {
                        Name = b.Name,
                        Email = b.Email,
                        Id = b.Id,
                        dateAdded = b.dateAdded
                    }).ToList(),
                    totalComment = i.Comments.Count(),
                    Categories = i.BlogCategories.Select(c => c.Category).ToList()
                }).FirstOrDefault();
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
        public IActionResult Blogs(int Id)
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
                    Vote = i.Vote,
                    ImageUrl = i.ImageUrl,
                    Comments = i.Comments.Select(b => new Comment()
                    {                        
                        Id = b.Id                        
                    }).ToList(),
                    totalComment = i.Comments.Count(),
                    Categories = i.BlogCategories.Where(d => d.CategoryId == Id).Select(c => c.Category).ToList()
                }).ToList();

            //blogs = blogs.Include(i => i.Categories)
            // .Where(i => i.Categories.Any(b => b.Id == Id));


            /*return View(
                    new BlogList()
                    {
                        Blogs = blogs
                    }                
                );
            */
            return View(blogs);
        }



    }
}
