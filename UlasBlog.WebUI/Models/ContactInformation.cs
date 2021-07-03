using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Entity;

namespace UlasBlog.WebUI.Models
{
    public class ContactInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime dateAdded { get; set; }
        public string RelativeTime { get; set; }
    }
}
