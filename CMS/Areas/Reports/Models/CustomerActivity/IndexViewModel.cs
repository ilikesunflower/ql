using System;
using System.Collections.Generic;
using CMS_Access.Repositories.Customers;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Reports.Models.CustomerActivity;

public class IndexViewModel
{
    public IndexViewModelCustomerType CustomerType { set; get; }
    public PagingList<TrackingOfCustomer> Customers { get; set; }
}
public class IndexViewModelCustomerType
{
    public double Staff { set; get; }
    public double GA { set; get; }
    public double Org { set; get; }
}
public class IndexViewModelCustomerTypeChart
{
    public string Name { set; get; }
    public double Y { set; get; }
}
