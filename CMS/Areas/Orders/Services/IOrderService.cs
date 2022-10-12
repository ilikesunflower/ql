using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ClosedXML.Excel;
using CMS.Areas.Customer.Services;
using CMS.Areas.Orders.Const;
using CMS.Areas.Orders.Models;
using CMS.Models;
using CMS_Access.Repositories.Orders;
using CMS_Access.Repositories.WareHouse;
using CMS_EF.Models.Orders;
using CMS_EF.Models.WareHouse;
using CMS_Lib.DI;
using CMS_Lib.Helpers;
using CMS_Lib.Services.HttpContext;
using CMS_Lib.Util;
using CMS_Ship.Consts;
using CMS_Ship.GHN;
using CMS_Ship.GHN.Models;
using CMS_Ship.VnPost;
using CMS_WareHouse.KiotViet;
using CMS.Config.Consts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CMS.Areas.Orders.Services;

public interface IOrderService : IScoped
{
    bool UpdateOrderStatus(CMS_EF.Models.Orders.Orders orders, int status, UserInfo userInfo);

    SendShip SendShip(CMS_EF.Models.Orders.Orders orders);

    ResultJson CancelOrders(string code, string message);

    List<OrderPartnerShipLog> GetOrderPartnerShipLogByCode(string orderCode);

