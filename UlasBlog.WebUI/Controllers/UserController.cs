using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.WebUI.IdentityCore;
using UlasBlog.WebUI.Models;

namespace UlasBlog.WebUI.Controllers
{
    public class UserController : Controller
    {
        private UserManager<AppUser> userManager { get; }       
        private SignInManager<AppUser> signInManager { get; }
        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public IActionResult Index()
        {
            var users = userManager.Users;
            ViewBag.SuccessSave = TempData["AddUser"] ?? null; // Edit post methodundan geliyor.
            return View(users);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(UserViewModel userViewModel)
        {
            List<string> dizi = new List<string>();
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser();
                user.UserName = userViewModel.UserName;
                user.Email = userViewModel.Email;
                IdentityResult result = await userManager.CreateAsync(user, userViewModel.Password); // passwordu şifreleyip kaydeder.
                if (result.Succeeded)
                {
                    TempData["AddUser"] = "Kullanıcı Eklendi";
                    return RedirectToAction("Index");
                }                    
                else
                {
                    foreach (IdentityError item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);                       
                    }
                    return View(userViewModel);
                }
            }
            return View(userViewModel);
        }
        public IActionResult Profile()
        {
            AppUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
            UserViewModel userView = user.Adapt<UserViewModel>();
            return View(userView);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(User.Identity.Name);
                bool passwordCheck = await userManager.CheckPasswordAsync(user, userViewModel.Password);
                if (passwordCheck)
                {
                    user.UserName = userViewModel.UserName;
                    user.Email = userViewModel.Email;
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        await userManager.UpdateSecurityStampAsync(user); // security stamp değişti. // 30 dakika sonra otomatik çıkış yaptıracak.
                        await signInManager.SignOutAsync(); // çıkış yapılır
                        await signInManager.SignInAsync(user, true); // giriş yapılır, cookie güncellenir.
                        ViewBag.SuccessChange = "Güncelleme İşlemi Başarılı";
                        return View();
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("",item.Description);
                        }
                    }
                }
                else
                {
                    ViewBag.AlertMessage = "Şifrenizi Yanlış Girdiniz";
                }
            }
            return View(userViewModel);
        }
        public IActionResult PasswordChange()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PasswordChange(PasswordChange password)
        {
            if (ModelState.IsValid)
            {
                AppUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
                if (user != null)
                {
                    bool exist = userManager.CheckPasswordAsync(user, password.OldPassword).Result; // user'ın mevcut şifresnini doğruluğunu kontrol eder.
                    if (exist)
                    {
                        IdentityResult result = userManager.ChangePasswordAsync(user, password.OldPassword, password.NewPassword).Result;
                        if (result.Succeeded)
                        {
                            userManager.UpdateSecurityStampAsync(user); // security stamp değişti. // 30 dakika sonra otomatik çıkış yaptıracak.
                            signInManager.SignOutAsync();
                            signInManager.PasswordSignInAsync(user, password.NewPassword, true, false); // kullanıcıyı logout yapıp login yaptırılır. Böylece yeni şifreyle cookie oluşturulur.
                            ViewBag.SuccessChange = "Şifre Başarıyla Değiştirildi.";
                            return View();
                        }
                        else
                        {
                            foreach (var item in result.Errors)
                            {
                                ModelState.AddModelError("",item.Description);
                            }                            
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("","Mevcut Şifreniz Doğru Değil");
                    }
                }                
            }
            return View(password);            
        }
        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
