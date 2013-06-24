using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DotNetReader.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }
        public int FeedID { get; set; }
        public string Guid { get; set; } // is integer in db model
        public string Title { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(256)]
        public string Creator { get; set; }

        public string Content { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "NVARCHAR")] 
        [StringLength(256)]
        public string Link { get; set; }

        [Column(TypeName = "DATETIME2")] 
        public Nullable<DateTime> Pubdate { get; set; }
        public bool Favorite { get; set; }
        public bool? Unread { get; set; } //not nullable in db model
        public virtual Feed Feed { get; set; }
    }
}