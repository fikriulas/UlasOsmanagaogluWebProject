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
using System.IO;
using Microsoft.Extensions.Logging;

namespace UlasBlog.WebUI.Controllers
{
    [Authorize(Roles = "admin,editör")]
    public class AdminController : Controller
    {
        private IUnitOfWork uow;
        private readonly ILogger<AdminController> logger;
        public AdminController(IUnitOfWork _uow, ILogger<AdminController> _logger)
        {
            uow = _uow;
            logger = _logger;
        }
        [Route("/Admin")]
        public IActionResult Index()
        {
            try
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
            catch (Exception ex)
            {
                logger.LogError(2, ex, "Controller Name: AdminController, Action: Index");
                return NotFound();
            }
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
                logger.LogError(2, ex, "Controller Name: AdminController, Action: Contact");
                return NotFound();
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

                MailboxAddress to = new MailboxAddress(contact.Name, contact.Email);
                message.To.Add(to);

                message.Subject = contact.dateAdded.ToString("MM/dd/yyyy MM:ss") + " Tarihli Mesajınız Hakkında";
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
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(2, ex, "Controller Name: AdminController, Action: ReplyMessage");
                return BadRequest();
            }
        }
        [Authorize(Roles = "admin")]
        public IActionResult ContactDelete(int id)
        {
            var contact = uow.Contacts.Get(id);
            if (contact != null)
            {
                try
                {
                    uow.Contacts.Delete(contact);
                    uow.SaveChanges();
                    return Ok(id);
                }
                catch (Exception ex)
                {
                    logger.LogError(2, ex, "Controller Name: AdminController, Action: ContactDelete");
                    return BadRequest("İşlem Başarısız, Yöneticiye ulaşın.");
                }
            }
            else
            {
                logger.LogError(1, "Controller Name: AdminController, Action: ContactDelete --> ilgili contact verisi bulunamadı. ContactId: {id}", id);
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
                {
                    string logfilePath = @"log.txt";
                    string logfileExt = Path.GetExtension(logfilePath);
                    FileInfo FileSizeInfo = new FileInfo(logfilePath);
                    string logFileSize = "";
                    if (FileSizeInfo.Length / 1024 <= 0)
                    {
                        logFileSize = FileSizeInfo.Length.ToString();
                        logFileSize = logFileSize + " Byte";
                    }
                    else if ((FileSizeInfo.Length / 1024) / 1024 <= 0)
                    {
                        logFileSize = (FileSizeInfo.Length / 1024).ToString();
                        logFileSize = logFileSize + " KB";
                    }
                    else
                    {
                        logFileSize = ((FileSizeInfo.Length / 1024) / 1024).ToString();
                        logFileSize = logFileSize + " MB";
                    }
                    string[] fileInfo = { "Log.txt", logfileExt, logFileSize };
                    ViewBag.fileinfo = fileInfo;
                    ViewBag.SettingsAlert = TempData["SettingsAlert"] ?? null; // DownloadLogFile Methodundan geliyor.
                    return View(settings);
                }
                else
                {
                    logger.LogError(1, "Controller Name: AdminController, Action: Settings --> ilgili settings verisi bulunamadı.");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(2, ex, "Controller Name: AdminController, Action: Settings");
                return NotFound();
            }
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Settings(Settings settings, string fileDeleteInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //delete log file
                    if (fileDeleteInfo == "Evet")
                    {
                        System.IO.File.WriteAllText(@"log.txt", string.Empty);
                    }
                    else
                    {
                        
                    }
                    settings.UpdateDate = DateTime.Now;
                    uow.Settings.Edit(settings);
                    uow.SaveChanges();
                    return Ok();
                }
                catch (Exception ex)
                {
                    logger.LogError(2, ex, "Controller Name: AdminController, Action: Settings");
                    return BadRequest();
                }
            }
            logger.LogError(1, "ModelState Invalid,Controller Name: AdminController, Action: Settings");
            return BadRequest();
        }
        [Authorize(Roles = "admin")]
        public IActionResult DownloadLogFile()
        {
            string logFilePath = @"log.txt";
            string logFileName = "log.txt";
            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(logFilePath);
                return File(fileBytes, "application/force-download", logFileName);
            }
            catch (Exception ex)
            {
                logger.LogError(2, ex, "Controller Name: Admin, Action: DownloadLogFile");
                TempData["SettingsAlert"] = "toastr.error('Bir Sorun Oluştu');" + "toastr.error('Yönetici İle İletişime Geçin');";
                return RedirectToAction("Settings");
            }

        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        public IActionResult IpList()
        {
            try
            {
                var ips = uow.Iplist.GetAll();
                return View(ips);
            }
            catch (Exception ex)
            {
                logger.LogError(2, ex, "Controller Name: AdminController, Action: IpList");
                return NotFound();
            }

        }
        [Authorize(Roles = "admin")]
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
                    logger.LogError(2, ex, "Controller Name: AdminController, Action: AddIpList");
                    return BadRequest(error);
                }
            }
            logger.LogError(1, "ModelState Invalid,Controller Name: AdminController, Action: AddIpList");
            return BadRequest();
        }
        [Authorize(Roles = "admin")]
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
                logger.LogError(2, ex, "Controller Name: AdminController, Action: DeleteIpList");
                return BadRequest(error.ToString());
            }
        }
        [Authorize(Roles = "admin")]
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
                    logger.LogError(2, ex, "Controller Name: AdminController, Action: EditIpList");
                    return BadRequest(error.ToString());
                }
            }
            logger.LogError(1, "ModelState Invalid,Controller Name: AdminController, Action: EditIpList");
            return BadRequest("Hata");
        }
        [Authorize(Roles = "admin")]
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
                    logger.LogError(2, ex, "Controller Name: AdminController, Action: IpListChangeStatus");
                    return BadRequest(error.ToString());
                }
            }
            return BadRequest("Bir hata Oluştu");
        }
    }
}
