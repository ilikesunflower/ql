using System.Net;
using CMS_Access.Repositories.Ship;
using CMS_EF.Models.Orders;
using CMS_EF.Models.Ship;
using CMS_Lib.DI;
using CMS_Lib.Services.HttpContext;
using CMS_Lib.Util;
using CMS_Ship.Consts;
using CMS_Ship.Models;
using CMS_Ship.VnPost.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CMS_Ship.VnPost;

public interface IVnPostService : IScoped
{
    Account? GetInfo(string username, string password);

    int CancelOrder(string orderCode,string orderIdVnpost);

    CreateOrderOutPut? CreateOrder(CreatedOrder createdOrder);

    List<CalculateFee> CalculateFee(string? receiverProvinceId, string? receiverDistrictId, decimal? weight,string? senderProvinceId = null, string? senderDistrictId = null);
}

public class VnPostService : IVnPostService
{
    private readonly ILogger<VnPostService> _iLogger;
    private readonly IHttpContextService _iHttpContextService;
    private readonly IShipVnPostRepository _iShipVnPostRepository;
    private ShipVnPost? _shipVnPost;
    private Dictionary<string, string> headers;

    public VnPostService(ILogger<VnPostService> iLogger, IHttpContextService iHttpContextService,
        IShipVnPostRepository iShipVnPostRepository)
    {
        _iLogger = iLogger;
        _iHttpContextService = iHttpContextService;
        _iShipVnPostRepository = iShipVnPostRepository;
        this._shipVnPost = this._iShipVnPostRepository.FindByStatus();
        if (this._shipVnPost != null)
        {
            headers = new Dictionary<string, string>()
            {
                { "h-token", this._shipVnPost!.Token },
            };   
        }
    }

    public Account? GetInfo(string username, string password)
    {
        if (this._shipVnPost == null)
        {
            return null;
        }
        try
        {
            string url = $"{this._shipVnPost!.PrefixApi}/MobileAuthentication/GetAccessToken";
            var response = this._iHttpContextService
                .PostJsonAsync(new HttpClient(), url, new { TenDangNhap = username, MatKhau = password }, headers)
                .Result;
            response.EnsureSuccessStatusCode();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string res = response.Content.ReadAsStringAsync().Result;
                var json = JObject.Parse(res);
                string token = $"{json["Token"]}";
                if (!string.IsNullOrEmpty(token))
                {
                    Account rs = new Account()
                    {
                        Token = token,
                        Email = $"{json.SelectToken("Payload.Email")}",
                        Phone = $"{json.SelectToken("Payload.SoDienThoai")}",
                        MaCrm = $"{json.SelectToken("Payload.MaCRM")}",
                        ProvinceId = $"{json.SelectToken("Payload.MaTinhThanh")}",
                    };
                    return rs;
                }
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"Get token vnPost username {username}");
        }