    List<ExportForControlViewModel> ReadDataFromExcelAndValidate(IFormFile file);
    void ImportData(List<ExportForControlViewModel> dataFile);
}

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _iLogger;
    private readonly IOrdersRepository _iOrdersRepository;
    private readonly IOrderLogRepository _iOrderLogRepository;
    private readonly IGhnService _iGhnService;
    private readonly IVnPostService _iVnPostService;
    private readonly IConfiguration _iConfiguration;
    private readonly IHttpContextService _iHttpContextService;
    private readonly IOrderPartnerShipLogRepository _iOrderPartnerShipLogRepository;
    private readonly IKiotVietService _kiotVietService;
    private readonly IWhTransactionRepository _iWhTransactionRepository;
    private readonly ICustomerNotificationService _iCustomerNotificationService;


    public OrderService(ILogger<OrderService> iLogger, IOrdersRepository iOrdersRepository,
        IOrderLogRepository iOrderLogRepository,
        IGhnService iGhnService, IVnPostService iVnPostService, IConfiguration iConfiguration,
        IHttpContextService iHttpContextService, IOrderPartnerShipLogRepository iOrderPartnerShipLogRepository,
        IKiotVietService kiotVietService, IWhTransactionRepository iWhTransactionRepository,
        ICustomerNotificationService iCustomerNotificationService)
    {
        _iLogger = iLogger;
        _iOrdersRepository = iOrdersRepository;
        _iOrderLogRepository = iOrderLogRepository;
        _iGhnService = iGhnService;
        _iVnPostService = iVnPostService;
        _iConfiguration = iConfiguration;
        _iHttpContextService = iHttpContextService;
        _iOrderPartnerShipLogRepository = iOrderPartnerShipLogRepository;
        _kiotVietService = kiotVietService;
        _iWhTransactionRepository = iWhTransactionRepository;
        _iCustomerNotificationService = iCustomerNotificationService;
    }

    public bool UpdateOrderStatus(CMS_EF.Models.Orders.Orders orders, int status, UserInfo userInfo)
    {
        try
        {
            DateTime t = DateTime.Now;
            OrderPartnerShipLog partnerShipLog = new OrderPartnerShipLog()
            {
                PartnerShipCreatedAt = t,
                OrderCode = orders.Code,
            };
            if (!orders.OrderStatusConfirmAt.HasValue)
            {
                orders.OrderStatusConfirmAt = t;
                partnerShipLog.PartnerShipDetails = $"{AppConst.AppName} đã xác nhận đơn";
            }

            if (!orders.OrderStatusShipAt.HasValue && status == OrderStatusConst.StatusOrderShip)
            {
                orders.OrderStatusShipAt = t;
                partnerShipLog.PartnerShipDetails = $"{AppConst.AppName} đã gửi đơn đến đối tác vận chuyển";
            }

            if (!orders.OrderStatusSuccessAt.HasValue && status == OrderStatusConst.StatusOrderSuccess)
            {
                orders.OrderStatusSuccessAt = t;
                partnerShipLog.PartnerShipDetails = $"{AppConst.AppName} giao hàng thành công";
            }

            if (orders.OrderStatusCancelAt.HasValue && status == OrderStatusConst.StatusOrderCancel)
            {
                orders.OrderStatusCancelAt = t;
                partnerShipLog.PartnerShipDetails = $"{AppConst.AppName} đã hủy đơn";
            }

            this._iOrderPartnerShipLogRepository.Create(partnerShipLog);
            orders.Status = status;
            orders.LastModifiedAt = DateTime.Now;
            this._iOrdersRepository.Update(orders);
            OrderLog orderLog = new OrderLog()
            {
                OrderId = orders.Id,
                Flag = 0,
                LastModifiedAt = DateTime.Now,
                Note =
                    $"Tài khoản {userInfo.UserName} câp nhật trạng thái đơn hàng sang {OrderStatusConst.ListStatus.FirstOrDefault(x => x.Key == status).Value}"
            };
            this._iOrderLogRepository.Create(orderLog);
            return true;
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"UpdateOrderStatus {orders.Id} - status {status}");
            // ignored
        }

        return false;
    }

    public SendShip? SendShip(CMS_EF.Models.Orders.Orders orders)
    {
        if (orders.ShipPartner == 1)
        {
            // ghn
            CreatedOrder c = new CreatedOrder()
            {
                Content = orders.OrderAddress.Note,
                OrderCode = orders.Code,
                ClientOrderCode = $"{orders.Id}",
                ToAddress = orders.OrderAddress.Address,
                ServiceTypeId = orders.ShipType,
                CodAmount = CmsFunction.ConvertToInt($"{orders.PriceShipNonSale}") ?? 0,
                PaymentTypeId = 1,
                ToName = orders.OrderAddress.Name,
                ToPhone = orders.OrderAddress.Phone,
                ToDistrictId = orders.OrderAddress.District.DistrictGhnId,
                ToWardCode = orders.OrderAddress.Commune.CommuneGhnId,
                Note = orders.OrderAddress.Note,
                Weight = orders.TotalWeight.HasValue ? (int) orders.TotalWeight.Value : 0
            };
            c.ListItem = orders.OrderProduct.ToList().Select(x => new Item()
            {
                name = $"{x.ProductName}",
                code = x.ProductSimilarCodeWh,
                price = IntegerHelper.ParseStringToInt($"{x.Price}") ?? 0,
                quantity = x.Quantity ?? 0,
                // weight = IntegerHelper.ParseStringToInt($"{ (x.Quantity > 0 ? (x.Weight / x.Quantity) : 0)}") ?? 0
            }).ToList();
            var o = this._iGhnService.CreateOrder(c);
            if (o != null && !string.IsNullOrEmpty(o.OrderCode))
            {
                return new SendShip()
                {
                    OrderCode = o.OrderCode,
                    Id = string.Empty,
                    Err = string.Empty,
                };
            }

            return new SendShip()
            {
                OrderCode = string.Empty,
                Id = string.Empty,
                Err = o?.Err,
            };
        }

        if (orders.ShipPartner == 2)
        {
            // vnpost
            CMS_Ship.VnPost.Models.CreatedOrder v = new CMS_Ship.VnPost.Models.CreatedOrder()
            {
                OrderCode = orders.Code,
                CustomerNote = orders.OrderAddress.Note,
                TypeShip = orders.ShipType ?? 0,
                WeightEvaluation = orders.TotalWeight,
                CodAmountEvaluation = CmsFunction.ConvertToDecimal($"{orders.PriceShipNonSale}"),
                ReceiverAddress = orders.OrderAddress.Address,
                ReceiverFullname = orders.OrderAddress.Name,
                ReceiverProvinceId = orders.OrderAddress.Province.ProvinceVnPostId,
                ReceiverDistrictId = orders.OrderAddress.District.DistrictVnPostId,
                ReceiverWardId = orders.OrderAddress.Commune.CommuneVnPostId,
                ReceiverPhone = orders.OrderAddress.Phone,
                IsPackageViewable = true,
            };
            string p =
                string.Join(" | ",
                    orders.OrderProduct.ToList()
                        .Select(x => $"{x.ProductName} ({x.ProductSimilarCodeWh}) - SL: {x.Quantity}"));
            if (!string.IsNullOrEmpty(p))
            {
                string p1 =
                    string.Join(" | ",
                        orders.OrderProduct.ToList().Select(x => $"{x.ProductSimilarCodeWh} - SL: {x.Quantity}"));
                v.PackageContent = p.Length >= 255 ? (p1.Length >= 255 ? p1.Substring(0, 254) : p1) : p;
            }

            var o = this._iVnPostService.CreateOrder(v);
            if (o != null && !string.IsNullOrEmpty(o.OrderCode))
            {
                return new SendShip()
                {
                    Id = o.ShipCodeIdVnPost,
                    OrderCode = o.OrderCode
                };
            }
            else
            {
                return new SendShip()
                {
                    Id = string.Empty,
                    OrderCode = string.Empty,
                    Err = o?.Err
                };
            }
        }

        return new SendShip()
        {
            Id = string.Empty,
            OrderCode = string.Empty
        };
    }

    public ResultJson CancelOrders(string code, string message)
    {
        try
        {
            var order = this._iOrdersRepository.FindByCode(code);
            if (order != null && !string.IsNullOrEmpty(order.CodeShip))
            {
                if (order.ShipPartner == ShipConst.GHN)
                {
                    this._iGhnService.CancelOrder(order.CodeShip);
                }
                else if (order.ShipPartner == ShipConst.VNnPost && !string.IsNullOrEmpty(order.ShipCodeIdVnPost))
                {
                    this._iVnPostService.CancelOrder(order.CodeShip, order.ShipCodeIdVnPost);
                }
            }

            string host = _iConfiguration["OrderService:Host"];
            string endpoint = _iConfiguration["OrderService:Endpoint"];
            string url = host + endpoint + $"/{code}";
            Dictionary<string, string> headers = new();
            var response = _iHttpContextService.PostJsonAsync(new HttpClient(), url, new {message}, headers);
            var responseResult = response.Result;

            responseResult.EnsureSuccessStatusCode();
            if (responseResult.StatusCode == HttpStatusCode.OK)
            {
                string json = responseResult.Content.ReadAsStringAsync().Result;
                var res = JsonConvert.DeserializeObject<OutputObject>(json)!.Show();
                if (!order!.OrderStatusCancelAt.HasValue)
                {
                    try
                    {
                        OrderPartnerShipLog partnerShipLog = new OrderPartnerShipLog()
                        {
                            OrderCode = order.Code,
                            PartnerShipDetails = $"{AppConst.AppName} đã hủy đơn",
                            PartnerShipCreatedAt = DateTime.Now,
                        };
                        this._iOrderPartnerShipLogRepository.Create(partnerShipLog);
                        if (order.CustomerId.HasValue)
                        {
                            this._iCustomerNotificationService.SendCustomerNotification(order.CustomerId.Value,
                                new CustomerNotificationObject()
                                {
                                    Title = $"{AppConst.AppName} đã hủy đơn hàng {order.Code}",
                                    Detail = "",
                                    Link = $"/account/purchase/{order.Code}"
                                });
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

                return res;
            }

            return new OutputObject(400, new { },
                "Một số sản phẩm trong giỏ hàng vừa được cập nhật, bạn vui lòng kiểm tra giỏ hàng và thử lại").Show();
        }
        catch (Exception e)
        {
            return new OutputObject(400, new { }, e.Message, e.Message).Show();
        }
    }

    public List<OrderPartnerShipLog> GetOrderPartnerShipLogByCode(string orderCode)
    {
        return this._iOrderPartnerShipLogRepository.FindAll().Where(x => x.OrderCode == orderCode)
            .OrderByDescending(x => x.PartnerShipCreatedAt).ToList();
    }

    public List<ExportForControlViewModel> ReadDataFromExcelAndValidate(IFormFile file)
    {
        XLWorkbook workbook = new XLWorkbook(file.OpenReadStream());
        if (!workbook.TryGetWorksheet("order",out IXLWorksheet ws))
        {
            throw new NullReferenceException("Không tìm thấy Sheet order");
        }
        IXLRange range = ws.RangeUsed();
        int rowCount = range.RowCount();

        List<ExportForControlViewModel> list = new List<ExportForControlViewModel>();

        for (var i = 4; i <= rowCount; i++)
        {
            IXLCell codeCell = range.Cell(i, 2);
            IXLCell statusCell = range.Cell(i, 3);

            bool isCodeCellNull = codeCell.IsEmpty() || string.IsNullOrEmpty(codeCell.GetString());
            bool isStatusCellNull = statusCell.IsEmpty() || string.IsNullOrEmpty(statusCell.GetString());

            if (isCodeCellNull) continue;
            if (isStatusCellNull)
            {
                throw new NullReferenceException($"Trạng thái ô D:{i} rỗng");
            }

            var code = codeCell.GetString().Trim();
            var statusStr = statusCell.GetString().Trim();

            list.Add(new ExportForControlViewModel()
            {
                Code = code,
                Status = statusStr == ExcelStatus.Paid.StatusStr ? ExcelStatus.Paid.Status : ExcelStatus.Unpaid.Status,
                StatusStr = statusStr,
            });
        }

        return list;
    }

    public void ImportData(List<ExportForControlViewModel> dataFile)
    {
        if (dataFile is {Count: <= 0}) return;

        List<string> codes = dataFile.Select(x => x.Code).ToList();

        List<CMS_EF.Models.Orders.Orders> orders = _iOrdersRepository.FindByCodes(codes);

        List<String> wrongCode = codes.Where(x => !orders.Select(xx => xx.Code).Contains(x)).ToList();

        if (wrongCode is {Count: > 0})
        {
            throw new NullReferenceException($"Mã {string.Join( ",", wrongCode )} không tồn tại!");
        }

        List<CMS_EF.Models.Orders.Orders> changeList = new List<CMS_EF.Models.Orders.Orders>();

        foreach (var order in orders)
        {
            var orderExcelData = dataFile.FirstOrDefault(x => x.Code == order.Code);
            if (orderExcelData == null)
            {
                continue;
            }

            if (orderExcelData.Status == order.StatusPayment) continue;
            order.StatusPayment = orderExcelData.Status;
            order.LastModifiedAt = DateTime.Now;
            changeList.Add(order);
        }

        if (changeList is {Count: > 0})
        {
            _iOrdersRepository.BulkUpdate(changeList);
        }
    }
}