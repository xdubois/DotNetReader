using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetReader.Models;
using System.Data.Entity;
using WebMatrix.WebData;
using DotNetReader.Utilities;
using System.Xml;

namespace DotNetReader.Controllers
{
   [Authorize]
    public class HomeController : Controller
    {
        ReaderDb _db = new ReaderDb();


        public ActionResult Index()
        {
            ViewBag.UerId = WebSecurity.CurrentUserId;
            ViewBag.Name = WebSecurity.CurrentUserName;

            UserProfile up = _db.UserProfiles.Find(WebSecurity.CurrentUserId);
            ViewBag.SyncType = ((SynchroType)up.SynchronisationType).ToString();
            ViewBag.DisplayType = ((DisplayType)up.articleDisplayType).ToString();

            return View(up);
        }

        protected override void Dispose(bool disposing)
        {
            if (_db != null)
            {
                _db.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
