using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;
using UlasBlog.Entity;
using UlasBlog.WebUI.IdentityCore;
using UlasBlog.WebUI.Models;

namespace UlasBlog.WebUI.Controllers
{
    [Authorize(Roles = "admin,editör")]
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private IUnitOfWork uow;
        private UserManager<AppUser> userManager;
        public BlogController(IUnitOfWork _uow, IHostingEnvironment hostingEnvironment, UserManager<AppUser> _userManager, ILogger<BlogController> _logger)
        {
            uow = _uow;
            logger = _logger;
            _hostingEnvironment = hostingEnvironment;
            userManager = _userManager;
        }

        [Route("/Admin/Blog/")]
        public IActionResult Index()
        {
            try
            {
                var blogs = uow.Blogs.GetAll()
                .Select(i => new BlogDetail()
                {
                    Id = i.Id,
                    Title = i.Title,
                    DateAdded = i.DateAdded,
                    //AuthorId = userManager.FindByNameAsync(i.AuthorId).Result.Name
                    AuthorId = i.AuthorId
                });

                var Categories = new List<SelectListItem>();
                foreach (var item in uow.Categories.GetAll())
                {
                    Categories.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.BlogAlert = TempData["BlogAlert"] ?? null; // Edit post methodundan geliyor.
                ViewBag.Categories = Categories;
                return View(blogs);
            }
            catch (Exception ex)
            {                
                logger.LogError(2, ex, "Controller Name: BlogController, Action: Index");
                return View("_404NotFound");
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> Add(Blog blog, string[] categories, IFormFile ImageUrl)
        {
            if (blog.Title != null)
                blog.SlugUrl = SeoUrl.AdresDuzenle(blog.Title);
            if (ModelState.IsValid)
            {
                try
                {
                    var user = userManager.FindByNameAsync(User.Identity.Name).Result;
                    var path = "";
                    if (ImageUrl != null)
                    {
                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Home\\Blog\\Img\\", ImageUrl.FileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await ImageUrl.CopyToAsync(stream);
                            blog.ImageUrl = ImageUrl.FileName;
                        }
                    }
                    blog.DateAdded = DateTime.Now;
                    blog.ViewCount = 0;
                    blog.AuthorId = user.Id;
                    uow.Blogs.Add(blog);
                    uow.SaveChanges();
                    List<BlogCategory> blogCategories = new List<BlogCategory>();
                    for (int i = 0; i < categories.Count(); i++)
                    {
                        var category = new BlogCategory()
                        {
                            Blog = blog,
                            CategoryId = Convert.ToInt32(categories[i]),
                        };
                        blogCategories.Add(category);
                    }
                    uow.BlogCategory.AddRange(blogCategories);
                    uow.SaveChanges();
                    return Ok(blog);
                }
                catch (Exception ex)
                {                    
                    logger.LogError(2, ex, "Controller Name: BlogController, Action: Add, Blog Title: {blog.Title}", blog.Title);
                    return BadRequest(ex.Message);
                }
            }
            logger.LogError(1, "ModelState Invalid,Controller Name: BlogController, Action: Add, Blog Title: {blog.Title}", blog.Title);
            return BadRequest();
        }
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            try
            {
                var blog = uow.Blogs.GetAll()
                .Include(i => i.Comments)
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.Category)
                .Where(i => i.Id == Id)
                .Select(i => new BlogEdit()
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    AuthorId = i.AuthorId,
                    ImageUrl = i.ImageUrl,
                    IsHome = i.IsHome,
                    IsAppproved = i.IsAppproved,
                    IsSlider = i.IsSlider,
                    HtmlContent = i.HtmlContent,
                    Categories = i.BlogCategories.Select(b => new Category()
                    {
                        Id = b.Category.Id,
                        Name = b.Category.Name,
                    }).ToList(),
                    Comments = i.Comments.Select(c => new Comment()
                    {
                        Name = c.Name,
                        Email = c.Email,
                        dateAdded = c.dateAdded,
                        Message = c.Message,
                        Id = c.Id
                    }).ToList(),
                }).FirstOrDefault();
                var Categories = new List<SelectListItem>();
                List<string> catId = new List<string> { };

                foreach (var item in uow.Categories.GetAll())
                {
                    Categories.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });

                }
                foreach (var category in blog.Categories)
                {
                    catId.Add(category.Id.ToString());
                }
                ViewBag.Categories = Categories;
                ViewBag.catId = catId.ToArray();
                return View(blog);
            }
            catch (Exception ex)
            {
                logger.LogError(2, ex, "Controller Name: BlogController, Action: Get Edit, Blog Id: {Id}", Id);
                TempData["BlogAlert"] = "toastr.error('Bir Sorun Oluştu');" + "toastr.error('Yönetici İle İletişime Geçin');";
                return RedirectToAction("Index");
            }
            

        }
        [HttpPost]
        public async Task<IActionResult> Edit(BlogEdit blog, string[] categories, IFormFile ImageUrl)
        {
            try
            {
                if (blog.Title != null)
                    blog.SlugUrl = SeoUrl.AdresDuzenle(blog.Title);
                ////
                var entity = new BlogEdit();
                entity = uow.Blogs.GetAll()
                 .Include(i => i.Comments)
                 .Include(i => i.BlogCategories)
                 .ThenInclude(i => i.Category)
                .Where(i => i.Id == blog.Id)
                .Select(i => new BlogEdit()
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    AuthorId = i.AuthorId,
                    ImageUrl = i.ImageUrl,
                    IsHome = i.IsHome,
                    IsAppproved = i.IsAppproved,
                    IsSlider = i.IsSlider,
                    HtmlContent = i.HtmlContent,
                    Categories = i.BlogCategories.Select(b => new Category()
                    {
                        Id = b.Category.Id,
                        Name = b.Category.Name,
                    }).ToList(),
                    Comments = i.Comments.Select(c => new Comment()
                    {
                        Name = c.Name,
                        Email = c.Email,
                        dateAdded = c.dateAdded,
                        Message = c.Message,
                        Id = c.Id
                    }).ToList(),
                }).FirstOrDefault();
                var Categories = new List<SelectListItem>();
                List<string> catId = new List<string> { };

                foreach (var item in uow.Categories.GetAll())
                {
                    Categories.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });

                }
                foreach (var category in entity.Categories)
                {
                    catId.Add(category.Id.ToString());
                }
                ViewBag.Categories = Categories;
                ViewBag.catId = catId.ToArray();
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                ViewBag.alertMessage = error;
                logger.LogError(2, ex, "Controller Name: BlogController, Action: Edit, Blog Title: {blog.Title}", blog.Title);
                return View(blog);
            }
            
            ////
            if (ModelState.IsValid)
            {
                try
                {
                    var path = "";
                    if (ImageUrl != null)
                    {
                        var ExistingBlog = uow.Blogs.Get(blog.Id);// Editlemeden önceki blog'u getir.
                        DeleteImage(ExistingBlog.ImageUrl); // Editlemeden önceki blog'un image'inin sil.
                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Home\\Blog\\Img\\", ImageUrl.FileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await ImageUrl.CopyToAsync(stream);
                            blog.ImageUrl = ImageUrl.FileName;
                        }
                    }
                    var blogEdit = new Blog();
                    blogEdit = uow.Blogs.GetAll()
                     .Include(i => i.BlogCategories)
                     .ThenInclude(i => i.Category)
                     .FirstOrDefault(i => i.Id == blog.Id);
                    if (categories.Length != 0)
                    {
                        blogEdit.BlogCategories.Clear();
                        for (int i = 0; i < categories.Length; i++)
                        {
                            blogEdit.BlogCategories.Add(new BlogCategory()
                            {
                                BlogId = blog.Id,
                                CategoryId = Convert.ToInt32(categories[i]),
                            });
                        }
                    }
                    blogEdit.Title = blog.Title;
                    blogEdit.Description = blog.Description;
                    blogEdit.HtmlContent = blog.HtmlContent;
                    blogEdit.IsAppproved = blog.IsAppproved;
                    blogEdit.IsHome = blog.IsHome;
                    blogEdit.IsSlider = blog.IsSlider;
                    if (blog.ImageUrl != null)
                        blogEdit.ImageUrl = blog.ImageUrl;
                    uow.Blogs.Edit(blogEdit);
                    uow.SaveChanges();
                    TempData["BlogAlert"] = "toastr.success('Güncelleme İşlemi Başarılı');";
                    //ViewBag.alertMessage = "Değer Bulanamadı";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    ViewBag.alertMessage = error;
                    //log tutulacak.
                    return View(blog);
                }
            }
            else
            {
                ViewBag.alertMessage = "toastr.error('Kontrol Edip Tekrar Deneyin');";
                return View(entity);
            }
        }
        public IActionResult Delete(int id)
        {
            var blog = uow.Blogs.Get(id);
            if (blog != null)
            {
                try
                {
                    if (blog.ImageUrl != null)
                    {
                        DeleteImage(blog.ImageUrl);
                    }
                    uow.Blogs.Delete(blog);
                    uow.SaveChanges();
                    return Ok(id);
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    logger.LogError(2, ex, "Controller Name: Blog, Action: Delete, Blog Id: {id}", id);
                    return BadRequest("Ekleme başarısız, Bir Sorunla Karşılaşıldı. Yöneticiyle İletişime Geçin");
                }
            }
            return BadRequest("İşlem Başarısız, Silmek İstediğiniz Blog Bulunamadı");
        }
        public IActionResult CommentDelete(Comment comment)
        {
            var Deletedcomment = uow.Comments.Get(comment.Id);
            if (Deletedcomment != null)
            {
                try
                {
                    uow.Comments.Delete(Deletedcomment);
                    uow.SaveChanges();
                    return Ok(comment.Id);
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    logger.LogError(2, ex, "Controller Name: Blog, Action: CommentDelete, Blog Id: {id}", comment.BlogId);
                    return BadRequest("Ekleme başarısız, Bir Sorunla Karşılaşıldı. Yöneticiyle İletişime Geçin");
                }
            }
            return BadRequest("İşlem Başarısız, Silmek İstediğiniz Yorum Bulunamadı");
        }

        private bool DeleteImage(string url)
        {
            try
            {
                var deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Home\\Blog\\Img\\", url);
                System.IO.File.Delete(deletePath);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                logger.LogError(2, ex, "Controller Name: Blog, Action: DeleteImage");
                return false;
            }
            return true;
        }

        public IActionResult SaveHtmlContent(string content, int Id)
        {

            try
            {
                var blog = uow.Blogs.Get(Id);
                blog.HtmlContent = content;
                uow.Blogs.Edit(blog);
                uow.SaveChanges();
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                logger.LogError(2, ex, "Controller Name: Blog, Action: SaveHtmlContent, Blog Id: {id}", Id);
                return BadRequest(error);
            }
            return Ok("Başarıyla kaydedildi");
        }
    }

}