        return null;
    }

    public List<CalculateFee> CalculateFee(string? receiverProvinceId, string? receiverDistrictId, decimal? weight,string? senderProvinceId = null, string? senderDistrictId = null)
    {
        if (this._shipVnPost == null)
        {
            return new List<CalculateFee>();
        }
        try
        {
            string url = $"{this._shipVnPost!.PrefixApi}/CustomerConnect/TinhCuocTatCaDichVu";
            var param = new
            {
                SenderProvinceId = string.IsNullOrEmpty(senderProvinceId) ? this._shipVnPost.ProvinceId : senderProvinceId,
                SenderDistrictId = string.IsNullOrEmpty(senderDistrictId) ? this._shipVnPost.DistrictId : senderDistrictId,
                ReceiverProvinceId = receiverProvinceId,
                ReceiverDistrictId = receiverDistrictId,
                Weight = weight
            };
            var response = this._iHttpContextService
                .PostJsonAsync(new HttpClient(), url, param, headers).Result;
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    var json = JArray.Parse(res);
                    if (json is { Count: > 0 })
                    {
                        var list = json.Select(o => new CalculateFee()
                        {
                            CodeService = $"{o["MaDichVu"]}",
                            TypeService = $"{o["MaDichVu"]}" == VnPostConst.VnPostStandard ? 2 : ($"{o["MaDichVu"]}" == VnPostConst.VnPostExpress ? 1 : 0),
                            Total = CmsFunction.ConvertToDouble($"{o["TongCuocSauVAT"]}")
                        }).Where(x => x.TypeService == 2 || x.TypeService == 1).ToList();
                        return list;
                    }
                }   
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                this._iLogger.LogError($"Tính cước vnpost fail: {res}");
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, "Tính cước vnpost");
        }

        return new List<CalculateFee>();
    }

    public CreateOrderOutPut? CreateOrder(CreatedOrder createdOrder)
    {
        if (this._shipVnPost == null)
        {
            return null;
        }
        try
        {
            string url = $"{this._shipVnPost!.PrefixApi}/CustomerConnect/CreateOrder";
            var param = new
            {
                SenderTel = this._shipVnPost.Phone,
                SenderFullname = this._shipVnPost.FullName,
                SenderAddress = this._shipVnPost.Address,
                SenderWardId = this._shipVnPost.WardId,
                SenderDistrictId = this._shipVnPost.DistrictId,
                SenderProvinceId = this._shipVnPost.ProvinceId,
                ReceiverTel = createdOrder.ReceiverPhone,
                ReceiverFullname = createdOrder.ReceiverFullname,
                ReceiverAddress = createdOrder.ReceiverAddress,
                ReceiverWardId = createdOrder.ReceiverWardId,
                ReceiverDistrictId = createdOrder.ReceiverDistrictId,
                ReceiverProvinceId = createdOrder.ReceiverProvinceId,
                PickupType = 1,
                IsPackageViewable = createdOrder.IsPackageViewable,
                // ReceiverAddressType = createdOrder.ReceiverAddressType,
                ServiceName = VnPostConst.GetServiceName(createdOrder.TypeShip),
                OrderCode = $"{TypeShipConst.PrefixOrder}{createdOrder.OrderCode}",
                PackageContent = createdOrder.PackageContent,
                WeightEvaluation = createdOrder.WeightEvaluation,
                CodAmountEvaluation = createdOrder.CodAmountEvaluation,
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
                    string orderCode = $"{json["ItemCode"]}";
                    if (!string.IsNullOrEmpty(orderCode))
                    {
                        var rs = new CreateOrderOutPut()
                        {
                            OrderCode = orderCode,
                            ShipCodeIdVnPost = $"{json["Id"]}",
                            TotalFee = CmsFunction.ConvertToInt($"{json["TotalFreightIncludeVatEvaluation"]}"),
                            ExpectedDeliveryTime = ""
                        };
                        return rs;
                    }
                }
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                this._iLogger.LogError($"CreateOrder {createdOrder.OrderCode} : err {res}");
                return new CreateOrderOutPut()
                {
                    OrderCode = string.Empty,
                    ShipCodeIdVnPost = string.Empty,
                    TotalFee = 0,
                    ExpectedDeliveryTime = "",
                    Err = res
                };
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"Tạo mới đơn hàng cho VNPost lỗi: {createdOrder.OrderCode}");
        }

        return new CreateOrderOutPut()
        {
            OrderCode = string.Empty,
            ShipCodeIdVnPost = string.Empty,
            TotalFee = 0,
            ExpectedDeliveryTime = "",
            Err = string.Empty
        };
    }

    public int CancelOrder(string orderCode, string orderIdVnpost)
    {
        if (this._shipVnPost == null)
        {
            return 0;
        }
        try
        {
            string url = $"{this._shipVnPost!.PrefixApi}/CustomerConnect/CancelOrder";
            var param = new
            {
                OrderId = orderIdVnpost
            };
            var response = this._iHttpContextService
                .PostJsonAsync(new HttpClient(), url, param, headers).Result;
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    var json = JObject.Parse(res);
                    return 1;
                }
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                this._iLogger.LogError($"CancelOrder {orderIdVnpost} : err {res}");
                return -1;
            }
        }
        catch (Exception ex)
        {
            // ignored
            this._iLogger.LogError(ex,"CancelOrder");
        }

        return 0;
    }
}