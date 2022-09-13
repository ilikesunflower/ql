#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.Customers;

namespace CMS_EF.Models.Orders
{
    public partial class OrderProductComment
    {
        [Key] public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? ProductSimilarId { get; set; }
        public int? CustomerId  { get; set; }
        [Column(TypeName = "ntext")] public string Comment { get; set; }
        public int? Status { get; set; }
        public int Flag { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? OrderProductId { get; set; }
        public int? OrderId { get; set; }

        [ForeignKey("OrderProductId")]
        [InverseProperty("OrderProductComment")]
        public virtual OrderProduct OrderProduct { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("OrderProductComment")]
        public virtual Products.Products Product { get; set; }
        
        [ForeignKey("CustomerId")]
        [InverseProperty("OrderProductComment")]
        public virtual Customer Customer { get; set; }
    }
}