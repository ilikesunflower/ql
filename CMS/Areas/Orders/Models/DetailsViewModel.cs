using System.Collections.Generic;
using CMS_EF.Models.Orders;

namespace CMS.Areas.Orders.Models;

public class DetailsViewModel
{
    public CMS_EF.Models.Orders.Orders Order { get; set; }
    
    public List<OrderPartnerShipLog> ListOrderPartnerShipLog { get; set; }
    public string Ship { get; set; }
    
    public string Payment { get; set; }
    
    public string Address { get; set; }
    
    public bool IsEdit { get; set; }
    public bool IsOrderConfirm { get; set; }
    
    public bool IsOrderShip { get; set; }
    
    public bool IsOrderSuccess { get; set; }
    
    public bool IsOrderCancel { get; set; }
    
    public bool IsStatusShowAll { get; set; }
    public bool IsStatusPayment { get; set; }
    public double IsPoi { get; set; }
}