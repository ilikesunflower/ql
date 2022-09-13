using CMS_EF.Models.Customers;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.PointInput.Models.PointInputs;

public class DetailsPointViewModel
{
    public HistoryFileChargePoint File { get; set; }
    public PagingList<CustomerPoint> ListPoint { get; set; }
    public bool IsSendNotification { get; set; }
}