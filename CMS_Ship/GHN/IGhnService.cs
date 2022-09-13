using System.Net;
using CMS_Access.Repositories.Ship;
using CMS_EF.Models.Ship;
using CMS_Lib.DI;
using CMS_Lib.Services.HttpContext;
using CMS_Lib.Util;
using CMS_Ship.Consts;
using CMS_Ship.GHN.Models;
using CMS_Ship.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CMS_Ship.GHN;

public interface IGhnService : IScoped
{
    public Account? GetInfo(int id);

    public List<CalculateFee> CalculateFee(int toDistrictId, string toWardCode, int weight,
        int insuranceValue = 0, string? coupon = null);

    public CreateOrderOutPut? CreateOrder(CreatedOrder createdOrder);

    public int CancelOrder(string orderCode);
}

public class GhnService : IGhnService
{
    private readonly ILogger<GhnService> _iLogger;
    private readonly IHttpContextService _iHttpContextService;
    private readonly IShipGhnRepository _iShipGhnRepository;
    private ShipGhn? _shipGhn;
    private Dictionary<string, string> headers;

    public GhnService(ILogger<GhnService> iLogger,
        IHttpContextService iHttpContextService, IShipGhnRepository iShipGhnRepository)
    {
        this._iLogger = iLogger;
        this._iHttpContextService = iHttpContextService;
        this._iShipGhnRepository = iShipGhnRepository;
        this._shipGhn = this._iShipGhnRepository.FindByStatus();

        headers = new Dictionary<string, string>()
        {
            // {"Content-Type", "application/json"},
            { "token", this._shipGhn!.Token }
        };
    }

    public Account? GetInfo(int id)
    {
        try
        {
            string url = $"{this._shipGhn?.PrefixApi}/v2/shop/all";
            var response = this._iHttpContextService
                .GetJsonAsync(new HttpClient(), url, headers).Result;
            response.EnsureSuccessStatusCode();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string res = response.Content.ReadAsStringAsync().Result;
                var json = JObject.Parse(res);
                var data = json.SelectToken("data.shops").OfType<JObject>().Select(o => new Account()
                {
                    ShopId = CmsFunction.ConvertToInt($"{o.SelectToken("_id")}"),
                    DistrictId = CmsFunction.ConvertToInt($"{o["district_id"]}"),
                    ClientId = CmsFunction.ConvertToInt($"{o["ClientId"]}"),
                    WardCode = $"{o["ward_code"]}",
                });
                return data.FirstOrDefault(x => x.ShopId == id);
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"Lấy thông tin tài khoản GHN lỗi: shopId: {id}");
        }

