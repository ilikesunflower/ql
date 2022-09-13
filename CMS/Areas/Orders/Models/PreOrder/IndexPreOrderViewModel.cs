using System.Collections.Generic;
using CMS.Areas.Orders.Const;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Orders.Models.PreOrder;

public class IndexPreOrderViewModel
{
    public PagingList<CMS_EF.Models.PreOrders.PreOrder> ListData { get; set; }
    public List<PreOrderStatus> PreOrderStatuses { get; set; }
}