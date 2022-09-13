#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.Customers;

namespace CMS_EF.Models.Orders
{
    public partial class OrderProductRateComment
    {
        [Key] public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? ProductSimilarId { get; set; }
        public int? CustomerId { get; set; }

        public int? Rate { get; set; }
        public int? OrderId { get; set; }
        public bool? Status { get; set; }
        public int Flag { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? OrderProductId { get; set; }
        public string CommentDefault { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("OrderProductId")]
        [InverseProperty("OrderProductRateComment")]
        public virtual OrderProduct OrderProduct { get; set; } 
        
        [ForeignKey("OrderId")]
        public virtual Orders  Orders{ get; set; }
        
        [ForeignKey("ProductId")]
        [InverseProperty("OrderProductRateComment")]
        public virtual Products.Products Product { get; set; }
        
        [ForeignKey("CustomerId")]
        [InverseProperty("OrderProductRateComment")]
        public virtual Customer Customer { get; set; }
    }
}