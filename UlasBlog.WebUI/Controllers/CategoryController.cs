using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;
using UlasBlog.Entity;
using UlasBlog.WebUI.Models;

namespace UlasBlog.WebUI.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> logger;
        private IUnitOfWork uow;
        public CategoryController(IUnitOfWork _uow, ILogger<CategoryController> _logger)
        {
            uow = _uow;
            logger = _logger;
        }
        [Route("/Admin/Category")]
        public IActionResult Index()
        {
            try
            {
                var categories = uow.Categories.GetAll();
                ViewBag.SuccessSave = TempData["EditCategory"] ?? null; // Edit post methodundan geliyor.
                return View(categories);
            }
            catch (Exception ex)
            {
                logger.LogError(2, ex, "Controller Name: Category, Action: Index");
                return View("_404NotFound");
            }            
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
                    logger.LogError(2, ex, "Controller Name: Category, Action: Add, CategoryName:{category.Name}",category.Name);
                    return BadRequest(ex.Message);
                }

            }
            logger.LogError(1,"ModelState Invalid,Controller Name: Category, Action: Add, CategoryName:{category.Name}", category.Name);
            return BadRequest();
        }
        public IActionResult Delete(int Id)
        {
            try
            {
                var category = uow.Categories.Get(Id);
                uow.Categories.Delete(category);
                uow.SaveChanges();
                return Ok(Id);

            }
            catch (Exception ex)
            {
                logger.LogError(2, ex, "İlgili kategori bulunamadı.(Silinmek istenen kategori Id: {Id}), Controller Name: Cateogry, Action: Delete", Id);
                return BadRequest();                
            }


            
        }
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    uow.Categories.Edit(category);
                    uow.SaveChanges();
                    TempData["EditCategory"] = "toastr.success('İşlem Başarılı');";
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    //TempData["EditCategory"] = "Güncelleme Başarısız, Yönetici İle İletişime Geçin";
                    TempData["EditCategory"] = "toastr.error('İşlem Başarısız');";
                    logger.LogError(2, ex, "Controller Name: Category, Action: Edit, CategoryName:{category.Id}, CategoryId: {category.Name}", category.Name, category.Id);
                    return RedirectToAction("Index");
                }
            }
            logger.LogError(1, "ModelState Invalid,Controller Name: Category, Action: Edit, CategoryName:{category.Name}, CategoryId: {category.Id}", category.Name, category.Id);
            return RedirectToAction("Index");
        }
    }
}
