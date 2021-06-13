using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UlasBlog.WebUI.Controllers
{    
    public class AdminController : Controller
    {
        [Route("/Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
