using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Flipper.Admin.Models;

namespace Flipper.Admin.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewBag.FlipperInfo = new FlipperInfo(new ServiceStack.Redis.BasicRedisClientManager("localhost:6379"));
            return View();
        }

    }
}
