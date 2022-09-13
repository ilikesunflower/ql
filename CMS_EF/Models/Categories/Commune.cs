#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.Customers;
using Microsoft.EntityFrameworkCore;

namespace CMS_EF.Models.Categories
{
    public partial class Commune
    {
        public Commune()
        {
            CustomerAddress = new HashSet<CustomerAddress>();
        }

        [Required]
        [StringLength(255)]
        public string DistrictCode { get; set; }
        [Required]
        [StringLength(255)]
        public string ProvinceCode { get; set; }
        [Key]
        [StringLength(255)]
        public string Code { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public int Flag { get; set; }
        [StringLength(255)]
        public string CommuneGhnId { get; set; }
        [StringLength(255)]
        public string CommuneVnPostId { get; set; }

        [ForeignKey("DistrictCode")]
        [InverseProperty("Commune")]
        public virtual District DistrictCodeNavigation { get; set; }
        [InverseProperty("CommuneCodeNavigation")]
        public virtual ICollection<CustomerAddress> CustomerAddress { get; set; }
    }
}