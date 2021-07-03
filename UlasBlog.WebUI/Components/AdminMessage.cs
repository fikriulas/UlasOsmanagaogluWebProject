using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;
using UlasBlog.Entity;
using UlasBlog.WebUI.Models;

namespace UlasBlog.WebUI.Components
{
    public class AdminMessage : ViewComponent
    {
        /*
         * ana sayfada sağ taraftaki sidebarı temsil eder.
         * burada 3 adet blog ve tüm kategoriler listeleneceği için blogandcategory modeli
         * kullanılmıştır. 
         * */
        private IUnitOfWork uow;
        public AdminMessage(IUnitOfWork _unitOfWork)
        {
            uow = _unitOfWork;
        }
        public IViewComponentResult Invoke()
        {
            // vote'a göre ilk 3 bloğu alır. 
            var contacts = uow.Contacts.GetAll()
                .Where(i=>i.IsRead == false)
                .Select(i => new ContactInformation()
                {
                    Id = i.Id,
                    Message = i.Message.Substring(0, 10),
                    Name = i.Name,
                    dateAdded = i.dateAdded,                    
                }).ToList();
            foreach(var contact in contacts)
            {
                contact.RelativeTime = RelativeTime(contact.dateAdded);
            }
            return View(contacts);
        }
        public string RelativeTime(DateTime date)
        {
            
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - date.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }
    }
}
