using Microsoft.AspNetCore.Identity;
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
using UlasBlog.WebUI.IdentityCore;
using UlasBlog.WebUI.Models;
using X.PagedList;

namespace UlasBlog.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private IUnitOfWork uow;
        private SignInManager<AppUser> signInManager;
        private UserManager<AppUser> userManager;
        public HomeController(IUnitOfWork _uow, SignInManager<AppUser> _signInManager, UserManager<AppUser> _userManager)
        {
            uow = _uow;
            signInManager = _signInManager;
            userManager = _userManager;
        }
        [Route("/{page?}")]
        public IActionResult Index(int page = 1)
        {
            var blogs = uow.Blogs.GetAll()
                .Where(i => i.IsAppproved)
                .Where(i => i.IsHome)
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
            if (blog != null)
            {
                var blogview = uow.Blogs.GetAll()
               .Where(i => i.SlugUrl == SlugUrl)
               .FirstOrDefault();

                blogview.ViewCount += 1;
                uow.Blogs.Edit(blogview);
                uow.SaveChanges();
                return View(blog);
            }
            else
            {
                return View("_404NotFound");
            }
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
        public IActionResult Blogs(string SlugUrl, int Id, int page = 1)
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
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Contact(Contact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    contact.dateAdded = DateTime.Now;
                    uow.Contacts.Add(contact);
                    uow.SaveChanges();
                    return Ok(); // success çalıştır.  
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    //log tutulacak.
                    return BadRequest("Bir Hata Oluştu.");
                }

            }
            var validError = "Kontrol edip, tekrar deneyin";
            return BadRequest(validError);

        }
        public IActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl; // actionlar arasında veri taşıyabilir.
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByEmailAsync(login.Email); // gelen email ile kullanıcı var mı kontrol edilir.
                if (user != null)
                {
                    await signInManager.SignOutAsync(); // login işleminden önce sistemde siteyle ilgili bir cookie olma durumuna karşın logout yapılır.
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (TempData["returnUrl"] != null)
                        {
                            return Redirect(TempData["returnUrl"].ToString());
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Hatali E-Mail Yada Şifre Girdiniz");
                }
            }
            return View(login);
        }



    }
}
