using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.WebUI.IdentityCore;

namespace UlasBlog.WebUI.Helper
{
    [HtmlTargetElement("td",Attributes="user-roles")]
    public class UserRoleName:TagHelper
    {
        public UserManager<AppUser> userManager { get; set; }
        public UserRoleName(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        [HtmlAttributeName("user-roles")]
        public string UserId { get; set; }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            AppUser user = await userManager.FindByIdAsync(UserId);
            IList<string> roles = await userManager.GetRolesAsync(user);
            string html = string.Empty;
            roles.ToList().ForEach(x =>
            {
                html += $"{x},"; 
            });
            output.Content.SetHtmlContent(html);
        }
    }
}
