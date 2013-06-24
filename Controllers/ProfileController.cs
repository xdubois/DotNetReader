using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetReader.Models;
using System.Text;
using DotNetReader.Utilities;

namespace DotNetReader.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ReaderDb db = new ReaderDb();
        private int USERID = WebMatrix.WebData.WebSecurity.CurrentUserId;

        //
        // GET: /Profile/

        public ActionResult Index()
        {

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Edit()
        {

                var up = (from u in db.UserProfiles
                          where u.UserId == USERID
                          select new EditProfile()
                          {
                              articleDisplayType = u.articleDisplayType,
                              feedMaxEvent = u.feedMaxEvent,
                              SynchronisationType = u.SynchronisationType,
                              Email = u.Email,
                              EventPerPage = u.EventPerPage
                          }).SingleOrDefault();

                ViewBag.DisplayTypeList = new SelectList(util.getDisplayTypeDictonnary(), "Key", "Value", up.articleDisplayType);
                ViewBag.SynchroTypeList = new SelectList(util.getSychroDictonary(), "Key", "Value", up.SynchronisationType);
                
            
            
            return View(up);
        }

        //
        // POST: /Profile/Edit/id

        [HttpPost]
        public ActionResult Edit(EditProfile up)
        {
            if (ModelState.IsValid)
            {
                UserProfile current = db.UserProfiles.Find(USERID);
                current.articleDisplayType = up.articleDisplayType;
                current.SynchronisationType = up.SynchronisationType;
                current.feedMaxEvent = up.feedMaxEvent;
                current.Email = up.Email;
                current.EventPerPage = up.EventPerPage;

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.DisplayTypeList = new SelectList(util.getDisplayTypeDictonnary(), "Key", "Value", up.articleDisplayType);
                ViewBag.SynchroTypeList = new SelectList(util.getSychroDictonary(), "Key", "Value", up.SynchronisationType);
            }
            return View(up);
        }

        //
        // GET: /Profile/Delete/5

        public ActionResult Delete(int id = 0)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            return View(userprofile);
        }

        //
        // POST: /Profile/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            db.UserProfiles.Remove(userprofile);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //Genere string unique
        public static string RandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");
            if (string.IsNullOrEmpty(allowedChars)) throw new ArgumentException("allowedChars may not be empty.");

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length) throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.", byteSize));

            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i)
                    {
                        // Divide the byte into allowedCharSet-sized groups. If the
                        // random value falls into the last group and the last group is
                        // too small to choose from the entire allowedCharSet, ignore
                        // the value in order to avoid biasing the result.
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}