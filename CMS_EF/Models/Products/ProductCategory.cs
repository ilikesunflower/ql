#nullable disable
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Products
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            InversePidNavigation = new HashSet<ProductCategory>();
            ProductCategoryProduct = new HashSet<ProductCategoryProduct>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string NonName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public int Flag { get; set; }
        public int Ord { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string Font { get; set; }
        [StringLength(255)]
        public string ImageBanner { get; set; }
        [StringLength(255)]
        public string ImageBannerMobile { get; set; }
        public int? Lvl { get; set; }
        [StringLength(255)]
        public string Rgt { get; set; }
        public int? Lft { get; set; }
        [Column("PId")]
        public int? Pid { get; set; }

        [ForeignKey("Pid")]
        [InverseProperty("InversePidNavigation")]
        public virtual ProductCategory PidNavigation { get; set; }
        [InverseProperty("PidNavigation")]
        public virtual ICollection<ProductCategory> InversePidNavigation { get; set; }
        [InverseProperty("Pcategory")]
        public virtual ICollection<ProductCategoryProduct> ProductCategoryProduct { get; set; }
    }
}