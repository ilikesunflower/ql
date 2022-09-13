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
    IndexViewModelCustomerType GetTypeCustomerActive(string txtSearch, DateTime startDate, DateTime endDate, int? type);
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

    public IndexViewModelCustomerType GetTypeCustomerActive(string txtSearch, DateTime startDate, DateTime endDate,
        int? type)
    {
        List<NumberOfCustomerGroups> numberOfCustomerGroups =
            _iCustomerTrackingRepository.GetNumberOfCustomerGroups(txtSearch, startDate, endDate, type);
        IndexViewModelCustomerType customerType = new IndexViewModelCustomerType
        {
            Org = numberOfCustomerGroups.FirstOrDefault(x => x.Type == CustomerTypeGroupConst.PhongBan)?.Count ?? 0,
            Staff = numberOfCustomerGroups.FirstOrDefault(x => x.Type == CustomerTypeGroupConst.Staff)?.Count ?? 0,
            GA = numberOfCustomerGroups.FirstOrDefault(x => x.Type == CustomerTypeGroupConst.GA)?.Count ?? 0,
        };
        return customerType;
    }

    public List<TrackingOfCustomer> GetTypeCustomerActiveDetails(string txtSearch, DateTime start, DateTime end, int? type)
    {
        List<TrackingOfCustomer> numberOfCustomerGroups =  _iCustomerTrackingRepository.GetTypeCustomerActiveDetails(txtSearch, start,end, type).ToList();
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
        NumberOfCustomerGroups org = numberOfCustomerGroups.FirstOrDefault(x => x.Type == CustomerTypeGroupConst.PhongBan);
        NumberOfCustomerGroups staff = numberOfCustomerGroups.FirstOrDefault(x => x.Type == CustomerTypeGroupConst.Staff);
        NumberOfCustomerGroups ga = numberOfCustomerGroups.FirstOrDefault(x => x.Type == CustomerTypeGroupConst.GA);

        charts.Add(new IndexViewModelCustomerTypeChart()
        {
            Name = "Phòng ban",
            Y = org?.Count ?? 0
        });

        charts.Add(new IndexViewModelCustomerTypeChart()
        {
            Name = "Staff",
            Y = staff?.Count ?? 0
        });
        
        charts.Add(new IndexViewModelCustomerTypeChart()
        {
            Name = "GA",
            Y = ga?.Count ?? 0
        });
        return charts;
    }

    public static string BindIdOrg(int? typeGroup, string org)
    {
        return typeGroup == CustomerTypeGroupConst.PhongBan
            ? org
            : CustomerTypeGroupConst.GetCustomerTypeGroup(typeGroup ?? 0);
    }
}