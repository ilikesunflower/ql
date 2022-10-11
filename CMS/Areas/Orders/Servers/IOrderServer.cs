using System;
using System.Collections.Generic;
using CMS_Access.Repositories.Products;
using CMS_EF.Models.Products;
using CMS_Lib.DI;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using CMS.Areas.Orders.Models;
using CMS.Models;
using CMS_Access.Repositories.Categories;
using CMS_Access.Repositories.Orders;
using CMS_EF.Models.Customers;
using CMS_EF.Models.Orders;
using CMS_Lib.Helpers;
using CMS_Lib.Services.HttpContext;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CMS.Areas.Orders.Servers;

public interface IOrderServer  : IScoped
{
    ProductCartModel GetValueCart(int id, int quantity, int order, int? price);
    List<ProductCartModel> GetListOrderProduct(int id);
    ResultJson CreateOrders(CMS_EF.Models.Orders.Orders orders);
    ResultJson EditOrders(OrderEditApiModel data );
    OrderAddress? CreateOrderAddress(int customerId,CreateOrderModel model);   

    CMS_EF.Models.Orders.Orders GetOrder(string code);
    CMS_EF.Models.Orders.Orders GetOrderById(int id);
    string GetShip(CMS_EF.Models.Orders.Orders order);
    IQueryable<CMS_EF.Models.Orders.Orders> GetOrderAll();
    string GetAddress(OrderAddress orderAddress);
    dynamic GetPriceWeight(int idS);
}

public class OrderServer : IOrderServer
{
    private readonly ILogger<OrderServer> _iLogger;
    private readonly IProductRepository _iProductRepository;
    private readonly IProductSimilarRepository _iProductSimilarRepository;
    private readonly IProvinceRepository _iProvinceRepository;
    private readonly IDistrictRepository _iDistrictRepository;
    private readonly ICommuneRepository _iCommuneRepository;
    private readonly IOrdersRepository _iOrdersRepository;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextService _httpContextService;
    private readonly IShipmentService _ShipmentService;
    private readonly IOrderProductRepository _iOrderProductRepository;

    public OrderServer(ILogger<OrderServer> iLogger, IProductRepository iProductRepository, IProductSimilarRepository iProductSimilarRepository,
        IConfiguration iConfiguration, IHttpContextService iHttpContextService, IOrdersRepository iOrdersRepository, IShipmentService iShipmentService,
        IProvinceRepository iProvinceRepository, IDistrictRepository iDistrictRepository, ICommuneRepository iCommuneRepository,
        IOrderProductRepository iOrderProductRepository)
    {
        _iLogger = iLogger;
        _iProductRepository = iProductRepository;
        _iProductSimilarRepository = iProductSimilarRepository;
        _iProductSimilarRepository = iProductSimilarRepository;
        _configuration = iConfiguration;
        _httpContextService = iHttpContextService;
        _iOrdersRepository = iOrdersRepository;
        _ShipmentService = iShipmentService;
        _iProvinceRepository = iProvinceRepository;
        _iDistrictRepository = iDistrictRepository;
        _iCommuneRepository = iCommuneRepository;
        _iOrderProductRepository = iOrderProductRepository;
    }

