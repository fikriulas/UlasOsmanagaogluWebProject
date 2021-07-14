using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UlasBlog.WebUI.IdentityCore.CustomValidation
{
    public class CustomRoleValidator : IRoleValidator<AppRole>
    {
        public Task<IdentityResult> ValidateAsync(RoleManager<AppRole> manager, AppRole role)
        {
            List<IdentityError> errors = new List<IdentityError>();
            string[] Digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            foreach (var item in Digits)
            {
                if (role.Name[0].ToString() == item)
                {
                    errors.Add(new IdentityError()
                    {
                        Code = "RoleContainFirsLetterDigit",
                        Description = "Rol Adı Rakam İle Başlayamaz"
                    });
                }
            }
            if (errors.Count == 0)
            {
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
        }
    }
}
