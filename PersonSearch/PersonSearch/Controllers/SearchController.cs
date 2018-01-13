using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PersonSearch.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult SimpleSearch()
        {
            return View();
        }

        public ActionResult AdvancedSearch()
        {
            return View();
        }

    }
}