using System.Collections.Generic;
using CMS_EF.Models.Orders;

namespace CMS.Areas.OrderComment.Models;

public  class OrderCommentIndex
{
    public string OrderCode { get; set; }
    public List<OrderCommentDetail> ListOrderCommentDetail { get; set; }
    
}
public  class OrderCommentDetail
{
    public CMS_EF.Models.Products.Products Product { get; set; }
    public OrderProductSimilarProperty OrderSimilar { get; set; }
    public OrderProductRateComment OrderProductRateComment { get; set; }
    public string CustomerName { get; set; }
 
}
