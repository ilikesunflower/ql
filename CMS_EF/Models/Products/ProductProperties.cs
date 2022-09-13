#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Products
{
    public partial class ProductProperties
    {
        public ProductProperties()
        {
            ProductPropertieValue = new HashSet<ProductPropertieValue>();
        }

        [Key]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string NonName { get; set; }
        public int Flag { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("ProductProperties")]
        public virtual Products Product { get; set; }
        [InverseProperty("ProductProperties")]
        public virtual ICollection<ProductPropertieValue> ProductPropertieValue { get; set; }
    }
}