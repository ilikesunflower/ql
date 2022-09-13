using System.Collections.Generic;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Reports.Models.SummaryReports;

public class IndexViewModel
{
    public PagingList<CMS_EF.Models.Orders.Orders> ListData { get; set; }
    public Dictionary<int,string> OrderStatusPayments { get; set; }
    public Dictionary<int,string> ListStatus { get; set; }
}