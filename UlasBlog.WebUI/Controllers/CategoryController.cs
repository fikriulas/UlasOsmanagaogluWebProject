using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;
using UlasBlog.Entity;
using UlasBlog.WebUI.Models;

namespace UlasBlog.WebUI.Controllers
{
    public class CategoryController : Controller
    {
        private IUnitOfWork uow;
        public CategoryController(IUnitOfWork _uow)
        {
            uow = _uow;
        }
        [Route("/Admin/Category")]
        public IActionResult Index()
        {
            var categories = uow.Categories.GetAll();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Category category)
        {
            if (ModelState.IsValid)
            {
                category.SlugUrl = SeoUrl.AdresDuzenle(category.SlugUrl);
                uow.Categories.Add(category);
                uow.SaveChanges();
                return Ok(category); // success çalıştır.                
            }
            return BadRequest();
            //return RedirectToAction("Index", category);
            //return View(category);
        }
        public IActionResult Delete(int Id)
        {
            var category = uow.Categories.Get(Id);
            if (category != null)
            {
                uow.Categories.Delete(category);
                uow.SaveChanges();
                return Ok(Id);
            }
            return BadRequest();
        }
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                uow.Categories.Edit(category);
                uow.SaveChanges();                
            }
            return RedirectToAction("Index");
        }
    }
}
