using DotNetReader.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace DotNetReader.Models.init
{
    public class initReader : DropCreateDatabaseAlways<ReaderDb>
    {
        protected override void Seed(ReaderDb context)
        {
            var Categories = new List<Category>
            {
                new Category { Name = "Default", UserId = 1 },
                new Category { Name = "Foreign", UserId = 1 },
            };

            Categories.ForEach(s => context.Categories.Add(s));
            context.SaveChanges();

            SeedMembership();

            var setAdminProfil = context.UserProfiles.Find(1);
            setAdminProfil.Email = "admin@admin.ch";
            setAdminProfil.eventClicked = 0;
            setAdminProfil.eventDownloaded = 0;
            setAdminProfil.eventRead = 0;
            setAdminProfil.feedMaxEvent = 15;
            setAdminProfil.articleDisplayType = 0;
            setAdminProfil.SynchronisationType = 0;
            setAdminProfil.EventPerPage = 15;
            setAdminProfil.SynchroCode = ProfileController.RandomString(20); ;

            context.SaveChanges();

            //On ajoute 2 flux de test
            var Feeds = new List<Feed>
            {
               new Feed {UserId = 1, Name = "PC INpact", Description = "Actualité Informatique", Website = "http://www.pcinpact.com/", Url = "http://feeds.feedburner.com/pcinpact", CategoryID = 1,Lastupdate = new DateTime(2000, 1, 1),},
               new Feed {UserId = 1, Name = "Korben", Description = "Upgrade your mind", Website = "http://www.korben.info", Url = "http://feeds.feedburner.com/KorbensBlog-UpgradeYourMind", CategoryID = 1,Lastupdate = new DateTime(2000,1,1)},
               new Feed {UserId = 1, Name = "WantChinaTimes", Description = "WantChinaTimes.com provides news feeds for our readers. Subscribe to our feeds to get the latest news and links back to full articles - formatted for your favorite feed reader and updated throughout the day.", Website = "http://www.wantchinatimes.com", Url = "http://www.wantchinatimes.com/Rss.aspx?MainCatID=0", CategoryID = 2,Lastupdate = new DateTime(2000,1,1)}
            };
            Feeds.ForEach(s => context.Feeds.Add(s));
            context.SaveChanges();

             //Ajoute quelques articles
            //var Event = new List<Event>();
            //foreach (var feed in context.Feeds)
            //{
                

            //    SyndicationFeed r = SyndicationFeed.Load(System.Xml.XmlReader.Create(feed.Url));

            //    foreach (SyndicationItem item in r.Items)
            //    {
            //        string Link = item.Links.First().Uri.AbsoluteUri;
            //       Event.Add(new Event { FeedID = feed.FeedID, Title = item.Title.Text, Link = Link,  });
            //    }

            //}
            //Event.ForEach(s => context.Events.Add(s));
            //context.SaveChanges();  
        }

       

        private void SeedMembership()
        {
            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);

            var roles = (SimpleRoleProvider)Roles.Provider;
            var membership = (SimpleMembershipProvider)Membership.Provider;

            if (!roles.RoleExists("Admin"))
            {
                roles.CreateRole("Admin");
            }
            if (!roles.RoleExists("User"))
            {
                roles.CreateRole("User");
            }
            if (!roles.RoleExists("Disabled"))
            {
                roles.CreateRole("Disabled");
            }


            if (membership.GetUser("administrator", false) == null)
            {
                membership.CreateUserAndAccount("administrator", "administrator");
            }

            if (!roles.GetRolesForUser("administrator").Contains("Admin"))
            {

                roles.AddUsersToRoles(new[] { "administrator" }, new[] { "Admin" });

            }


        }
    }
}