using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DotNetReader.Models
{
    public class Feed
    {
        [Key]
        public int FeedID { get; set; }
        public int? CategoryID { get; set; } //not nullable in db model
        public int UserId { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(256)]
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(512)]
        public string Description { get; set; } // 256 in db model

        [Column(TypeName = "NVARCHAR")]
        [StringLength(256)]
        public string Website { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(256)]
        public string Url { get; set; }
        public bool DisplayPublic { get; set; }

        [Column(TypeName = "DATETIME2")] 
        public Nullable<DateTime> Lastupdate { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual Category Category { get; set; }
        public virtual UserProfile User { get; set; }

    }
}