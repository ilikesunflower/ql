#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.Categories;

namespace CMS_EF.Models.Customers
{
    public partial class CustomerAddress
    {
        [Key] public int Id { get; set; }
        public int? CustomerId { get; set; }
        [StringLength(255)] public string ProvinceCode { get; set; }
        [StringLength(255)] public string DistrictCode { get; set; }
        [StringLength(255)] public string CommuneCode { get; set; }
        [StringLength(255)] public string Phone { get; set; }
        [StringLength(150)] public string Address { get; set; }
        public bool? IsDefault { get; set; }
        [Column(TypeName = "datetime")] public DateTime? LastModifiedAt { get; set; }
        public int Flag { get; set; }
        [StringLength(255)] public string Email { get; set; }
        [StringLength(255)] public string Name { get; set; }

        [ForeignKey("CommuneCode")]
        [InverseProperty("CustomerAddress")]
        public virtual Commune CommuneCodeNavigation { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty("CustomerAddress")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("DistrictCode")]
        [InverseProperty("CustomerAddress")]
        public virtual District DistrictCodeNavigation { get; set; }

        [ForeignKey("ProvinceCode")]
        [InverseProperty("CustomerAddress")]
        public virtual Province ProvinceCodeNavigation { get; set; }
    }
}