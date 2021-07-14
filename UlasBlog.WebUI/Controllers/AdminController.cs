using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;
using UlasBlog.Entity;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Authorization;

namespace UlasBlog.WebUI.Controllers
{    
    [Authorize]
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
        [HttpPost]
        public IActionResult ReplyMessage(Contact contact, string reply)
        {
            if (reply == null)
                return BadRequest(": Yanıt Alanı Boş Bırakılamaz.");
            try
            {
                var settings = uow.Settings.Get(5);
                MimeMessage message = new MimeMessage();
                MailboxAddress from = new MailboxAddress(settings.MailUserName, settings.MailUserName);
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress(contact.Name,contact.Email);
                message.To.Add(to);

                message.Subject = contact.dateAdded.ToString("MM/dd/yyyy MM:ss") + " Tarihli Mesajınız Hakkında" ;
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = reply.ToString();
                message.Body = bodyBuilder.ToMessageBody();
                SmtpClient client = new SmtpClient();
                client.Connect(settings.SmtpAddress, int.Parse(settings.Port), true);
                client.Authenticate(settings.MailUserName, settings.MailPassword);
                //client.Send(message);
                //client.Disconnect(true);
                //client.Dispose();
                contact.IsRead = true;
                uow.Contacts.Edit(contact);
                uow.SaveChanges();
                
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return BadRequest();
            }
            return Ok();            
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
        public IActionResult Settings()
        {
            try
            {
                var settings = uow.Settings.Get(5);
                if (settings != null)
                    return View(settings);
                else
                    return View("_404NotFound");

            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return View("_404NotFound");
            }            
        }
        [HttpPost]
        public IActionResult Settings(Settings settings)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    settings.UpdateDate = DateTime.Now;
                    uow.Settings.Edit(settings);
                    uow.SaveChanges();
                    return Ok(); 
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    return BadRequest(error);
                }
            }
            return BadRequest();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }


    }
}
