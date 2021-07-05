using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UlasBlog.WebUI.IdentityCore.CustomValidation
{
    public class CustomIdentityErrorDescriber:IdentityErrorDescriber
    {
        public override IdentityError InvalidUserName(string userName)
        {
            //return base.InvalidUserName(userName);
            return new IdentityError()
            {
                Code = "InvalidUserName",
                Description = $"{userName} Geçersizdir."
            };
        }        
        public override IdentityError DuplicateEmail(string email)
        {
            //return base.DuplicateEmail(email);
            return new IdentityError()
            {
                Code = "DuplicateEmail",
                Description = $"{email} Kullanılmaktadır."
            };
        }
        public override IdentityError PasswordTooShort(int length)
        {
            //return base.PasswordTooShort(length);
            return new IdentityError()
            {
                Code = "PasswordTooShort",
                Description = $"Şifre En Az {length} Karakterten Oluşmalıdır."
            };
        }


    }
}
