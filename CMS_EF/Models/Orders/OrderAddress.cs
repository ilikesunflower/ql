#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.Categories;

namespace CMS_EF.Models.Orders
{
    public partial class OrderAddress
    {
        [Key] public int Id { get; set; }
        public int? OrderId { get; set; }
        public string ProvinceCode { get; set; }
        public string DistrictCode { get; set; }
        public string CommuneCode { get; set; }
        [Column(TypeName = "ntext")] public string Address { get; set; }
        [Column(TypeName = "datetime")] public DateTime? LastModifiedAt { get; set; }
        public int Flag { get; set; }
        [StringLength(255)]
        public string Note { get; set; }
        [StringLength(255)]
        public string Email { get; set; }
        public string Name { get; set; }
        [StringLength(255)] public string Phone { get; set; }
        
        [ForeignKey("ProvinceCode")]
        public virtual Province Province { get; set; }
        
        [ForeignKey("DistrictCode")]
        public virtual District District { get; set; } 
        
        [ForeignKey("CommuneCode")]
        public virtual Commune Commune { get; set; } 

        [ForeignKey("OrderId")]
        [InverseProperty("OrderAddress")]
        public virtual Orders Order { get; set; }
    }
}