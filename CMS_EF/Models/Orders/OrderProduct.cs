#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Orders
{
    public partial class OrderProduct
    {
        public OrderProduct()
        {
            OrderProductRateComment = new HashSet<OrderProductRateComment>();
            OrderProductSimilarProperty = new HashSet<OrderProductSimilarProperty>();
        }

        [Key] public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? ProductSimilarId { get; set; }
        public double? Price { get; set; }
        public double? Weight { get; set; }
        public int? Quantity { get; set; }
        [StringLength(255)] public string ProductName { get; set; }
        [Column(TypeName = "ntext")] public string ProductImage { get; set; }
        [Column(TypeName = "datetime")] public DateTime? LastModifiedAt { get; set; }
        public int Flag { get; set; }
        public int? PriceSale { get; set; }
        
        public string ProductSimilarCodeWh { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("OrderProduct")]
        public virtual Orders Order { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("OrderProduct")]
        public virtual Products.Products Product { get; set; }
        
        [InverseProperty("OrderProduct")] public virtual ICollection<OrderProductRateComment> OrderProductRateComment { get; set; }

        [InverseProperty("OrderProduct")]
        public virtual ICollection<OrderProductSimilarProperty> OrderProductSimilarProperty { get; set; }
    }
}