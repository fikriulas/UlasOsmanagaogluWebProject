using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Entity;

namespace UlasBlog.WebUI.Models
{
    public class AddComment
    {
        public Comment comment { get; set; }
        public int BlogId { get; set; }
    }
}
