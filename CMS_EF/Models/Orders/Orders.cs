#nullable disable
using CMS_EF.Models.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Orders
{
    public partial class Orders
    {
        [Key]
        public int Id { get; set; }
        public int? CustomerId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? OrderAt { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        public int? Status { get; set; }

        public double? Total { get; set; }
        public double? Price { get; set; }
        public double? Point { get; set; }

        public double? CouponDiscount { get; set; }
        public string CouponCode { get; set; }
        public string ReasonNote { get; set; }
        public double? PriceShip { get; set; }
        public double? PriceShipNonSale { get; set; }
        public double? PriceShipSalePercent { get; set; }

        // [StringLength(255)]
        // public string PartnerShipAlias { get; set; }
        public int? ShipType { get; set; }

        public int? AddressType { get; set; }
        public int? ShipPartner { get; set; }
        public int? PaymentType { get; set; }
        public string PrCode { get; set; }
        public string PrFile { get; set; }
        public string BillCompanyName { get; set; }
        public string BillAddress { get; set; }
        public string BillTaxCode { get; set; }
        public string BillEmail { get; set; }
        public int? OrderIdWh { get; set; }
        public bool? StatusRateComment { get; set; }
        public int? StatusPayment { get; set; }

        [Column(TypeName = "ntext")]
        public string Note { get; set; }

        [StringLength(255)]
        public string CodeShip { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? OrderStatusConfirmAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? OrderStatusSuccessAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? OrderStatusCancelAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? OrderStatusShipAt { get; set; }

        public int Flag { get; set; }
        public double? CodAmountEvaluation { get; set; }
        public double? TotalWeight { get; set; }
        public double? PointDiscount { get; set; }
        public string ShipCodeIdVnPost { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty("Orders")]
        public virtual Customer Customer { get; set; }


        [InverseProperty("Order")]
        public virtual OrderAddress OrderAddress { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<OrderLog> OrderLog { get; set; } = new HashSet<OrderLog>();

        [InverseProperty("Order")]
        public virtual ICollection<OrderProduct> OrderProduct { get; set; } = new HashSet<OrderProduct>();

        [InverseProperty("Order")]
        public virtual ICollection<OrderPoint> OrderPoint { get; set; }

    }
}