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
        public UserController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
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
    }
}
