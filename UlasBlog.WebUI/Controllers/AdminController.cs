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
using UlasBlog.WebUI.Models;

namespace UlasBlog.WebUI.Controllers
{
    [Authorize(Roles = "admin,editör")]
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
            int totalComment = uow.Comments.GetAll().Count();
            int totalBlog = uow.Blogs.GetAll().Count();
            int totalMessage = uow.Contacts.GetAll()
                .Where(i => i.IsRead == false)
                .Count();
            int totalBlockedIp = uow.Iplist.GetAll()
                .Where(i => i.Block)
                .Count();
            var dashboard = new Dashboard()
            {
                TotalComment = totalComment,
                TotalBlog = totalBlog,
                TotalMessage = totalMessage,
            };
            return View(dashboard);
        }
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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
        public IActionResult IpList()
        {
            var ips = uow.Iplist.GetAll();
            return View(ips);
        }
        public IActionResult AddIpList(Iplist iplist)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    iplist.Date = DateTime.Now;
                    iplist.Block = true;
                    uow.Iplist.Add(iplist);
                    uow.SaveChanges();
                    return Ok(iplist);
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    return BadRequest(error);
                }
            }
            return BadRequest();
        }
        public IActionResult DeleteIpList(int Id)
        {
            try
            {
                var ip = uow.Iplist.Get(Id);
                if (ip != null)
                {
                    uow.Iplist.Delete(ip);
                    uow.SaveChanges();
                    return Ok(Id);
                }
                return BadRequest("Ip listesi listede buluanamadı");
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return BadRequest(error.ToString());
            }
        }
        public IActionResult EditIpList(Iplist iplist)
        {
            if (ModelState.IsValid)
            {
                var ips = uow.Iplist.Get(iplist.Id);
                ips.Ip = iplist.Ip;
                ips.Note = iplist.Note;
                try
                {
                    uow.Iplist.Edit(ips);
                    uow.SaveChanges();
                    return Ok(ips);
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    return BadRequest(error.ToString());
                }
            }
            return BadRequest("Hata");
        }
        public IActionResult IpListChangeStatus(int Id)
        {
            var ips = uow.Iplist.Get(Id);
            if (ips != null)
            {
                if (ips.Block)
                    ips.Block = false;
                else
                    ips.Block = true;
                try
                {
                    uow.Iplist.Edit(ips);
                    uow.SaveChanges();
                    return Ok(ips);
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    return BadRequest(error.ToString());                    
                }
            }
            return BadRequest("Bir hata Oluştu");          
            
            
        }




    }
}
