#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.Customers;
using Microsoft.EntityFrameworkCore;

namespace CMS_EF.Models.Categories
{
    public partial class District
    {
        public District()
        {
            Commune = new HashSet<Commune>();
            CustomerAddress = new HashSet<CustomerAddress>();
        }

        [Key]
        [StringLength(255)]
        public string Code { get; set; }
        [Required]
        [StringLength(255)]
        public string ProvinceCode { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public int Flag { get; set; }
        [StringLength(255)]
        public string DistrictGhnId { get; set; }
        [StringLength(255)]
        public string DistrictVnPostId { get; set; }

        [ForeignKey("ProvinceCode")]
        [InverseProperty("District")]
        public virtual Province ProvinceCodeNavigation { get; set; }
        [InverseProperty("DistrictCodeNavigation")]
        public virtual ICollection<Commune> Commune { get; set; }
        [InverseProperty("DistrictCodeNavigation")]
        public virtual ICollection<CustomerAddress> CustomerAddress { get; set; }
    }
}