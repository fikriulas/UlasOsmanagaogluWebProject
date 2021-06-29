using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;

namespace UlasBlog.WebUI.Controllers
{    
    public class AdminController : Controller
    {
        private IUnitOfWork uow;
        public AdminController(IUnitOfWork _uow)
        {
            uow = _uow;
        }
        [Route("/Admin")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Contact()
        {
            try
            {
                var contact = uow.Contacts.GetAll();
                return View(contact);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return View("_404NotFound");
            }            
        }
        public IActionResult ContactDelete(int id)
        {
            var contact = uow.Contacts.Get(id);
            if(contact !=null)
            {
                try
                {
                    uow.Contacts.Delete(contact);
                    uow.SaveChanges();
                    return Ok(id);
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    return BadRequest("İşlem Başarısız, Yöneticiye başvurun.");
                }
            }
            else
            {
                return BadRequest("İlgili obje bulunamadı.");
            }
            
        }


    }
}
