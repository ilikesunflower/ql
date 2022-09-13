using System.Collections.Generic;
using CMS_EF.Models.Orders;

namespace CMS_App_Api.Areas.Orders.Models;

public class OrderEditApiModel
{
    public List<OrderProduct> ListOrderProductDelete { get; set; }
    public List<OrderProduct> ListOrderProductCreate { get; set; }
    public List<OrderProduct> ListOrderProductUpdate { get; set; }
    public CMS_EF.Models.Orders.Orders Order { get; set; }
    public OrderAddress OrderAddress { get; set; }
    
    public bool IsChangeProduct { get; set; }
    public bool IsChangePoi { get; set; }
}