using System.Collections.Generic;
using CMS_EF.Models.Orders;
using Novacode;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.OrderComment.Models;

public class CommentIndexModel
{
    public PagingList<OrderProductRateComment> ListData { get; set; }
    public int Page { get; set; }
    public List<CommnentOrder> ListOrder { get; set; }
}

public class CommnentOrder
{
    public string Code { get; set; }
    public int Count { get; set; }
}