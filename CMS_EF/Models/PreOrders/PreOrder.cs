#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.Customers;
using CMS_EF.Models.Products;

namespace CMS_EF.Models.PreOrders
{
    public class PreOrder
    {
        
        [Key] 
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? OrderCode { get; set; }
        public int? ProductId { get; set; }
        public int? ProductSimilarId { get; set; }
        public int? Quantity { get; set; }
        public int? ShipPartner { get; set; }
        public int? ShipType { get; set; }
        public double? PriceShip { get; set; }
        public int? PaymentType { get; set; }
        public int? Status { get; set; }
        public DateTime? PreOrderAt { get; set; }
        public int Flag { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string BillCompanyName { get; set; }
        public string BillAddress { get; set; }
        public string BillTaxCode { get; set; }
        public string BillEmail { get; set; }
        
        public string PrCode { get; set; }
        public string PrFile { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty("PreOrder")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("ProductId")]
        [InverseProperty("PreOrder")]
        public virtual Products.Products Product { get; set; }
        [ForeignKey("ProductSimilarId")]
        [InverseProperty("PreOrder")]
        public virtual ProductSimilar ProductSimilar { get; set; }
        [InverseProperty("PreOrder")]
        public virtual PreOrderAddress PreOrderAddress { get; set; }


    }
}