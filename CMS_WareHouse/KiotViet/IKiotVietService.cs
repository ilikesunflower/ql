using System.Net;
using CMS_Access.Repositories.Orders;
using CMS_Access.Repositories.WareHouse;
using CMS_EF.Models.Orders;
using CMS_EF.Models.WareHouse;
using CMS_Lib.DI;
using CMS_Lib.Services.HttpContext;
using CMS_Lib.Util;
using CMS_WareHouse.KiotViet.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CMS_WareHouse.KiotViet;

public interface IKiotVietService : IScoped
{
    ProductDetail? FindProductDetail(string code);

     int DeleteOrderId(int id);

     OrderWareHouse? CreateOrder(Orders orders);

}

public class KiotVietService : IKiotVietService
{
    private readonly ILogger<KiotVietService> _iLogger;
    private readonly WhKiotViet? _whKiotViet;
    private readonly IWhKiotVietRepository _whKiotVietRepository;
    private readonly IWhTransactionRepository _iWhTransactionRepository;
    private readonly IOrdersRepository _iOrdersRepository;
    private readonly IHttpContextService _iHttpContextService;
    private readonly Dictionary<string, string>? _header;

    public KiotVietService(ILogger<KiotVietService> iLogger, IWhKiotVietRepository whKiotVietRepository,
        IHttpContextService iHttpContextService, IWhTransactionRepository iWhTransactionRepository, IOrdersRepository iOrdersRepository)
    {
        _iLogger = iLogger;
        _whKiotVietRepository = whKiotVietRepository;
        _iHttpContextService = iHttpContextService;
        _iWhTransactionRepository = iWhTransactionRepository;
        _iOrdersRepository = iOrdersRepository;
        _whKiotViet = whKiotVietRepository.FindByStatus();
        if (_whKiotViet != null &&
            (string.IsNullOrEmpty(_whKiotViet.Token) || (_whKiotViet.ExpiredTokenTime < DateTime.Now)))
        {
            GetToken();
        }

        if (_whKiotViet != null)
        {
            _header = new Dictionary<string, string>()
            {
                { "Retailer", _whKiotViet.Retailer },
                { "Authorization", $"Bearer {_whKiotViet.Token}" }
            };
        }
    }