        return null;
    }

    public List<CalculateFee> CalculateFee(int toDistrictId, string toWardCode, int weight,
        int insuranceValue = 0, string? coupon = null)
    {
        if (this._shipGhn == null)
        {
            return new List<CalculateFee>();
        }

        try
        {
            List<CalculateFee> data = new List<CalculateFee>();
            // var express = CalculateFeeType(TypeShipConst.Express, toDistrictId, toWardCode, weight, insuranceValue, coupon);
            var standard = CalculateFeeType(TypeShipConst.Standard, toDistrictId, toWardCode, weight, insuranceValue, coupon);
            // if (express != null)
            // {
            //     data.Add(express);
            // }

            if (standard != null)
            {
                data.Add(standard);
            }

            return data;
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, "CalculateFee fail");
        }

        return new List<CalculateFee>();
    }

    private CalculateFee? CalculateFeeType(int typeServiceId, int toDistrictId, string toWardCode, int weight,
        int insuranceValue = 0, string? coupon = null)
    {
        try
        {
            string url = $"{this._shipGhn?.PrefixApi}/v2/shipping-order/fee";
            var response = this._iHttpContextService
                .PostJsonAsync(new HttpClient(), url, new
                {
                    from_district_id = CmsFunction.ConvertToInt(this._shipGhn?.DistrictId),
                    service_id = 0,
                    service_type_id = typeServiceId,
                    to_district_id = toDistrictId,
                    to_ward_code = toWardCode,
                    weight = weight,
                }, headers).Result;
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    var json = JObject.Parse(res);
                    var o = JObject.Parse($"{json.SelectToken("data")}");
                    if (o != null)
                    {
                        return new CalculateFee()
                        {
                            Total = CmsFunction.ConvertToDouble($"{(o["total"])}"),
                            CodeService = typeServiceId == 1 ? "express" : "standard",
                            TypeService = typeServiceId,
                        };
                    }
                }   
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                this._iLogger.LogError($"Tính toán phí vận chuyển lỗi: {res}");
            }
        }
        catch (Exception ex)
        {
            // ignored
            this._iLogger.LogError(ex,"Tính toán phí vận chuyển lỗi");
        }

        return null;
    }

    public CreateOrderOutPut? CreateOrder(CreatedOrder createdOrder)
    {
        try
        {
            string url = $"{this._shipGhn?.PrefixApi}/v2/shipping-order/create";
            var param = new
            {
                payment_type_id = createdOrder.PaymentTypeId,
                required_note = "CHOXEMHANGKHONGTHU",
                client_order_code = $"{TypeShipConst.PrefixOrder}{createdOrder.OrderCode}",
                to_name = createdOrder.ToName,
                to_phone = createdOrder.ToPhone,
                to_address = createdOrder.ToAddress,
                to_ward_code = createdOrder.ToWardCode,
                to_district_id = CmsFunction.ConvertToInt(createdOrder.ToDistrictId),
                cod_amount = createdOrder.CodAmount,
                weight = createdOrder.Weight,
                service_id = 0,
                service_type_id = 2,
                content = createdOrder.Content ?? "không có ghi chú",
                note = createdOrder.Note ?? "không có ghi chú",
                items = createdOrder.ListItem
            };
            var response = this._iHttpContextService
                .PostJsonAsync(new HttpClient(), url, param, headers).Result;
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    var json = JObject.Parse(res);
                    string orderCode = $"{json.SelectToken("data.order_code")}";
                    if (!string.IsNullOrEmpty(orderCode))
                    {
                        var totalFee = CmsFunction.ConvertToInt($"{json.SelectToken("data.total_fee")}");
                        var rs = new CreateOrderOutPut()
                        {
                            OrderCode = orderCode,
                            ExpectedDeliveryTime = $"{json.SelectToken("data.expected_delivery_time")}",
                            TotalFee = totalFee
                        };
                        return rs;
                    }
                    return null;
                }
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                var json = JObject.Parse(res);
                string message = $"{json["message"]}";
                this._iLogger.LogError( $"Tạo đơn hàng cho GHN lỗi: {createdOrder.OrderCode}: lỗi: {res}");
                return new CreateOrderOutPut()
                {
                    OrderCode = string.Empty,
                    ExpectedDeliveryTime = string.Empty,
                    Err = message
                };
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"Tạo đơn hàng cho GHN lỗi: {createdOrder.OrderCode}");
        }

        return new CreateOrderOutPut()
        {
            OrderCode = string.Empty,
            ExpectedDeliveryTime = string.Empty,
            Err = string.Empty
        };;
    }

    public int CancelOrder(string orderCode)
    {
        try
        {
            List<string> lisOrder = new List<string>() { orderCode };
            string url = $"{this._shipGhn!.PrefixApi}/v2/switch-status/cancel";
            var response = this._iHttpContextService
                .PostJsonAsync(new HttpClient(), url, new
                {
                    order_codes = lisOrder
                }, headers).Result;
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    var json = JObject.Parse(res);
                    return 1;
                }   
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                this._iLogger.LogError($"Ghn {orderCode} hủy đơn hàng lỗi: {res}");
                return -1;
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex,"");
        }

        return 0;
    }
}