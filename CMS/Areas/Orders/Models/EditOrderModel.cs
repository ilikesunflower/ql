using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace CMS.Areas.Orders.Models;

public class EditOrderModel
{
    public List<ProductCheckoutViewmodel>? Products { get; set; }
    public int OrderId { get; set; }  
    public bool CheckChangeP { get; set; }  
    public bool CheckChangePoi { get; set; }  
    public double? PriceShip { get; set; }
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
    public double? TotalWeight { get; set; }
    public string? Note { get; set; }
    public int? ShipPartner { get; set; }
    public int? ShipType { get; set; }
    public int? PaymentType { get; set; }
    public string? BillCompanyName { get; set; }
    public string? BillAddress { get; set; }
    public string? BillTaxCode { get; set; }
    public string? BillEmail { get; set; }
    public string? PrCode { get; set; }
    public IFormFile? PrFile { get; set; }
}

