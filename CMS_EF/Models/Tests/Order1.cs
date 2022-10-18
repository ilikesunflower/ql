#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Tests
{
    [Table("order1")]
    public partial class Order1
    {
        [Column("methodPayment")]
        [StringLength(255)]
        public string MethodPayment { get; set; }
        [Column("discount")]
        public long? Discount { get; set; }
        [Column("typeOrder")]
        [StringLength(255)]
        public string TypeOrder { get; set; }
        [Column("totalAccumulatePoints")]
        public long? TotalAccumulatePoints { get; set; }
        [Column("discountCode")]
        [StringLength(255)]
        public string DiscountCode { get; set; }
        [Column("note")]
        [StringLength(255)]
        public string Note { get; set; }
        [Column("image")]
        [StringLength(255)]
        public string Image { get; set; }
        [Column("itemCount")]
        public int? ItemCount { get; set; }
        [Column("cashTransactionId")]
        public int CashTransactionId { get; set; }
        [Column("createdAt")]
        public long? CreatedAt { get; set; }
        [Column("updatedAt")]
        public long? UpdatedAt { get; set; }
        [Key]
        [Column("id")]
        [StringLength(255)]
        public string Id { get; set; }
        [Column("totalMoney")]
        public double? TotalMoney { get; set; }
        [Column("customerId")]
        public double? CustomerId { get; set; }
        [Column("receiveName")]
        [StringLength(255)]
        public string ReceiveName { get; set; }
        [Column("receivePhone")]
        [StringLength(255)]
        public string ReceivePhone { get; set; }
        [Column("receiveEmail")]
        [StringLength(255)]
        public string ReceiveEmail { get; set; }
        [Column("provinceId")]
        public int? ProvinceId { get; set; }
        [Column("districtId")]
        public int? DistrictId { get; set; }
        [Column("wardId")]
        public int? WardId { get; set; }
        [Column("address")]
        [StringLength(255)]
        public string Address { get; set; }
        [Column("shipStorage")]
        [StringLength(255)]
        public string ShipStorage { get; set; }
        [Column("shipService")]
        [StringLength(255)]
        public string ShipService { get; set; }
        [Column("shipId")]
        public double? ShipId { get; set; }
        [Column("shipStatus")]
        [StringLength(255)]
        public string ShipStatus { get; set; }
        [Column("shipMsg")]
        [StringLength(255)]
        public string ShipMsg { get; set; }
        [Column("shipMoney")]
        public double? ShipMoney { get; set; }
        [Column("paymentStatus")]
        public string PaymentStatus { get; set; }
        [Column("moneyPaid")]
        public double? MoneyPaid { get; set; }
        [Column("pointPaid")]
        public double? PointPaid { get; set; }
        [Column("cancelReason")]
        [StringLength(255)]
        public string CancelReason { get; set; }
        [Column("linkReturnUrl")]
        [StringLength(255)]
        public string LinkReturnUrl { get; set; }
        [Column("transporterOrder")]
        [StringLength(255)]
        public string TransporterOrder { get; set; }
        [Column("shipOrderCodeId")]
        [StringLength(255)]
        public string ShipOrderCodeId { get; set; }
        [Column("codeRewardGame")]
        [StringLength(255)]
        public string CodeRewardGame { get; set; }
        [Column("status")]
        [StringLength(255)]
        public string Status { get; set; }
        [Column("orderItems")]
        public string OrderItems { get; set; }
        [Column("voucherDiscount")]
        public int? VoucherDiscount { get; set; }
        [Column("methodShip")]
        [StringLength(255)]
        public string MethodShip { get; set; }
        [Column("linkVnpPayment")]
        [StringLength(255)]
        public string LinkVnpPayment { get; set; }
        [Column("scopeCustomer")]
        [StringLength(255)]
        public string ScopeCustomer { get; set; }
        [Column("discountShip")]
        public int? DiscountShip { get; set; }
        [Column("discountProduct")]
        public int? DiscountProduct { get; set; }
        [Column("companyTaxName")]
        [StringLength(255)]
        public string CompanyTaxName { get; set; }
        [Column("adrressTax")]
        [StringLength(255)]
        public string AdrressTax { get; set; }
        [Column("taxNumber")]
        [StringLength(255)]
        public string TaxNumber { get; set; }
        [Column("receiveEmailTax")]
        [StringLength(255)]
        public string ReceiveEmailTax { get; set; }
        [Column("shipMoneyChange")]
        public int? ShipMoneyChange { get; set; }
        [Column("totalPriceProduct")]
        public int? TotalPriceProduct { get; set; }
        [Column("handler")]
        public int? Handler { get; set; }
        [Column("handleDescription")]
        public string HandleDescription { get; set; }
        [Column("successTime")]
        public long? SuccessTime { get; set; }
        [Column("refIdKiotViet")]
        [StringLength(255)]
        public string RefIdKiotViet { get; set; }
        [Column("isSyncKiotViet")]
        public int? IsSyncKiotViet { get; set; }
        [Column("refOrderIdKiotViet")]
        [StringLength(255)]
        public string RefOrderIdKiotViet { get; set; }
        [Column("refOrderCodeKiotViet")]
        [StringLength(255)]
        public string RefOrderCodeKiotViet { get; set; }
        [Column("refCodeKiotViet")]
        [StringLength(255)]
        public string RefCodeKiotViet { get; set; }
    }
}