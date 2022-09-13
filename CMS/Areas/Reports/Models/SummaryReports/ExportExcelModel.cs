using System;

namespace CMS.Areas.Reports.Models.SummaryReports;

public class ExportExcelModel
{
    public string Code { set; get; }
    public string OrderAt { set; get; }
    public string OrderStatusConfirmAt { set; get; }
    public string PaymentType { set; get; }
    public string StatusPayment { set; get; }
    public string Status { set; get; }
    public string ShipPartner { set; get; }
    public string CodeShip { set; get; }
    public string ProductName { set; get; }
    public double Quantity { set; get; }
    public double Price { set; get; }
    public double PriceShip { set; get; }
    public string UserName { set; get; }
    public string Org { set; get; }
    public string Name { set; get; }
    public string Phone { set; get; }
    public string Email { set; get; }
    public string Address { set; get; }
    public string AddressNote { set; get; }
    public string Note { set; get; }
    public DateTime? OrderAtDate { get; set; }
}