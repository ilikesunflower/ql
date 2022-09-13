#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.PreOrders;

namespace CMS_EF.Models.Products
{
    public partial class ProductSimilar
    {
        public ProductSimilar()
        {
            ProductSimilarProperty = new HashSet<ProductSimilarProperty>();
            PreOrder = new HashSet<PreOrder>();
        }

        [Key]
        public int Id { get; set; }
        [Column("SKUWH")]
        [StringLength(255)]
        public string Skuwh { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [Column("QuantityWH")]
        public int? QuantityWh { get; set; }
        public int? QuantityUse { get; set; }
        public int? ProductId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedAt { get; set; }
        public int Flag { get; set; }
        public double? Price { get; set; }
        [Column("ProductPropertiesValue", TypeName = "ntext")]
        public string ProductPropertiesValue { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("ProductSimilar")]
        public virtual Products Product { get; set; }
        [InverseProperty("ProductSimilar")]
        public virtual ICollection<ProductSimilarProperty> ProductSimilarProperty { get; set; }
        
        [InverseProperty("ProductSimilar")]
        public virtual ICollection<PreOrder> PreOrder { get; set; }
    }
}