    private Tokens GetToken()
    {
        if (_whKiotViet == null)
        {
            return null;
        }

        try
        {
            string url = $"https://id.kiotviet.vn/connect/token";
            var param = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", this._whKiotViet.ClientId),
                new KeyValuePair<string, string>("client_secret", this._whKiotViet.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scopes", "PublicApi.Access"),
            });
            var response = _iHttpContextService.PostFormAsync(new HttpClient(), url, param)
                .Result;
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    var json = JObject.Parse(res);
                    Tokens tokens = new Tokens()
                    {
                        access_token = $"{json["access_token"]}",
                        expires_in = CmsFunction.ConvertToInt($"{json["expires_in"]}")
                    };
                    _whKiotViet.Token = tokens.access_token;
                    _whKiotViet.ExpiredTokenTime = DateTime.Now.AddSeconds(tokens.expires_in!.Value);
                    this._whKiotVietRepository.Update(_whKiotViet);
                }
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                this._iLogger.LogError($"get token lỗi: {res}");
            }
        }
        catch (Exception e)
        {
            this._iLogger.LogError(e, "Get token");
        }

        return null;
    }

    public bool ChangeProduct(string code, int quantity)
    {
        if (this._whKiotViet == null)
        {
            return false;
        }
        if (!string.IsNullOrEmpty(code))
        {
            var pDetail = FindProductDetail(code);
            if (pDetail != null)
            {
                string url = $"{this._whKiotViet!.PrefixApi}/products/{pDetail.ProductId}";
                var response = this._iHttpContextService
                    .PutJsonAsync(new HttpClient(), url, new
                    {
                        inventories = new
                        {
                            branchId = pDetail.BranchId,
                            onHand = Math.Max(pDetail.Quantity - quantity, 0)
                        }
                    }, _header).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        var json = JObject.Parse(res);
                        int? id = CmsFunction.ConvertToInt($"{json["id"]}");
                        if (id.HasValue)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    this._iLogger.LogError($"Cập nhật sản phẩm {code} trong kho kiot việt lỗi: {res}");
                }
            }
        }

        return false;
    }

    public ProductDetail? FindProductDetail(string code)
    {
        if (this._whKiotViet == null)
        {
            return null;
        }
        try
        {
            string url = $"{this._whKiotViet!.PrefixApi}/products/code/{code}";
            var response = _iHttpContextService.GetJsonAsync(new HttpClient(), url, _header).Result;
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    var json = JObject.Parse(res);
                    int? id = CmsFunction.ConvertToInt($"{json["id"]}");
                    if (id.HasValue)
                    {
                        
                        var jsonInventories = JArray.Parse($"{json["inventories"]}").OfType<JObject>().Select(o => new
                        {
                            branchId = $"{o["branchId"]}",
                            productId = $"{o["productId"]}",
                            cost = $"{o["cost"]}",
                            onHand = $"{o["onHand"]}"
                        }).FirstOrDefault();
                        ProductDetail rs = new ProductDetail()
                        {
                            Code = $"{json["id"]}",
                            ProductId = id.Value,
                            BranchId = CmsFunction.ConvertToInt(jsonInventories!.branchId)!.Value,
                            Cost = CmsFunction.ConvertToInt($"{json["basePrice"]}") ?? 0,
                            Quantity = CmsFunction.ConvertToInt(jsonInventories.onHand)!.Value
                        };
                        return rs;
                    }
                }
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                this._iLogger.LogError($"FindProductDetail: code: {code} lỗi: {res}");
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"FindProductDetail: code: {code}");
        }

        return null;
    }
    
    public int DeleteOrderId(int id)
    {
        if (this._whKiotViet == null)
        {
            return 0;
        }
        try
        {
            string url = $"{this._whKiotViet!.PrefixApi}/invoices";
            var response = this._iHttpContextService.DeleteJsonAsync(new HttpClient(), url, new { id = id,isVoidPayment = true }, _header).Result;
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    var json = JObject.Parse(res);
                    int? orderId = CmsFunction.ConvertToInt($"{json["id"]}");
                    if (orderId.HasValue)
                    {
                        this._iLogger.LogError($"DeleteOrderId: orderId: {id} thành công");
                        return 1;
                    }   
                }
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                this._iLogger.LogError($"DeleteOrderId: orderId: {id} lỗi: {res}");
                if (response.StatusCode == HttpStatusCode.GatewayTimeout || response.StatusCode == HttpStatusCode.BadGateway 
                                                                         || response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    return -1;
                }
            }
        }
        catch(Exception ex)
        {
            // ignored
            this._iLogger.LogError(ex,"Xóa đơn hàng trong kiot viet thành công");
        }

        return 0;
    }
    
    public OrderWareHouse? CreateOrder(Orders orders)
    {
        if (this._whKiotViet == null)
        {
            return null;
        }
        try
        {
            string url = $"{this._whKiotViet!.PrefixApi}/invoices";
            var list = orders.OrderProduct.Where(x => !string.IsNullOrEmpty(x.ProductSimilarCodeWh)).Select(x => new
            {
                productCode = x.ProductSimilarCodeWh,
                quantity = x.Quantity,
                price = x.Price
            }).ToList();
            if (list.Count == 0)
            {
                return new OrderWareHouse()
                {
                    Status = 0,
                    Msg = "Không có sản phẩm có mã sku"
                };
            }
            var param = new
            {
                branchId = this._whKiotViet.BranchId,
                soldById = this._whKiotViet.SoldById,
                description = $"Tạo đơn từ hệ thống Daiichi {orders.Code}",
                discount = "",
                discountRatio = "",
                purchaseDate = "",
                invoiceDetails = list,
                method = "Card"
            };
            var response = this._iHttpContextService.PostJsonAsync(new HttpClient(), url, param, _header).Result;
            if (response.IsSuccessStatusCode)
            {
                string res = response.Content.ReadAsStringAsync().Result;
                var json = JObject.Parse(res);
                int? id = CmsFunction.ConvertToInt($"{json["id"]}");
                if (id.HasValue)
                {
                    this._iLogger.LogInformation($"CreateOrder kiot viet {orders.Code} thành công");
                    return new OrderWareHouse()
                    {
                        OrderId = id.Value,
                        Status = 1,
                        Msg = "Tạo đơn hàng trên kiot việt thành công"
                    };
                }
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                this._iLogger.LogError($"CreateOrder: orderId : {orders.Id}:  lỗi: {res}");
                return new OrderWareHouse()
                {
                    Status = -1,
                    OrderId = 0,
                    Msg = "Lỗi hệ thống api"
                };
            }
        }
        catch (Exception ex)
        {
            // ignored
            this._iLogger.LogError(ex,"CreateOrder err");
        }

        return null;
    }
    
}