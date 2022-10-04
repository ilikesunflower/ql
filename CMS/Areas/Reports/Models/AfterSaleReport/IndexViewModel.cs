using System;
using System.Collections.Generic;
using CMS_Access.Repositories.Customers;
using CMS_EF.Models.Reports;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Reports.Models.AfterSaleReport;

public class IndexViewModel
{
    public int Page { get; set; }
    public PagingList<ReportAfterSales> ListData { get; set; }
}
