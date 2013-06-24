using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetReader.Models;
using WebMatrix.WebData;

namespace DotNetReader.Controllers
{
     [Authorize]
    public class CategoryController : Controller
    {
        private ReaderDb db = new ReaderDb();
        private int UserId = WebSecurity.CurrentUserId;

        //
        // GET: /Category/

        public ActionResult Index()
        {

            var model = db.Categories.Where(x => x.UserId == UserId).ToList();

            return View(model);
        }


        //
        // GET: /Category/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Category/Create

        [HttpPost]
        public ActionResult Create(Category category)
        {
                if (ModelState.IsValid)
                {
                    var myCategory = db.Categories
                                        .Where(x => x.UserId == UserId)
                                        .Where(x => x.Name == category.Name.ToLower())
                                        .Count();

                    if (myCategory == 0)
                    {
                        category.UserId = UserId;
                        db.Categories.Add(category);
                        db.SaveChanges();
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        ModelState.AddModelError("", "The category " + category.Name + " already exist.");
                    }
                }

       
            return View(category);
        }

        //
        // GET: /Category/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // POST: /Category/Edit/5

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            category.UserId = UserId;
            if (ModelState.IsValid)
            {

                var myCategory = db.Categories
                                     .Where(x => x.UserId == UserId)
                                     .Where(x => x.Name == category.Name.ToLower())
                                     .Count();

                if (myCategory == 0)
                {
                    category.UserId = UserId;
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                else
                {
                    ModelState.AddModelError("", "The category " + category.Name + " already exist.");
                }         
            }
            return View(category);
        }

        //
        // GET: /Category/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Category category = db.Categories.Find(id);

            
            
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // POST: /Category/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            //Set feeds to default category 1
            var feedsToUpdate = from c in db.Feeds
                          where c.CategoryID == id && c.UserId == UserId
                          select c;
            foreach( var f in feedsToUpdate){
                f.CategoryID = 1;
            }

            
            //Delete the category
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}