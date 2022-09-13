#nullable disable
using CMS_EF.Models.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.PreOrders;

namespace CMS_EF.Models.Products
{
    public partial class Products
    {
        public Products()
        {
            PreOrder = new HashSet<PreOrder>();
            OrderProduct = new HashSet<OrderProduct>();
            OrderProductRateComment = new HashSet<OrderProductRateComment>();
            ProductCategoryProduct = new HashSet<ProductCategoryProduct>();
            ProductImage = new HashSet<ProductImage>();
            ProductProperties = new HashSet<ProductProperties>();
            ProductSimilar = new HashSet<ProductSimilar>();
        }

        [Key]
        public int Id { get; set; }
        [Column("SKU")]
        [StringLength(255)]
        public string Sku { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [Column("QuantityWH")]
        public int? QuantityWh { get; set; }
        public double? Weight { get; set; }
        public int? PriceSale { get; set; }
        // public int? Price { get; set; }
        [Column(TypeName = "ntext")]
        public string Description { get; set; }
        public bool? IsHot { get; set; }
        public bool? IsBestSale { get; set; }
        public bool? IsPromotion { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public int Flag { get; set; }
        public int? ProductGroupId { get; set; }
        public int? ProductPurposeId { get; set; }
        public int? ProductSex { get; set; }
        public int? ProductAge { get; set; }
        public bool? IsPublic { get; set; }
        [Column("unit")]
        [StringLength(255)]
        public string Unit { get; set; }
        [StringLength(255)]
        public string Image { get; set; }
        [Column(TypeName = "ntext")]
        public string Specifications { get; set; }
        public bool? IsNew { get; set; }
        [Column(TypeName = "ntext")]
        public string Lead { get; set; }
        public double? Rate { get; set; }
        public double? RateCount { get; set; }
        public int? TotalComment { get; set; }
        
        public int? Org1Status { get; set; }
        public int? Org2Status { get; set; }
        public int? Org3Status { get; set; }
        [Column(TypeName = "ntext")]
        public string Org1Comment { get; set; }
        
        [Column(TypeName = "ntext")]
        public string Org2Comment { get; set; }
        
        [Column(TypeName = "ntext")]
        public string Org3Comment { get; set; }

        [ForeignKey("ProductGroupId")]
        [InverseProperty("Products")]
        public virtual ProductGroup ProductGroup { get; set; }
        [ForeignKey("ProductPurposeId")]
        [InverseProperty("Products")]
        public virtual ProductPurpose ProductPurpose { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<OrderProduct> OrderProduct { get; set; }
        
        [InverseProperty("Product")]
        public virtual ICollection<OrderProductRateComment> OrderProductRateComment { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<ProductCategoryProduct> ProductCategoryProduct { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<ProductImage> ProductImage { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<ProductProperties> ProductProperties { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<ProductSimilar> ProductSimilar { get; set; }
        
        [InverseProperty("Product")]
        public virtual ICollection<PreOrder> PreOrder { get; set; }
    }
}