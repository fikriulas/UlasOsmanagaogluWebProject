using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            var roles = new List<SelectListItem>();
            foreach (var item in roleManager.Roles)
            {
                roles.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Name
                });
            }
            ViewBag.Roles = roles;
            var users = userManager.Users;
            ViewBag.alertMessage = TempData["alertMessage"] ?? null; // Edit post methodundan geliyor.
            return View(users);
        }
        public IActionResult Add()
        {
            var roles = new List<SelectListItem>();
            foreach (var item in roleManager.Roles)
            {
                roles.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Name
                });
            }
            ViewBag.Roles = roles;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(UserViewModel userViewModel, string[] Roles)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser();                
                user.UserName = userViewModel.UserName;
                user.Email = userViewModel.Email;
                user.Name = userViewModel.Name;
                user.Surname = userViewModel.Surname;
                IdentityResult result = await userManager.CreateAsync(user, userViewModel.Password); // passwordu şifreleyip kaydeder.
                if (result.Succeeded)
                {
                    AppUser CurrentUser = await userManager.FindByNameAsync(user.UserName);
                    foreach (var item in Roles)
                    {
                        IdentityResult AddRoleResult = await userManager.AddToRoleAsync(CurrentUser, item);
                        if (AddRoleResult.Succeeded != true)
                        {
                            string message = AlertMessageByModalError(AddRoleResult);
                            ViewBag.Roles = AllRoles();
                            return BadRequest(AlertMessageForToastr(message));
                        }
                    }
                    return Ok(user);
                }
                else
                {
                    string message = AlertMessageByModalError(result);
                    return BadRequest(message);
                }
            }
            return BadRequest("Kontrol Edip Tekrar Deneyiniz");
        }
        public IActionResult Edit(string Id)
        {
            AppUser user = userManager.FindByIdAsync(Id).Result;
            if (user != null)
            {
                ViewBag.currentRoles = CurrentRoles(user);
                ViewBag.userRoles = AllRoles();
                UserEditViewModel userView = new UserEditViewModel();
                userView.Name = user.Name;
                userView.Surname = user.Surname;
                userView.Email = user.Email;
                userView.UserName = user.UserName;
                userView.Id = user.Id;
                return View(userView);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel userView, string[] roles)
        {
            AppUser user = await userManager.FindByIdAsync(userView.Id);
            if (ModelState.IsValid)
            {                
                string[] userRoles = CurrentRoles(user).ToArray();
                if (userRoles.Length != 0)
                {
                    foreach (var item in userRoles)
                    {
                        var roleName = FindRoleName(item);
                        IdentityResult removeResult = await userManager.RemoveFromRoleAsync(user, roleName);
                        if (removeResult.Succeeded != true)
                        {                            
                            string message = AlertMessageByModalError(removeResult);
                            ViewBag.alertMessage = AlertMessageForToastr(message);
                            ViewBag.currentRoles = CurrentRoles(user);
                            ViewBag.userRoles = AllRoles();
                            return View(userView);
                        }
                    }
                }
                foreach (var item in roles)
                {
                    var roleName = FindRoleName(item); // basecontrollerdan id ile rol adı bulur.
                    IdentityResult AddRoleResult = await userManager.AddToRoleAsync(user, roleName);
                    if (AddRoleResult.Succeeded != true)
                    {
                        string message = AlertMessageByModalError(AddRoleResult);
                        ViewBag.alertMessage = AlertMessageForToastr(message);
                        ViewBag.currentRoles = CurrentRoles(user);
                        ViewBag.userRoles = AllRoles();
                        return View(userView);
                    }
                }
                user.UserName = userView.UserName;
                user.Email = userView.Email;
                user.Name = userView.Name;
                user.Surname = userView.Surname;
                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.UpdateSecurityStampAsync(user); // security stamp değişti. // 30 dakika sonra otomatik çıkış yaptıracak.
                    await signInManager.SignOutAsync(); // çıkış yapılır
                    await signInManager.SignInAsync(user, true); // giriş yapılır, cookie güncellenir.
                    TempData["alertMessage"] = "Güncelleme İşlemi Başarılı";
                    return RedirectToAction("Index");
                }
                else
                {
                    string message = AlertMessageByModalError(result);
                    ViewBag.alertMessage = AlertMessageForToastr(message);
                    ViewBag.currentRoles = CurrentRoles(user); //basecontrollerda
                    ViewBag.userRoles = AllRoles();
                    return View(userView);
                }
            }
            ViewBag.currentRoles = CurrentRoles(user); //basecontrollerda
            ViewBag.userRoles = AllRoles();
            return View(userView);
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
        public IActionResult EditProfile(UserViewModel userView)
        {
            AppUser user = userManager.FindByIdAsync(userView.Id).Result;
            if (user != null)
            {
                user.UserName = userView.UserName;
                user.Name = userView.Name;
                user.Surname = userView.Surname;
                user.Email = userView.Email;
                var result = userManager.UpdateAsync(user).Result;
                if (result.Succeeded)
                {
                    return Ok(user);
                }
                else
                {
                    string message = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                        );
                    return BadRequest(message);
                }
            }
            return BadRequest("Kullanıcı Bulunamadı");
        }
        public IActionResult DeleteUser(string Id)
        {
            AppUser user = userManager.FindByIdAsync(Id).Result;
            if (user != null)
            {
                IdentityResult result = userManager.DeleteAsync(user).Result;
                if (result.Succeeded)
                {
                    return Ok(user.Id);
                }
                else
                {
                    AddModelError(result);
                    string message = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                        );
                    return BadRequest(message);
                }
            }
            return BadRequest("Kullanıcı Bulunamadı");
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
        public IActionResult EditRole(RoleViewModel roleView)
        {
            if (ModelState.IsValid)
            {
                AppRole role = roleManager.FindByIdAsync(roleView.Id).Result;
                role.Name = roleView.Name;
                IdentityResult result = roleManager.UpdateAsync(role).Result;
                if (result.Succeeded)
                {
                    return Ok(role);
                }
                else
                {
                    AddModelError(result);
                    string message = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                        );
                    return BadRequest(message);
                }
            }
            return BadRequest("Kontrol Edip Tekrar Deneyin");
        }
        public IActionResult DeleteRole(string Id)
        {
            AppRole role = roleManager.FindByIdAsync(Id).Result;
            if (role != null)
            {
                IdentityResult result = roleManager.DeleteAsync(role).Result;
                if (result.Succeeded)
                {
                    return Ok(Id);
                }
                else
                {
                    AddModelError(result);
                    string message = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                        );
                    return BadRequest(message);
                }
            }
            return BadRequest("Rol Veritabanında Bulunamadı");
        }
    }
}
