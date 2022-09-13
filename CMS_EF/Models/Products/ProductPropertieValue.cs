#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Products
{
    public partial class ProductPropertieValue
    {
        public ProductPropertieValue()
        {
            ProductSimilarProperty = new HashSet<ProductSimilarProperty>();
        }

        [Key]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? ProductPropertiesId { get; set; }
        [StringLength(255)]
        public string NonValue { get; set; }
        [StringLength(255)]
        public string Value { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public int Flag { get; set; }

        [ForeignKey("ProductPropertiesId")]
        [InverseProperty("ProductPropertieValue")]
        public virtual ProductProperties ProductProperties { get; set; }
        [InverseProperty("ProductPropertiesValue")]
        public virtual ICollection<ProductSimilarProperty> ProductSimilarProperty { get; set; }
    }
}