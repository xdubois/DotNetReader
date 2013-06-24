using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DotNetReader.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public int UserId { get; set; } //Not present in db model

        [Required]
        [DisplayName("Category")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Name { get; set; }
        public virtual ICollection<Feed> Feeds { get; set; }

    }
}