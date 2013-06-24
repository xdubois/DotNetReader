using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotNetReader.Models
{
  
    public class AddFeedModel
    {
        [Required]
        public string Url { get; set; }
        [Display(Name = "Category")]
        public int? CategoryID { get; set; }
    }

    public class FeedUpdateInfo
    {

        public int FeedID { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name = "Category")]
        public int? CategoryID { get; set; }
    }


    public class FeedListItems
    {
      
         public int Id { get; set; }
         public string Category { get; set; }
         public string Name { get; set; }
         public int Stared { get; set; }
         public int NewItemsCount { get; set; }
    }

}


namespace LinqGrouping.Models
{
    public class Group<T, K>
    {
        public K Key;
        public IEnumerable<T> Values;
    }
}
