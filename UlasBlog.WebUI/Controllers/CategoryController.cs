using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
            ViewBag.SuccessSave = TempData["EditCategory"] ?? null; // Edit post methodundan geliyor.
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
                try
                {
                    category.SlugUrl = SeoUrl.AdresDuzenle(category.Name);
                    uow.Categories.Add(category);
                    uow.SaveChanges();
                    return Ok(category); // success çalıştır. 
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    //log tutulacak.
                    return BadRequest(error);
                }

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
                try
                {
                    uow.Categories.Edit(category);
                    uow.SaveChanges();
                    TempData["EditCategory"] = "Güncelleme Başarılı";
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    TempData["EditCategory"] = "Güncelleme Başarısız, Yönetici İle İletişime Geçin";
                    return RedirectToAction("Index");
                }                              
            }
            return RedirectToAction("Index");
        }
    }
}
