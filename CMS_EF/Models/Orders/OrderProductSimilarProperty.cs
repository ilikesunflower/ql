#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Orders
{
    public partial class OrderProductSimilarProperty
    {
        [Key] public int Id { get; set; }
        public int? OrderProductId { get; set; }
        [StringLength(255)] 
        public string ProductPropertiesName { get; set; }
        [Column(TypeName = "datetime")] 
        public DateTime? LastModifiedAt { get; set; }
        public int Flag { get; set; }
        public int? ProductPropertiesId { get; set; }
        [StringLength(255)] 
        public string ProductPropertiesValueName { get; set; }
        public int? ProductPropertiesValueId { get; set; }
        [StringLength(255)] 
        public string ProductPropertiesValueNonName { get; set; }

        [ForeignKey("OrderProductId")]
        [InverseProperty("OrderProductSimilarProperty")]
        public virtual OrderProduct OrderProduct { get; set; }
    }
}