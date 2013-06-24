using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetReader.Models;
using WebMatrix.WebData;
using System.ServiceModel.Syndication;
using System.Diagnostics;
using LinqGrouping.Models;
using System.Xml;
using System.Data;
using System.Data.Entity;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Text;
using DotNetReader.Utilities;
using PagedList;

namespace DotNetReader.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        private ReaderDb _db = new ReaderDb();
        private int userId = WebSecurity.CurrentUserId;

        public ActionResult Index()
        {
            var model = _db.Feeds.Include(f => f.Category)
                        .Where(u => u.UserId == userId);

            return View(model.ToList());
        }


        //
        // GET the list of feeds of the user

        [ChildActionOnly]
        public ActionResult ListFeed()
        {
            // get a FeedListItems collection
            var m = from b in _db.Feeds
                    where b.UserId == userId
                    select new FeedListItems()
                    {
                        Id = b.FeedID,
                        Category = b.Category.Name,
                        Name = b.Name,
                        Stared = b.Events.Count(x => x.Favorite == true),
                        NewItemsCount = b.Events.Count(x => x.Unread == null || x.Unread == true) 
                    };

            // Group the feeds by Category
            var FeedListItem = from b in m
                               group b by b.Category into g
                               select new Group<FeedListItems, String> { Values = g, Key = g.Key };

            return PartialView("_FeedList", FeedListItem.ToList());
        }

        //
        // GET the feed view by id

        [HttpGet]
        public ActionResult View(int id, int? page)
        {

            int pageNumber = (page ?? 1);
            UserProfile up = _db.UserProfiles.Find(userId);
            int feedMaxEvent = (int)up.feedMaxEvent;

            Feed feedInfo = _db.Feeds.Find(id);

            if (feedInfo == null)
            {
                return RedirectToAction("Index");
            }

            if (up.SynchronisationType == (int)SynchroType.Partial) //check if we need to update now
            {
                getLastEvent(feedInfo, feedMaxEvent);
                flushCache(feedInfo, feedMaxEvent);
            }

            ViewBag.FeedTitle = feedInfo.Name;
            ViewBag.id = id;
            IEnumerable<Event> model = null;

            if (up.articleDisplayType == (int)DisplayType.UnreadItems)
            {
             
                model = _db.Events
                        .Where(a => a.FeedID == id && (a.Unread == true || a.Unread == null))
                        .OrderByDescending(o => o.Pubdate)
                        .Take(feedMaxEvent);
            }
            else if (up.articleDisplayType == (int)DisplayType.AllItems)
            {
                model = _db.Events
                        .Where(a => a.FeedID == id)
                        .OrderByDescending(o => o.Pubdate)
                        .Take(feedMaxEvent);
            }
            else
            {
                Redirect("Index");
            }

            return View(model.ToPagedList(pageNumber,(int)up.EventPerPage));
        }

        //
        // GET the feeds articles by subcategory

        public ActionResult Views(string slug, int? page){

            int pageNumber = (page ?? 1);
            List<Event> model = new List<Event>();
            UserProfile up = _db.UserProfiles.Find(userId);
            int feedMaxEvent = (int)up.feedMaxEvent;


            if (slug.Equals("All"))
            {
                var feeds = _db.Feeds.Where(u => u.UserId == userId).ToList();
                foreach (Feed feed in feeds)
                {
                    model.AddRange(_db.Events.Where(e => e.FeedID == feed.FeedID && (e.Unread == true || e.Unread == null)).ToList());
                }


            }
            else if (slug.Equals("Star"))
            {
                var feeds = _db.Feeds.Where(u => u.UserId == userId).ToList();
                foreach (Feed feed in feeds)
                {
                    model.AddRange(_db.Events.Where(e => e.FeedID == feed.FeedID && e.Favorite == true).ToList());
                }

            }

            else{

                RedirectToAction("Index");
             }
            ViewBag.Slug = slug;
            return View("View", model.ToPagedList(pageNumber, (int)up.EventPerPage));
        }


        //
        // Mark all item as read from the feed id

        public ActionResult MarkAllAsRead(int id)
        {
            var events = _db.Events.Where(i => i.FeedID == id).ToList();

            if (events != null)
            {
                events.ForEach(s => s.Unread = false);
                _db.SaveChanges();
            }
            return RedirectToAction("View", new { id = id });
        }


        //
        // Update all the feeds
        // !It might take a while to do it


        [AllowAnonymous]
        public ActionResult UpdateAll(string synchroCode)
        {
            if (WebSecurity.IsAuthenticated)
            {
                var feeds = _db.Feeds.Where(u => u.UserId == userId).ToList();
                int maxEvent = (int)_db.UserProfiles.Where(u => u.UserId == userId).SingleOrDefault().feedMaxEvent;

                foreach (Feed feed in feeds)
                {
                    getLastEvent(feed, maxEvent);
                    flushCache(feed, maxEvent);
                }
                return RedirectToAction("Index", "Home");
            }
            else //Si il est pas authentifié on cherche l'utilisateur par son code de synchro
            // localhost:59034/Feed/UpdateAll/P12UIe0vAJLh9DiuMJGN ex
            {
                UserProfile up = _db.UserProfiles.Where(t => t.SynchroCode == synchroCode).SingleOrDefault();
                if (up != null)
                {
                    if (up.SynchronisationType != (int)SynchroType.Manual)
                    {
                        List<Feed> feeds = _db.Feeds.Where(u => u.UserId == up.UserId).ToList();
                        foreach (Feed feed in feeds)
                        {
                            getLastEvent(feed, (int)up.feedMaxEvent, up.UserId);
                            flushCache(feed, (int)up.feedMaxEvent);
                        }

                        return Content("Update done");
                    }
                    else
                    {
                        return Content("Auto update are disabled");
                    }
                }
                else
                {
                    return Content("Invalid synchro code");
                }
            }
        
        }


        //
        // Delete old articles from the specific feed 

        [ChildActionOnly]
        private void flushCache(Feed feedInfo, int feedMaxEvent)
        {

            //Check if the cache > limit defined by the user
            if (feedInfo.Events.Where(f => f.Favorite == false && f.Unread != true).Count() > feedMaxEvent)
            {
                IEnumerable<Event> Ev = feedInfo.Events
                .Where(f => f.Favorite == false && f.Unread != true)
                .OrderBy(i => i.EventID)
                .TakeWhile(c => feedInfo.Events.Count() > feedMaxEvent);

                foreach (Event e in Ev)
                {
                    _db.Events.Remove(e);
                }
                _db.SaveChanges();
            }

        }

        //
        // GET: /Feed/Create

        public ActionResult Create()
        {
            var cat = _db.Categories.Where(u => u.UserId == userId);
            ViewBag.CategoryList = new SelectList(cat, "CategoryID", "Name");
            return View();
        }

        //
        // POST: /Feed/Create

        [HttpPost]
        public ActionResult Create(AddFeedModel feed)
        {
            if (ModelState.IsValid)
            {

                if (isValidFeed(feed.Url))
                {

                    SyndicationFeed f = SyndicationFeed.Load(System.Xml.XmlReader.Create(feed.Url));

                    Feed Coming = new Feed()
                    {
                        Url = feed.Url,
                        Name = f.Title.Text,
                        Lastupdate = new DateTime(2000, 1, 1),
                        Description = f.Description.Text,
                        Website = f.Links.Count > 0 ? f.Links[0].Uri.OriginalString : "",
                        CategoryID = feed.CategoryID,
                        UserId = userId
                    };

                    _db.Feeds.Add(Coming);
                    _db.SaveChanges();

                    //On recupere les derniers articles pour le flux
                    int feedMaxEvent = (int)_db.UserProfiles.Find(WebSecurity.CurrentUserId).feedMaxEvent;
                    getLastEvent(Coming, feedMaxEvent);

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Error, the feed cannot be read. Sorry");
                }
            }
      
            ViewBag.CategoryList = new SelectList(_db.Categories, "CategoryID", "Name");
            return View(feed);
        }


        [HttpPost]
        public void Delete(int id)
        {

            Feed f = _db.Feeds.Find(id);
            _db.Feeds.Remove(f);
            _db.SaveChanges();
             var model = _db.Feeds.Include(c => c.Category);

        }

        public ActionResult Edit(int id)
        {
            var cat = _db.Categories.Where(u => u.UserId == userId);
            ViewBag.CategoryList = new SelectList(cat, "CategoryID", "Name");
            
            var m = from b in _db.Feeds
                    where b.FeedID == id
                    select new FeedUpdateInfo()
                    {
                        Name = b.Name,
                        FeedID = b.FeedID,
                        CategoryID = b.CategoryID

                    };

                

            return View(m.Single());
        }

        [HttpPost]
        public ActionResult Edit(FeedUpdateInfo f)
        {
            if (ModelState.IsValid)
            {

                Feed toUpdate = _db.Feeds.Find(f.FeedID);
                toUpdate.CategoryID = f.CategoryID;
                toUpdate.Name = f.Name;


                _db.SaveChanges();
                return RedirectToAction("Index");

            }
            else
            {
                var cat = _db.Categories.Where(u => u.UserId == userId);
                ViewBag.CategoryList = new SelectList(cat, "CategoryID", "Name");
            }

            return View();
        }



        //
        // Check if a feed is valid

        private bool isValidFeed(string url)
        {
            try
            {
                SyndicationFeed feed = SyndicationFeed.Load(System.Xml.XmlReader.Create(url));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //
        // GET the last articles of the specificc feed

        [ChildActionOnly]
        private void getLastEvent(Feed f, int maxItem,int? uId = null)
        {
            var Event = new List<Event>();

            string content = "";
            string creator = "";
            int itemCounter = 0;

            SyndicationFeed r = SyndicationFeed.Load(System.Xml.XmlReader.Create(f.Url));
            SyndicationItem preCheck = r.Items.First();
          
            IEnumerable<SyndicationItem> items = null;
            //Sometimes the feeds don't have a publishate so we take the lastUpdate
            if (preCheck.PublishDate.DateTime == DateTime.MinValue)
            {
                items = r.Items.Where(d => d.LastUpdatedTime > f.Lastupdate).Take(maxItem);
            }
            else
            {
                items = r.Items.Where(d => d.PublishDate.DateTime > f.Lastupdate).Take(maxItem);
            }
         

            foreach (SyndicationItem item in items)
                {
                    if (item.Authors.Count > 0)
                    {
                        creator = item.Authors[0].Name;
                    }

                    foreach (SyndicationElementExtension ext in item.ElementExtensions)
                    {
                        if (ext.GetObject<XElement>().Name.LocalName == "encoded")
                            content = ext.GetObject<XElement>().Value;


                        if (ext.GetObject<XElement>().Name.LocalName == "creator")
                            creator = ext.GetObject<XElement>().Value;
                    }

                    Event.Add(new Event
                    {
                        FeedID = f.FeedID,
                        Title = item.Title.Text,
                        Link = item.Links.First().Uri.IsAbsoluteUri ? item.Links.First().Uri.AbsoluteUri : item.Links.First().Uri.OriginalString,
                        Favorite = false,
                        Unread = null,
                        Creator = creator,
                        Content = content,
                        Description = item.Summary.Text,
                        Guid = item.Id,
                        Pubdate = item.PublishDate.DateTime == DateTime.MinValue ? item.LastUpdatedTime.DateTime : item.PublishDate.DateTime

                    });
                    itemCounter++;

                }
            //if It cannot read the lastUpdateDate frome the date with set it to now
            f.Lastupdate = r.LastUpdatedTime.DateTime == DateTime.MinValue ? DateTime.Now : r.LastUpdatedTime.DateTime;
            Event.ForEach(s => _db.Events.Add(s));

            uId = uId == null ? userId : uId; //Check if is a cron update
            UserProfile u = _db.UserProfiles.Find(uId);
            


            
            u.eventDownloaded += itemCounter;

            _db.SaveChanges();
        }

        //
        // Set item to Read state

        [HttpPost]
        public ActionResult SetItemRead(int id)
        {
            Event e = _db.Events.Find(id);
          
            if (e.Unread == null)
            {
                e.Unread = false;
                UserProfile u = _db.UserProfiles.Find(userId);
                u.eventRead += 1;
                _db.SaveChanges();
            }

            return ListFeed();
            
        }

        //
        // toggle item to favorite

        [HttpPost]
        public ActionResult SetItemFavorite(int id)
        {
            Event e = _db.Events.Find(id);
            if (e.Favorite)
            {
                e.Favorite = false;
            }
            else
            {
                e.Favorite = true;
            }
            _db.SaveChanges();


            return ListFeed();

        }


        //
        // Set item to unread state

        [HttpPost]
        public ActionResult SetItemUnread(int id, bool unread)
        {
            Event e = _db.Events.Find(id);
            e.Unread = unread;
            _db.SaveChanges();


            return ListFeed();

        }

        [HttpPost]
        public void UpdateItemClicked(int id)
        {
            UserProfile u = _db.UserProfiles.Find(userId);
            u.eventClicked += 1;
            _db.SaveChanges();
        }



        //
        // Export the User's list of feeds to an .opml file

      
        public ActionResult ExportFeed()
        {
            var feeds = _db.Feeds.Where(u => u.UserId == WebSecurity.CurrentUserId).ToList();

            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(dec);
            XmlElement root = doc.CreateElement("opml");
            root.SetAttribute("version", "1.0");
            doc.AppendChild(root);

            XmlElement head = doc.CreateElement("head");
            XmlElement title = doc.CreateElement("title");
            title.InnerText = "Abonnement von" + WebSecurity.CurrentUserName + " in dotNetReader";
            head.AppendChild(title);
            root.AppendChild(head);
            XmlElement body = doc.CreateElement("body");
            root.AppendChild(body);

            foreach (var f in feeds)
            {
                XmlElement outline = doc.CreateElement("outline");
                outline.SetAttribute("text", f.Name);
                outline.SetAttribute("title", f.Name);
                outline.SetAttribute("xmlUrl", f.Url);
                outline.SetAttribute("htmlUrl", f.Website);
                outline.SetAttribute("description", f.Description);
                body.AppendChild(outline);
            }

            string xmlOutput = doc.OuterXml;

            return File(Encoding.UTF8.GetBytes(xmlOutput), "application/xml", "subscriptions.xml");
        }

        [HttpPost]
        public ActionResult ImportFeed(HttpPostedFileBase file)
        {
            List<Feed> validFeed = new List<Feed>();
            if (file != null)
            {

                XDocument doc = XDocument.Load(file.InputStream);

                var xmlFeeds = (from item in doc.Descendants("outline")
                                select new Feed()
                                {
                                    Name = (string)item.Attribute("text"),
                                    Url =(string)item.Attribute("xmlUrl"),
                                    Website = (string)item.Attribute("htmlUrl"),
                                    Description = (string)item.Attribute("description"),
                                    CategoryID = 1,
                                    UserId = userId,
                                    Lastupdate = new DateTime(2000,1,1),
                                }).ToList();
              

               
                //Check for valid feeed
                ViewBag.Valid = 0;
                foreach (var item in xmlFeeds)
                {
                    if (isValidFeed(item.Url))
                    {
                        validFeed.Add(item);
                        ViewBag.valid++;
                    }
                }

                //add the feeds to the db
                validFeed.ForEach(s => _db.Feeds.Add(s));
                _db.SaveChanges();
                return View();
            }
            return Content("Invalid file");
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
