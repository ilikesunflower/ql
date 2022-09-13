#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Products
{
    public partial class ProductSimilarProperty
    {
        [Key]
        public int Id { get; set; }
        public int? ProductSimilarId { get; set; }
        public int? ProductPropertiesValueId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public int Flag { get; set; }

        [ForeignKey("ProductPropertiesValueId")]
        [InverseProperty("ProductSimilarProperty")]
        public virtual ProductPropertieValue ProductPropertiesValue { get; set; }
        [ForeignKey("ProductSimilarId")]
        [InverseProperty("ProductSimilarProperty")]
        public virtual ProductSimilar ProductSimilar { get; set; }
    }
}