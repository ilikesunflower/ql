using System;
using System.Collections.Generic;
using System.Linq;
using CMS.Areas.Orders.Models;
using CMS_Access.Repositories.Categories;
using CMS_Access.Repositories.Customers;
using CMS_Lib.DI;
using CMS_Lib.Helpers;
using CMS_Ship.GHN;
using CMS_Ship.Models;
using CMS_Ship.VnPost;

namespace CMS.Areas.Orders.Servers;

public interface IShipmentService : IScoped
{
    ShipmentViewModel GetShipmentCost(int customerAddressId, int weight);
    ShipmentViewModel GetShipmentCost(string provinceCode,string districtCode,string communeCode,int weight);
}

public class ShipmentService : IShipmentService
{
    private readonly IProvinceRepository _provinceRepository;
    private readonly IDistrictRepository _districtRepository;
    private readonly ICommuneRepository _communeRepository;
    private readonly IVnPostService _vnPostService;
    private readonly IGhnService _ghnService;
    private readonly ICustomerAddressRepository _customerAddressRepository;
    
    public ShipmentService(IProvinceRepository provinceRepository, IDistrictRepository districtRepository, ICommuneRepository communeRepository, IVnPostService vnPostService, IGhnService ghnService, ICustomerAddressRepository customerAddressRepository)
    {
        _provinceRepository = provinceRepository;
        _districtRepository = districtRepository;
        _communeRepository = communeRepository;
        _vnPostService = vnPostService;
        _ghnService = ghnService;
        _customerAddressRepository = customerAddressRepository;
    }
 
    public ShipmentViewModel GetShipmentCost(int customerAddressId, int weight)
    {
        var customerAddress = _customerAddressRepository.GetValueCustomerAddress(customerAddressId);
        var province = customerAddress.ProvinceCodeNavigation;
        var district = customerAddress.DistrictCodeNavigation;
        var commune =  customerAddress.CommuneCodeNavigation;
        if (province == null || district == null || commune == null)
        {
            throw new Exception("Không tìm thấy địa chỉ");
        }
        List<CalculateFee> ghnCost = _ghnService.CalculateFee( IntegerHelper.ParseStringToInt(district.DistrictGhnId)!.Value,commune.CommuneGhnId, weight);
        List<CalculateFee> vnPostCost  = _vnPostService.CalculateFee(province.ProvinceVnPostId,district.DistrictVnPostId,weight);

        return new ShipmentViewModel(ghnCost,vnPostCost);
    }

    public ShipmentViewModel GetShipmentCost(string provinceCode, string districtCode, string communeCode,int weight)
    {
        var checkAddress = _communeRepository.FindAll().Where(x =>
            x.Code == communeCode && x.DistrictCode == districtCode && x.ProvinceCode == provinceCode).ToList();
        if (checkAddress.Count == 0)
        {
            throw new Exception("Không tìm thấy địa chỉ");
        }
        var province = _provinceRepository.FindByCode(provinceCode);
        var district = _districtRepository.FindByCode(districtCode);
        var commune = _communeRepository.FindByCode(communeCode);
        if (province == null || district == null || commune == null)
        {
            throw new Exception("Không tìm thấy địa chỉ");
        }

        List<CalculateFee> ghnCost = _ghnService.CalculateFee(IntegerHelper.ParseStringToInt(district.DistrictGhnId)!.Value, commune.CommuneGhnId, weight);
        List<CalculateFee> vnPostCost  = _vnPostService.CalculateFee(province.ProvinceVnPostId,district.DistrictVnPostId,weight);

        return new ShipmentViewModel(ghnCost,vnPostCost);
    }

}
