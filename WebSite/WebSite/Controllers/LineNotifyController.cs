using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class LineNotifyController : Controller
    {
        public ActionResult Index()
        {
            var vm = new LineNotifySubscriberViewModel();
            return View(vm);
        }

        public ActionResult Subscribe()
        {
            var vm = new LineNotifySubscriberViewModel
            {
                HasSubscribed = true
            };
            return View("Index", vm);
        }
        
        public ActionResult Unsubscribe()
        {
            var vm = new LineNotifySubscriberViewModel
            {
                HasSubscribed = false
            };
            return View("Index", vm);
        }
    }
}