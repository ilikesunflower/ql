using System;
using System.Collections.Generic;
using System.Linq;
using CMS_Access.Repositories.Customers;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using CMS.Areas.Customer.Const;
using CMS.Areas.Reports.Models.CustomerActivity;

namespace CMS.Areas.Reports.Services;

public interface ICustomerActivityService : IScoped
{
    List<IndexCustomerType> GetTypeCustomerActive(string txtSearch, DateTime startDate, DateTime endDate, int? type);
    List<TrackingOfCustomer> GetTypeCustomerActiveDetails(string txtSearch, DateTime start, DateTime end, int? type);
    List<IndexViewModelCustomerTypeChart> GetTypeCustomerActiveChart(DateTime start, DateTime end);
}

public class CustomerActivityService : ICustomerActivityService
{
    private readonly ICustomerTrackingRepository _iCustomerTrackingRepository;

    public CustomerActivityService(ICustomerTrackingRepository iCustomerTrackingRepository)
    {
        _iCustomerTrackingRepository = iCustomerTrackingRepository;
    }

    public  List<IndexCustomerType> GetTypeCustomerActive(string txtSearch, DateTime startDate, DateTime endDate,
        int? type)
    {
        List<NumberOfCustomerGroups> numberOfCustomerGroups =
            _iCustomerTrackingRepository.GetNumberOfCustomerGroups(txtSearch, startDate, endDate, type);
        List<IndexCustomerType> rs = new List<IndexCustomerType>();
        foreach (var item in CustomerTypeGroupConst.ListCustomerTypeGroupConst)
        {
            IndexCustomerType indexCustomerType = new IndexCustomerType()
            {
                Name = item.Value,
                Type = item.Key,
                Value = numberOfCustomerGroups.FirstOrDefault(x => x.Type == item.Key)?.Count ?? 0,
            };
            rs.Add(indexCustomerType);
        }
   
        return rs;
    }

    public List<TrackingOfCustomer> GetTypeCustomerActiveDetails(string txtSearch, DateTime start, DateTime end, int? type)
    {
        List<TrackingOfCustomer> numberOfCustomerGroups =  _iCustomerTrackingRepository.GetTypeCustomerActiveDetails(txtSearch, start,end, type).OrderByDescending(x => x.ActiveTime).ToList();
        return numberOfCustomerGroups.Select(x => new TrackingOfCustomer
        {
            Id = x.Id,
            FullName = x.FullName,
            Username = x.Username,
            Org = BindIdOrg(x.TypeGroup,x.Org),
            ActiveTime = x.ActiveTime,
            TypeGroup = x.TypeGroup
        }).ToList();
    }

    public List<IndexViewModelCustomerTypeChart> GetTypeCustomerActiveChart(DateTime start, DateTime end)
    {
        List<IndexViewModelCustomerTypeChart> charts = new List<IndexViewModelCustomerTypeChart>();
        List<NumberOfCustomerGroups> numberOfCustomerGroups = _iCustomerTrackingRepository.GetNumberOfCustomerGroups("", start, end, null);

        foreach (var item in CustomerTypeGroupConst.ListCustomerTypeGroupConst)
        {
            IndexViewModelCustomerTypeChart indexCustomerType = new IndexViewModelCustomerTypeChart()
            {
                Name = item.Value,
                Y = numberOfCustomerGroups.FirstOrDefault(x => x.Type == item.Key)?.Count ?? 0,
            };
            charts.Add(indexCustomerType);
        }
     
        return charts;
    }

    public static string BindIdOrg(int? typeGroup, string org)
    {
        return typeGroup == CustomerTypeGroupConst.PhongBan
            ? org
            : CustomerTypeGroupConst.GetCustomerTypeGroup(typeGroup ?? 0);
    }
}