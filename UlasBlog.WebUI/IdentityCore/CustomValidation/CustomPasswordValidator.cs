using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UlasBlog.WebUI.IdentityCore.CustomValidation
{
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();
            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordContainsUserName",
                    Description = "Şifre Alanı Kullanıcı Adı İçeremez"
                });
            }
            if (password.ToLower().Contains("123456"))
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordContain123456",
                    Description = "Şifre Alanı Ardaşık Sayı İçeremez"
                });
            }
            if(password.ToLower().Contains(user.Email.ToLower()))
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordContainsEmail",
                    Description = "Şifre Alanı Mail Adresinizi İçeremez"
                });
            }
            if(errors.Count == 0)
            {
                return Task.FromResult(IdentityResult.Success);
            }
            else
            { // ıdentity result success yada failed döner. Fail'e döndüğünde IdentityError tipinde bir array ister.
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
        }
    }
}
