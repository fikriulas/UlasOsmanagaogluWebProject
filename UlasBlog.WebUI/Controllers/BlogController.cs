using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                            blog.DateAdded = DateTime.Now;
                        }
                    }
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
        public IActionResult Delete(int id)
        {
            var blog = uow.Blogs.Get(id);
            if (blog != null)
            {
                try
                {
                    if (blog.ImageUrl != null)
                    {
                        var deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Home\\Blog\\Img\\", blog.ImageUrl);
                        System.IO.File.Delete(deletePath);
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
        [Route("/Blog/test/{testUrl}")]
        public IActionResult SeoTest(string testUrl)
        {          
            return Content("SeoTest: " + SeoUrl.AdresDuzenle(testUrl));
        }
        public string toSeo(string s)
        {            
            if (string.IsNullOrEmpty(s)) //string yok veya boş ise true döndürür
            {
                return "";
            }

            if (s.Length > 80)
            {
                s = s.Substring(0, 80); //string den belli karakter alır.
            }
            s = s.Replace("ş", "s"); //karakter değişimi için kullanılır
            s = s.Replace("Ş", "S");
            s = s.Replace("ğ", "g");
            s = s.Replace("Ğ", "G");
            s = s.Replace("İ", "I");
            s = s.Replace("ı", "i");
            s = s.Replace("ç", "c");
            s = s.Replace("Ç", "C");
            s = s.Replace("ö", "o");
            s = s.Replace("Ö", "O");
            s = s.Replace("ü", "u");
            s = s.Replace("Ü", "U");
            s = s.Replace("'", "");
            s = s.Replace("\"", "");
            Regex r = new Regex("[^a-zA-Z0-9_-]");
            //if (r.IsMatch(s))
            s = r.Replace(s, "-");
            if (!string.IsNullOrEmpty(s))
                while (s.IndexOf("--") > -1)
                    s = s.Replace("--", "-");
            if (s.StartsWith("-")) s = s.Substring(1);
            if (s.EndsWith("-")) s = s.Substring(0, s.Length - 1);
            return s;
        }
    }

}



