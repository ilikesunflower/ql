#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Categories
{
    public partial class PartnerShip
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Alias { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedAt { get; set; }
        public int Flag { get; set; }
    }
}