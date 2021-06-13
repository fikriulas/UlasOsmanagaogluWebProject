using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;
using UlasBlog.Entity;
using UlasBlog.WebUI.Models;

namespace UlasBlog.WebUI.Controllers
{
    public class BlogController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IUnitOfWork uow;
        public BlogController(IUnitOfWork _uow, IHostingEnvironment hostingEnvironment)
        {
            uow = _uow;
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("/Admin/Blog/")]
        public IActionResult Index()
        {
            var blogs = uow.Blogs.GetAll();
            var Categories = new List<SelectListItem>();
            foreach (var item in uow.Categories.GetAll())
            {
                Categories.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Categories = Categories;
            return View(blogs);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Blog blog, string[] categories, IFormFile ImageUrl)
        {
            blog.SlugUrl = SeoUrl.AdresDuzenle(blog.Title);
            if (ModelState.IsValid)
            {
                try
                {
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
                    var error = ex.Message;
                    //log tutulacak.
                    return BadRequest(error);
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var blog = uow.Blogs.GetAll()
                .Include(i => i.Comments)
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.Category)
                .Where(i => i.Id == Id)
                .Select(i => new BlogDetail()
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
                        Id = c.Id
                    }).ToList(),
                }).FirstOrDefault();
            var Categories = new List<SelectListItem>();
            List<string> catName = new List<string> { };
            List<string> catId = new List<string> { };

            foreach (var item in uow.Categories.GetAll())
            {
                Categories.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
                catId.Add(item.Id.ToString());
                catName.Add(item.Name);
            }
            ViewBag.Categories = Categories;            
            ViewBag.catId = catId.ToArray();

            if (blog == null)
            {
                RedirectToAction("404");// blog bulunamadı.
            }
            return View(blog);
            
        }
        public async Task<IActionResult> Edit(Blog blog, string[] categories, IFormFile ImageUrl)
        {

            blog.SlugUrl = SeoUrl.AdresDuzenle(blog.Title);
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
                    var entity = new Blog();
                    entity = blog;
                    entity = uow.Blogs.GetAll()
                     .Include(i => i.BlogCategories)
                     .ThenInclude(i => i.Category)
                     .FirstOrDefault(i => i.Id == blog.Id);
                    if (categories.Length != 0)
                    {
                        blog.BlogCategories.Clear();
                        for (int i = 0; i < categories.Length; i++)
                        {
                            blog.BlogCategories.Add(new BlogCategory()
                            {
                                BlogId = entity.Id,
                                CategoryId = Convert.ToInt32(categories[i]),
                            });
                        }
                    }
                    uow.Blogs.Edit(entity);
                    uow.SaveChanges();
                    return Ok();
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    //log tutulacak.
                    return BadRequest(error);
                }
            }
            return BadRequest();

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
                    //log tutulacak.
                    return BadRequest("Ekleme başarısız, Bir Sorunla Karşılaşıldı. Yöneticiyle İletişime Geçin");
                }
            }
            return BadRequest("İşlem Başarısız, Silmek İstediğiniz Blog Bulunamadı");
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
                //log tutulacak.
                return false;
            }
            return true;
        }

    }

}



