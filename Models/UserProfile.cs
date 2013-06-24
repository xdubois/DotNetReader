using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DotNetReader.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [DisplayName("User Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string UserName { get; set; }


        [EmailAddress]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(256)]
        public string Email { get; set; }

        [DefaultValue(0)]
        [Display(Name = "Articles read")]
        public Nullable<int> eventRead { get; set; }

        [DefaultValue(0)]
        [Display(Name = "Articles downloaded")]
        public Nullable<int> eventDownloaded { get; set; }

        [DefaultValue(0)]
        [Display(Name = "Articles clicked")]
        public Nullable<int> eventClicked { get; set; }

        [Column(TypeName = "SMALLINT")]
        [DefaultValue(30)]
        [Display(Name = "Cache limit")]
        public Nullable<short> feedMaxEvent { get; set; }

        [Column(TypeName = "SMALLINT")]
        [DefaultValue(0)]
        [Display(Name = "Synchronisation Type")]
        public Nullable<short> SynchronisationType { get; set; }

        [Column(TypeName = "SMALLINT")]
        [DefaultValue(0)]
        [Display(Name = "Display")]
        public Nullable<short> articleDisplayType { get; set; }


        [Column(TypeName = "NVARCHAR")]
        [StringLength(20)]
        public string SynchroCode { get; set; } // NEW NOT IN MODEL

        [Column(TypeName = "SMALLINT")]
        [Display(Name = "Article per page")]
        public Nullable<short> EventPerPage { get; set; } // NEW NOT IN MODEL

        public ICollection<Feed> Feeds { get; set; }
    }


    //ViewModel

    public class EditProfile
    {
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Articles cache limit")]
        [Range(1,100)]
        public short? feedMaxEvent { get; set; }

        [Display(Name = "Synchronisation type")]
         [Range(0, 10)]
        public short? SynchronisationType { get; set; }

        [Display(Name = "Display article mode")]
         [Range(0, 10)]
        public short? articleDisplayType { get; set; }

        [Display(Name = "Articles per page")]
         [Range(1, 100)]
         public short? EventPerPage { get; set; } 

    }
}