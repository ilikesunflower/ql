#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Products
{
    public partial class ProductGroup
    {
        public ProductGroup()
        {
            Products = new HashSet<Products>();
        }

        [Key]
        public int Id { get; set; }
        public int? Name { get; set; }
        [StringLength(255)]
        public string NonName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public int Flag { get; set; }

        [InverseProperty("ProductGroup")]
        public virtual ICollection<Products> Products { get; set; }
    }
}