using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Controllers.Filters;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Your contact page.";
            return View("Contact");
        }

        [ActionName("send-message")]
        [HttpPost, AjaxValidation]
        public ActionResult SendMessage(CustomerMessage model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.SuccessMessage = "Message Sent!";
                model = new CustomerMessage();
                ModelState.Clear();
            }
            return PartialView("Message", model);
        }
    }
}