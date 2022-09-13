#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.Customers;
using Microsoft.EntityFrameworkCore;

namespace CMS_EF.Models.Categories
{
    public partial class Province
    {
        public Province()
        {
            CustomerAddress = new HashSet<CustomerAddress>();
            District = new HashSet<District>();
        }

        [Key]
        [StringLength(255)]
        public string Code { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public int Flag { get; set; }
        [StringLength(255)]
        public string ProvinceGhnId { get; set; }
        [StringLength(255)]
        public string ProvinceVnPostId { get; set; }
        
        public int? Area { get; set; }

        [InverseProperty("ProvinceCodeNavigation")]
        public virtual ICollection<CustomerAddress> CustomerAddress { get; set; }
        [InverseProperty("ProvinceCodeNavigation")]
        public virtual ICollection<District> District { get; set; }
    }
}