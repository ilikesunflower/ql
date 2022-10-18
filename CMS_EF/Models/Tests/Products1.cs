using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CMS_EF.Models.Tests
{
    public partial class Products1
    {
        [Column("height")]
        public double? Height { get; set; }
        [Column("lengths")]
        public double? Lengths { get; set; }
        [Column("widths")]
        public double? Widths { get; set; }
        [Column("weight")]
        public double? Weight { get; set; }
        [Column("paymentCash")]
        public int? PaymentCash { get; set; }
        [Column("paymentPoint")]
        public int? PaymentPoint { get; set; }
        [Column("payment")]
        [StringLength(255)]
        public string Payment { get; set; }
        [Column("originPrice")]
        public int? OriginPrice { get; set; }
        [Column("createdAt")]
        public long? CreatedAt { get; set; }
        [Column("updatedAt")]
        public long? UpdatedAt { get; set; }
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("itemId")]
        public double? ItemId { get; set; }
        [Column("productTypeId")]
        public double? ProductTypeId { get; set; }
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }
        [Column("isActive")]
        public byte? IsActive { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("images")]
        public string Images { get; set; }
        [Column("isDelete")]
        public byte? IsDelete { get; set; }
        [Column("storeId")]
        public int? StoreId { get; set; }
        [Column("customerPrice")]
        public int? CustomerPrice { get; set; }
        [Column("customerDaichiPrice")]
        public int? CustomerDaichiPrice { get; set; }
        [Column("partnerPrice")]
        public int? PartnerPrice { get; set; }
        [Column("agencyPrice")]
        public int? AgencyPrice { get; set; }
        [Column("status")]
        [StringLength(255)]
        public string Status { get; set; }
        [Column("inventory")]
        public int? Inventory { get; set; }
        [Column("skuKiotViet")]
        [StringLength(255)]
        public string SkuKiotViet { get; set; }
        [Column("reserved")]
        public int? Reserved { get; set; }
        [Column("salePrice")]
        public int? SalePrice { get; set; }
    }
}