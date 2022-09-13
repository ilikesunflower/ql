namespace CMS.Areas.Customer.Models.Customer;

public class IndexViewModel
{
    public ReflectionIT.Mvc.Paging.PagingList<CMS_EF.Models.Customers.Customer> ListData { set; get; }

    public bool IsDelete { get; set; }
    public bool IsEdit { get; set; }
    public bool IsExportFile { get; set; }
    public bool IsImportFile { get; set; }
}