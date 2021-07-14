using Mapster;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class UserController : BaseController
    {

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
            : base(userManager, signInManager, roleManager)
        {

        }

        public IActionResult Index() // kullanıcılar listelenir
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
                    AddModelError(result);
                    return View(userViewModel);
                }
            }
            return View(userViewModel);
        }
        public IActionResult Profile()
        {
            AppUser user = CurrentUser;
            UserViewModel userView = user.Adapt<UserViewModel>();
            return View(userView);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = CurrentUser;
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
                        AddModelError(result);
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
                AppUser user = CurrentUser;
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
                            AddModelError(result);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mevcut Şifreniz Doğru Değil");
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
        public IActionResult Roles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }
        public IActionResult CreateRole(RoleViewModel roleView)
        {
            if (ModelState.IsValid)
            {
                AppRole role = new AppRole();
                role.Name = roleView.Name;
                IdentityResult result = roleManager.CreateAsync(role).Result;
                if (result.Succeeded)
                {
                    return Ok(role);
                }
                else
                {
                    AddModelError(result);
                    //ModelState.SelectMany(x => x.Value.Errors);
                    string messages = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                        );
                    return BadRequest(messages);
                }
            }
            return BadRequest("Kontrol Edip Tekrar Deneyin");
        }
    }
}
