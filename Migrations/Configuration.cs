namespace DotNetReader.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using DotNetReader.Models;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<DotNetReader.Models.ReaderDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(DotNetReader.Models.ReaderDb context)
        {
            context.Categories.AddOrUpdate(r => r.Name,
                new Category { Name = "Public" },
                new Category { Name = "Tech" },
                new Category { Name = "Blog" },
                new Category { Name = "News" });

            context.Feeds.AddOrUpdate(r => r.Name,
                new Feed {UserId = 2, Name = "Le super flux RSS BashFR.org", Description = "", Website = "http://danstonchat.com/", Url = "http://feeds.feedburner.com/bashfr", CategoryID = 1 },
                new Feed {UserId = 2, Name = "Clubic.com - Actualité", Description = "articles", Website = "http://clubic.com/", Url = " 	http://www.clubic.com/xml/news.xml",  CategoryID = 1 },
               new Feed {UserId = 2, Name = "PC INpact", Description = "Actualit&eacute;s Informatique", Website = "http://www.pcinpact.com/", Url = "http://feeds.feedburner.com/pcinpact", CategoryID = 2 });


            SeedMemberShip();


            
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
        }

        



        private void SeedMemberShip()
        {
            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true); //put in static method

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

            if(membership.GetUser("dubois", false) == null){

                membership.CreateUserAndAccount("dubois","dubois");
            }

            if(!roles.GetRolesForUser("dubois").Contains("Admin")){

                roles.AddUsersToRoles(new [] {"dubois"}, new [] {"Admin"});

            }



        }
    }
}
