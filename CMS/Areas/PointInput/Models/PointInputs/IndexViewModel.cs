using CMS_EF.Models.Customers;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.PointInput.Models.PointInputs;

public class IndexViewModel
{
    public string Title { get; set; }
    public PagingList<HistoryFileChargePoint> ListData { get; set; }
    public bool IsUploadFile { get; set; }
}