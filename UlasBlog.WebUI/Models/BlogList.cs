using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UlasBlog.WebUI.Models
{
    public class BlogList
    {
        public IEnumerable<BlogDetail> Blogs { get; set; }
    }
}
