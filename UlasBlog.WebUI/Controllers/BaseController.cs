using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.WebUI.IdentityCore;

namespace UlasBlog.WebUI.Controllers
{
    public class BaseController : Controller
    {
        protected UserManager<AppUser> userManager { get; }
        protected SignInManager<AppUser> signInManager { get; }
        protected RoleManager<AppRole> roleManager { get; }
        protected AppUser CurrentUser => userManager.FindByNameAsync(User.Identity.Name).Result;
        public BaseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }
        public void AddModelError(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
        }
        public string FindRoleName(string Id)
        {
            AppRole role = roleManager.FindByIdAsync(Id).Result;
            return role.Name;
        }
        public List<string> CurrentRoles(AppUser user)
        {
            List<string> userRoles = userManager.GetRolesAsync(user).Result as List<string>; // kullanıcının rollerini getirir. Getirilen rollerin sadece name'i gelir.
            List<string> currentRoles = new List<string> { }; // kullanıcı rolleri listeye atılacağı için string tipinde bir liste oluşturulur.
            for (int i = 0; i < userRoles.Count; i++)
            {
                var role = roleManager.FindByNameAsync(userRoles[i]).Result; // kullanıcıdan çekilen roller yenirole'e atanır.                    
                currentRoles.Add(role.Id.ToString());
            }
            return currentRoles;
        }
        public List<SelectListItem> AllRoles()
        {
            var userSelectRoles = new List<SelectListItem>(); // dropdown için select list oluşturulur.
            var allRoles = roleManager.Roles; //selectliste tüm rolleri atamak için roller getirilir.         
            foreach (var item in allRoles) // tüm rolleri selectlistte aktarır. Selectlist view'deki dropdown'ı dolduracak.
            {
                userSelectRoles.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id
                });
            }
            return userSelectRoles;
        }
        public string AlertMessageByModalError(IdentityResult result)
        {
            AddModelError(result);
            string message = string.Join("; ", ModelState.Values
                                                        .SelectMany(x => x.Errors)
                                                        .Select(x => x.ErrorMessage)
                                                        );
            return message;
        }
        public string AlertMessageForToastr(string alertMessage)
        {
            var returnText = "";
            string[] words = alertMessage.Split(';');            
            foreach (var word in words)
            {
                returnText += "toastr.error('" + word + "');";
            }
            return returnText;
        }
    }
}
