using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace CMS.Areas.Orders.Models;

public class CreateOrderModel
{
    public List<ProductCheckoutViewmodel>? Products { get; set; }
    public double? PriceShip { get; set; }
    public double? PriceNoSale { get; set; }
    public double? Percent { get; set; }
    public double? Total { get; set; }
    public int? CouponDiscount { get; set; }
    public int? CustomerId { get; set; }
    public string? CouponCode { get; set; }
    public int? Point { get; set; }
    public int? AddressType { get; set; }
    public string ProvinceCode { get; set; }
    public string DistrictCode { get; set; }
    public string CommuneCode { get; set; }
    public string? Address { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int? TotalWeight { get; set; }
    public string? Note { get; set; }
    public int? ShipPartner { get; set; }
    public int? ShipType { get; set; }
    public int? PaymentType { get; set; }
    public string? BillCompanyName { get; set; }
    public string? BillAddress { get; set; }
    public string? BillTaxCode { get; set; }
    public string? BillEmail { get; set; }
    public string? PrCode { get; set; }
    public string? PrFile { get; set; }
}

public class ProductCheckoutViewmodel
{
    public int ProductSimilarId { get; set; }
    public int Quantity { get; set; }
    public int? Price { get; set; }
    public int? Weight { get; set; }
}