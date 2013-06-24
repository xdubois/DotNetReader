using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DotNetReader.Models
{
    public class ReaderDb : DbContext
    {
        public ReaderDb()
            : base("name = DefaultConnection")
        {

        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<Event> Events { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<Feed>()
            //            .HasOptional(p => p.Category)
            //            .WithMany()
            //            .HasForeignKey(p => p.CategoryID);
                      

           // modelBuilder.Configurations.Add(new FeedMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}