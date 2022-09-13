#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Categories
{
    public partial class Banner
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Link { get; set; }
        public int? Ord { get; set; }

        [StringLength(255)]
        public string Alias { get; set; }  

        [StringLength(255)]
        public string Images { get; set; }
        
        [StringLength(255)]
        public string ImagesMobile { get; set; }
        public bool Status { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public int Flag { get; set; }
    }
}