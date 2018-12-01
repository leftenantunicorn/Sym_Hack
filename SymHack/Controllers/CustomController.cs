using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymHack.Controllers
{
    public class CustomController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            filterContext.Result = RedirectToAction("Error", "Home");
        }
    }
}