using System;
using System.Collections.Generic;
using CMS_Access.Repositories.Customers;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Reports.Models.CustomerActivity;

public class IndexViewModel
{
    public List<IndexCustomerType> CustomerTypeList { set; get; }
    public PagingList<TrackingOfCustomer> Customers { get; set; }
}


public class IndexCustomerType
{
    public string Name { get; set; }
    public double Value { get; set; }
    public int Type { get; set; }
}
public class IndexViewModelCustomerTypeChart
{
    public string Name { set; get; }
    public double Y { set; get; }
}