    public List<ProductCartModel> GetListOrderProduct(int id)
    {
        var data = _iOrderProductRepository.FindAll().Where(x => x.OrderId == id)
            .Include(x => x.Product)
            .Include(x => x.OrderProductSimilarProperty).ToList();
        var rs = new List<ProductCartModel>();
        int i = 0;
        foreach (var item in data)
        {
            var similar = _iProductSimilarRepository.FindById(item.ProductSimilarId!.Value);
            var properties = new List<PropertiesModel>();
           
                foreach (var item1 in item.OrderProductSimilarProperty)
                {
                    PropertiesModel check = new PropertiesModel()
                    {
                        PropertiesValueId = item1.ProductPropertiesValueId.Value,
                        PropertiesName = item1.ProductPropertiesName,
                        PropertiesValueName = item1.ProductPropertiesValueName
                    };
                    properties.Add(check);
            }

                var pc = new ProductCartModel()
                {
                    ProductId = item.Product.Id,
                    Image = item.Product.Image,
                    NameProduct = item.Product.Name,
                    ProductSimilarId = item.ProductSimilarId!.Value,
                    QuantityWH = similar != null ? similar.QuantityWh : 0,
                    Price = item.Price,
                    PriceNew = similar != null ? similar.Price : 0,
                    QuantityBy = item.Quantity!.Value,
                    QuantityByOld = item.Quantity!.Value,
                    ListProperties = properties,
                    Ord = i,
                    Weight = item.Weight == null ? 0 : item.Weight,
                    WeightNew = similar != null ? item.Product.Weight : 0,
                    Old = true,
                };
                if (pc.Price != pc.PriceNew && pc.Weight != pc.WeightNew)
                {
                    pc.Change = true;
                }
                rs.Add(pc);
                i++;
        }

        return rs;
    }
    public ProductCartModel GetValueCart(int id,  int quantity, int order, int? price)
    {
        var data = _iProductSimilarRepository.FindAll().Where(x => x.Id == id)
            .Include(x => x.Product)
            .Include(x => x.ProductSimilarProperty.Where(x => x.Flag == 0))
            .ThenInclude(k => k.ProductPropertiesValue)
            .ThenInclude(k => k.ProductProperties)
            .FirstOrDefault();
        ProductCartModel newPr = new ProductCartModel();
        if (data != null)
        {
            var listProperties = new List<PropertiesModel>();
            foreach (var item1 in data.ProductSimilarProperty)
            {
                PropertiesModel check = new PropertiesModel()
                {
                    PropertiesValueId = item1.ProductPropertiesValueId.Value,
                    PropertiesName = item1.ProductPropertiesValue?.ProductProperties?.Name ?? "",
                    PropertiesValueName = item1.ProductPropertiesValue.Value
                };
                listProperties.Add(check);
            }
            newPr = new ProductCartModel()
            {
                ProductId = data.Product.Id,
                Image = data.Product.Image,
                NameProduct = data.Product.Name,
                ProductSimilarId = data.Id,
                QuantityWH = data.QuantityWh,
                Price = price == null ? data.Price : data.Price,
                QuantityBy = quantity,
                ListProperties = listProperties,
                Ord = order,
                Weight =  data.Product.Weight
            };
        }

        return newPr;
    }
    public ResultJson CreateOrders(CMS_EF.Models.Orders.Orders orders)
    {
        try
        {
            string host = _configuration["OrderService:Host"];
            string endpoint = _configuration["OrderService:Endpoint"];
            Dictionary<string, string> headers = new();
            var response = _httpContextService.PostJsonAsync(new HttpClient(), host + endpoint, orders, headers).Result;
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    var  rs = JsonConvert.DeserializeObject<OutputObject>(json)!.Show();
                    return JsonConvert.DeserializeObject<OutputObject>(json)!.Show();
                }
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                this._iLogger.LogError($"Tạo đơn hàng thất bại : {res}");
            }
            return new OutputObject(400, new{IsReload = true}, "Tạo đơn hàng thất bại, vui lòng thử lại").Show();
        }
        catch (Exception e)
        {
            return new OutputObject(400, new{}, e.Message,e.Message).Show();
        }
    }
    public ResultJson EditOrders(OrderEditApiModel orders)
    {
        try
        {
            string host = _configuration["OrderService:Host"];
            string endpoint = _configuration["OrderService:Endpoint"];
            Dictionary<string, string> headers = new();
            var response = _httpContextService.PutJsonAsync(new HttpClient(), host + endpoint, orders, headers).Result;
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    var  rs = JsonConvert.DeserializeObject<OutputObject>(json)!.Show();
                    return JsonConvert.DeserializeObject<OutputObject>(json)!.Show();
                } 
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                this._iLogger.LogError($"Sửa đơn hàng lỗi: {res}");
            }
        
            return new OutputObject(400, new{IsReload = true}, "Sửa đơn hàng lỗi hãy thử lại").Show();
           
        }
        catch (Exception e)
        {
            return new OutputObject(400, new{}, e.Message,e.Message).Show();
        }
    }
    public OrderAddress? CreateOrderAddress(int customerId,CreateOrderModel model)
    {
        return new OrderAddress
        {
            Address = model!.Address,
            Name = model.Name,
            Email = model.Email,
            Flag = 0,
            Note = model.Note ?? "",
            Phone = model.Phone,
            CommuneCode = model.CommuneCode,
            DistrictCode = model.DistrictCode,
            ProvinceCode = model.ProvinceCode,
            LastModifiedAt = DateTime.Now
        };
    }


    
    public CMS_EF.Models.Orders.Orders GetOrder(string code)
    {
        var order = _iOrdersRepository.FindAll().Where(x => x.Code == code)
            .Include(x => x.Customer)
            .Include(x => x.OrderAddress)
            .Include(x => x.OrderProduct.Where(x => x.Flag == 0))
            .ThenInclude(x => x.OrderProductSimilarProperty)
            .FirstOrDefault();

        return order;
    }

    public CMS_EF.Models.Orders.Orders GetOrderById(int  id)
    {
        var order = _iOrdersRepository.FindAll().Where(x => x.Id == id)
            .Include(x => x.Customer)
            .Include(x => x.OrderAddress)
            .Include(x => x.OrderProduct.Where(k => k.Flag == 0))
            .ThenInclude(x => x.OrderProductSimilarProperty)
            .FirstOrDefault();

        return order;
    }

    public string GetShip(CMS_EF.Models.Orders.Orders order)
    {
        var rs = "";
        var listIdProduct = order.OrderProduct.Select( x=> x.ProductId).ToList();
        var weight = _iProductRepository.FindAll().Where(x => listIdProduct.Contains(x.Id)).Sum(x => x.Weight) ?? 0;
        if (order.OrderAddress?.ProvinceCode == null || order.OrderAddress?.DistrictCode == null ||
            order.OrderAddress?.CommuneCode == null)
        {
            rs = "Nhận hàng tại kho";
        }
        else
        {
           var shipList =  _ShipmentService.GetShipmentCost(order.OrderAddress?.ProvinceCode, order.OrderAddress?.DistrictCode,
                order.OrderAddress?.CommuneCode, (int) weight);
           var shipPartner = shipList.ShipmentPartners.Where(x => x.Type == order.ShipPartner.Value).FirstOrDefault();
           if (shipPartner != null)
           {
               rs += shipPartner.Name ?? "";
               var shipT = shipPartner.ShipmentTypes.Where(x => x.Type == order.ShipType.Value).FirstOrDefault();
               if (shipT != null)
               {
                   rs += " ( " + shipT.Name + " )";
               }
           }

        }

        return rs;
    }

    public IQueryable<CMS_EF.Models.Orders.Orders> GetOrderAll()
    {
        var data = _iOrdersRepository.FindAll()
            .Include(x => x.Customer)
            .Include(x => x.OrderAddress)
            .Include(x => x.OrderAddress.Province)
            .Include(x => x.OrderAddress.District)
            .Include(x => x.OrderAddress.Commune);
        return data;
    }

    public string GetAddress(OrderAddress orderAddress)
    {
        if (orderAddress == null)
        {
            return "";
        }
        var provice = _iProvinceRepository.FindByCode(orderAddress.ProvinceCode);
        var district = _iDistrictRepository.FindByCode(orderAddress.DistrictCode);
        var comu = _iCommuneRepository.FindByCode(orderAddress.CommuneCode);
        var text =
            $"Xã/Phường: {comu?.Name ?? ""}, Quận/Huyện: {district?.Name ?? ""}, Tỉnh/Thành phố: {provice?.Name ?? ""}";
        return text;
    }

    public dynamic GetPriceWeight(int idS)
    {
        var rs = _iProductSimilarRepository.FindAll().Where(x => x.Id == idS)
            .Include(x => x.Product).Select(x => new
            {
                Price = x.Price,
                Weight = x.Product.Weight
            }).FirstOrDefault();
        return rs;
    }
}
