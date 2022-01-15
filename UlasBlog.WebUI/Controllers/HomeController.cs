﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;
using UlasBlog.Entity;
using UlasBlog.WebUI.IdentityCore;
using UlasBlog.WebUI.Models;
using X.PagedList;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;

namespace UlasBlog.WebUI.Controllers
{
    public class HomeController : BaseController
    {
        IConfiguration _configuration;
        private readonly ILogger<CategoryController> logger;
        private IUnitOfWork uow;
        private SignInManager<AppUser> signInManager;
        private UserManager<AppUser> userManager;
        private RoleManager<AppRole> roleManager;
        public HomeController(IUnitOfWork _uow, SignInManager<AppUser> _signInManager, UserManager<AppUser> _userManager, RoleManager<AppRole> _roleManager, IConfiguration configuration, ILogger<CategoryController> _logger)
            : base(_userManager, _signInManager, _roleManager)
        {
            _configuration = configuration;
            uow = _uow;
            logger = _logger;
            signInManager = _signInManager;
            userManager = _userManager;
            roleManager = _roleManager;

        }
        [Route("/{page?}")]
        public IActionResult Index(int page = 1)
        {
            try
            {
                var blogs = uow.Blogs.GetAll()
                .Where(i => i.IsAppproved)
                .Where(i => i.IsHome)
                .Include(i => i.Comments)
                .Select(i => new BlogDetail()
                {
                    Id = i.Id,
                    SlugUrl = i.SlugUrl,
                    Title = i.Title,
                    ImageUrl = i.ImageUrl,
                    AuthorId = i.AuthorId,
                    DateAdded = i.DateAdded,
                    totalComment = i.Comments.Count,
                    ViewCount = i.ViewCount
                }).AsQueryable().ToPagedList(page, 10);
                if (blogs != null)
                {
                    return View(blogs);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                logger.LogError(2, ex, "Controller Name: Home, Action: Index");
                return NotFound();
            }
            
        }
        [Route("Blog/{SlugUrl}")]
        public IActionResult Blog(string SlugUrl)
        {
            try
            {
                var blog = uow.Blogs.GetAll()
                .Include(i => i.Comments)
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.Category)
                .Where(i => i.SlugUrl == SlugUrl)
                .Where(i => i.IsAppproved)
                .Select(i => new BlogDetail()
                {
                    Id = i.Id,
                    SlugUrl = i.SlugUrl,
                    Title = i.Title,
                    Description = i.Description,
                    HtmlContent = i.HtmlContent,
                    DateAdded = i.DateAdded,
                    AuthorId = i.AuthorId,
                    Vote = i.Vote,
                    ViewCount = i.ViewCount,
                    ImageUrl = i.ImageUrl,
                    Comments = i.Comments.Select(b => new Comment()
                    {
                        Name = b.Name,
                        Email = b.Email,
                        Message = b.Message,
                        Id = b.Id,
                        dateAdded = b.dateAdded
                    }).ToList(),
                    totalComment = i.Comments.Count(),
                    Categories = i.BlogCategories.Select(c => c.Category).ToList()
                }).FirstOrDefault();
                if (blog != null)
                {
                    var commentStatus = uow.Settings.GetAll()
                        .Where(i => i.Id == 1)
                        .FirstOrDefault();
                    string CommentOfStatus = commentStatus.Comment.ToString();
                    TempData["commentStatus"] = CommentOfStatus;                    
                    var blogview = uow.Blogs.GetAll()
                   .Where(i => i.SlugUrl == SlugUrl)
                   .FirstOrDefault();

                    blogview.ViewCount += 1;
                    uow.Blogs.Edit(blogview);
                    uow.SaveChanges();
                    return View(blog);
                }
                else
                {
                    return NotFound();                    
                }
            }
            catch (Exception ex)
            {
                logger.LogError(2, ex, "Controller Name: Home, Action: Blog");
                return NotFound();
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            try
            {
                var captchaImage = HttpContext.Request.Form["g-recaptcha-response"];
                if (string.IsNullOrEmpty(captchaImage))
                {
                    return BadRequest("Captcha Doğrulayıp Tekrar Deneyin");
                }
                var verified = await CheckCaptcha();
                if (!verified)
                {
                    return BadRequest("Captcha Doğrulaması Hatalı, Tekrar Deneyin");
                }
                if (ModelState.IsValid)
                {
                    string uzakIPadresi = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                    comment.IpAddress = uzakIPadresi;
                    comment.dateAdded = DateTime.Now;
                    uow.Comments.Add(comment);
                    uow.SaveChanges();
                    return Ok(comment); // success çalıştır.                
                }
                logger.LogError(1, "ModelState Invalid,Controller Name: Home, Action: AddComment");
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger.LogError(2, ex, "Controller Name: Home, Action: AddComment");
                return BadRequest();
            }            
        }
        [Route("Category/{SlugUrl}/{page}")]
        public IActionResult Blogs(string SlugUrl, int Id, int page = 1)
        {
            try
            {
                var categoryCheck = uow.Categories.GetAll()
                        .Where(i => i.SlugUrl == SlugUrl)
                        .Select(i => new Category()
                        {
                            SlugUrl = i.SlugUrl,
                        }).ToList();
                if (categoryCheck.Count == 0)
                    return NotFound();
                var blogs = uow.Blogs.GetAll()
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.Category)
                .Where(i => i.IsAppproved)
                .Select(i => new BlogDetail()
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    DateAdded = i.DateAdded,
                    AuthorId = i.AuthorId,
                    SlugUrl = i.SlugUrl,
                    Vote = i.Vote,
                    ImageUrl = i.ImageUrl,
                    ViewCount = i.ViewCount,
                    Comments = i.Comments.Select(b => new Comment()
                    {
                        Id = b.Id
                    }).ToList(),
                    totalComment = i.Comments.Count(),
                    Categories = i.BlogCategories.Select(c => c.Category).ToList()
                });
                var categoryOfBlogs = blogs
                    .Where(i => i.Categories.Any(b => b.SlugUrl == SlugUrl)).AsQueryable().ToPagedList(page, 10);    
                return View(categoryOfBlogs);
            }
            catch (Exception ex)
            {
                logger.LogError(2, ex, "Controller Name: Home, Action: Blogs, Blog Id: {Id}, SlugUrl: {SlugUrl}",Id,SlugUrl);
                return NotFound();
            }            
        }
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Contact(Contact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var captchaImage = HttpContext.Request.Form["g-recaptcha-response"];
                    if (string.IsNullOrEmpty(captchaImage))
                    {
                        ViewBag.LoginAlert = AlertMessageForToastr("Captcha Doğrulayıp Tekrar Deneyin");
                        return BadRequest("Captcha Doğrulayıp Tekrar Deneyin");
                    }
                    var verified = await CheckCaptcha();
                    if (!verified)
                    {
                        ViewBag.LoginAlert = AlertMessageForToastr("Captcha Doğrulaması Hatalı, Tekrar Deneyin");
                        return BadRequest("Captcha Doğrulaması Hatalı, Tekrar Deneyin");
                    }
                    string uzakIPadresi = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                    contact.IpAddress = uzakIPadresi;
                    contact.dateAdded = DateTime.Now;
                    uow.Contacts.Add(contact);
                    uow.SaveChanges();
                    return Ok(); // success çalıştır.  
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    logger.LogError(2, ex, "Controller Name: Home, Action: Contact");
                    return BadRequest("Bir Hata Oluştu.");
                }

            }
            var validError = "Kontrol edip, tekrar deneyin";
            logger.LogError(1, "ModelState Invalid,Controller Name: Home, Action: ContactPost");
            return BadRequest(validError);

        }
        public IActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl; // actionlar arasında veri taşıyabilir.
            ViewBag.SuccessSave = TempData["PasswordReset"] ?? null; // Edit post methodundan geliyor.            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            try
            {
                var captchaImage = HttpContext.Request.Form["g-recaptcha-response"];
                if (string.IsNullOrEmpty(captchaImage))
                {
                    ViewBag.LoginAlert = AlertMessageForToastr("Captcha Doğrulayıp Tekrar Deneyin");
                    return View(login);
                }
                var verified = await CheckCaptcha();
                if (!verified)
                {
                    ViewBag.LoginAlert = AlertMessageForToastr("Captcha Doğrulaması Hatalı, Tekrar Deneyin");
                    return View(login);
                }
                if (ModelState.IsValid)
                {
                    AppUser user = await userManager.FindByEmailAsync(login.Email); // gelen email ile kullanıcı var mı kontrol edilir.
                    if (user != null)
                    {
                        if (await userManager.IsLockedOutAsync(user)) // kullanıcı lock olmuş mu bunun kontrolünü yapar.
                        {
                            ViewBag.LoginAlert = AlertMessageForToastr("Hesabınız Bir Süreliğine Devre Dışı Bırakılmıştır");
                            return View(login);
                        }
                        await signInManager.SignOutAsync(); // login işleminden önce sistemde siteyle ilgili bir cookie olma durumuna karşın logout yapılır.
                        Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, false);
                        if (result.Succeeded) // kullanıcı başarılı giriş yaparsa.
                        {
                            await userManager.ResetAccessFailedCountAsync(user); // kullanıcı başarılı login olursa, başarısız login sayısı sıfırlanır.
                            if (TempData["returnUrl"] != null)
                            {
                                return Redirect(TempData["returnUrl"].ToString());
                            }
                            return RedirectToAction("Index", "Admin");
                        }
                        else // başarısız giriş yaparsa.
                        {
                            await userManager.AccessFailedAsync(user); // başarısız login sayısını bir attırır.
                            int failAccess = await userManager.GetAccessFailedCountAsync(user); // başarısız giriş sayısını tutar.
                            if (failAccess == 3)
                            {
                                await userManager.SetLockoutEndDateAsync(user, new System.DateTimeOffset(DateTime.Now.AddMinutes(20))); // kullanıcı başarısız giriş yaparsa, 20 dakika hesabı kitler.                            
                                ViewBag.LoginAlert = AlertMessageForToastr("Hesabınız Başarısız Girişlerden Dolayı 20 Dakika Kitlenmiştir");
                                return View(login);
                            }
                            ViewBag.LoginAlert = AlertMessageForToastr("Hatalı E-Mail Yada Şifre Girdiniz");
                        }
                    }
                    else
                    {
                        ViewBag.LoginAlert = AlertMessageForToastr("Hatalı E-Mail Yada Şifre Girdiniz");
                    }
                }
                return View(login);
            }
            catch (Exception ex)
            {
                logger.LogError(2, ex, "Controller Name: Home, Action: Login");
                return NotFound();
            }            
        }
        private async Task<bool> CheckCaptcha()
        {
            var postData = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("secret", "6LcUiOUdAAAAAL_bDrph8C1v-zbK4xP6Db9Nt7TW"),
                new KeyValuePair<string, string>("remoteip", HttpContext.Connection.RemoteIpAddress.ToString()),
                new KeyValuePair<string, string>("response", HttpContext.Request.Form["g-recaptcha-response"])
            };

            var client = new HttpClient();
            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(postData));

            var o = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            return (bool)o["success"];
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel forgotPasswordView)
        {
            if(forgotPasswordView.Email != null)
            {
                AppUser user = userManager.FindByEmailAsync(forgotPasswordView.Email).Result; // kullanıcı var mı kontrol edilir.
                if (user != null)
                {
                    try
                    {
                        string passwordResetToken = userManager.GeneratePasswordResetTokenAsync(user).Result;
                        string passwordLink = Url.Action("ResetPasswordConfirm", "Home", new
                        {
                            userId = user.Id,
                            token = passwordResetToken,
                        }, HttpContext.Request.Scheme);
                        //www.localhost.com/Home/ResetPasswordConfirm?userId?kfgj98708=kdfjglkfjdkjhklfjhlkfjghl
                        var settings = uow.Settings.Get(1);
                        MimeMessage message = new MimeMessage();
                        MailboxAddress from = new MailboxAddress(settings.MailUserName, settings.MailUserName);
                        message.From.Add(from);
                        MailboxAddress to = new MailboxAddress(user.UserName, user.Email);
                        message.To.Add(to);
                        message.Subject = "Şifremi Unuttum";
                        BodyBuilder bodyBuilder = new BodyBuilder();
                        bodyBuilder.HtmlBody = "<h2> Şifrenizi Sıfırlamak için aşağıdaki linke tıklayınız </h2><hr/>";
                        bodyBuilder.HtmlBody += $"<p><a href='{passwordLink}'> Şifre Yenilemek İçin Tıklayınız </a></p>";
                        message.Body = bodyBuilder.ToMessageBody();
                        SmtpClient client = new SmtpClient();
                        client.Connect(settings.SmtpAddress, int.Parse(settings.Port), true);
                        client.Authenticate(settings.MailUserName, settings.MailPassword);
                        client.Send(message);
                        client.Disconnect(true);
                        client.Dispose();
                        //ViewBag.PasswordReset = "Şifre Değiştirildi";
                        ViewBag.ForgotPasswordAlert = AlertMessageForToastr("Şifre Sıfırlama Maili Başarıyla Gönderildi", "success");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(2, ex, "Controller Name: Home, Action: ForgotPassword");
                        ViewBag.ForgotPasswordAlert = AlertMessageForToastr("Bir Hatayla Karşılaşıldı", "error");
                        return View(forgotPasswordView);
                    }
                }
                else
                {
                    ViewBag.ForgotPasswordAlert = AlertMessageForToastr("Kullanıcı Bulunamadı");
                }
            }
            else
            {
                ViewBag.ForgotPasswordAlert = AlertMessageForToastr("Kullanıcı Bulunamadı");
            }
            return View(forgotPasswordView);
        }
        public IActionResult ResetPasswordConfirm(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("NewPassword")] ForgotPasswordViewModel model)
        {
            string token = TempData["token"].ToString();
            string userId = TempData["userId"].ToString();
            AppUser user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                try
                {
                    IdentityResult result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        await userManager.UpdateSecurityStampAsync(user); // securitystamp güncellendiğinde kullanıcı yeniden login olur. Böylece userin eski şifreyle olan oturumu sonlandırılmış olur.      
                        TempData["PasswordReset"] = "Şifre Değiştirildi";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(2, ex, "Controller Name: Home, Action: ResetPasswordConfirm");
                    ModelState.AddModelError("", "Bir hatayla karşılaşıldı.");
                    return View(model);
                }                
            }
            else
            {
                ModelState.AddModelError("", "Böyle Bir Kullanıcı Bulunamamıştır");
            }
            return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        [Route("/NotFound")]
        public IActionResult NotFound404()
        {
            return View();
        }
    }
}
