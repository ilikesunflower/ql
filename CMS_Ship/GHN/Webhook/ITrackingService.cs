using CMS_Access.Repositories.Orders;
using CMS_EF.Models.Orders;
using CMS_Lib.DI;
using CMS_Lib.Util;
using CMS_Ship.Consts;
using CMS_Ship.GHN.Webhook.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CMS_Ship.GHN.Webhook;

public interface ITrackingService : IScoped
{
    public Orders? InsertTracking(TrackingObject o);
}

public class TrackingService : ITrackingService
{
    private readonly ILogger<TrackingService> _iLogger;
    private readonly IOrderPartnerShipLogRepository _iOrderPartnerShipLogRepository;
    private readonly IOrdersRepository _iOrdersRepository;

    public TrackingService(ILogger<TrackingService> iLogger,
        IOrderPartnerShipLogRepository iOrderPartnerShipLogRepository, IOrdersRepository iOrdersRepository)
    {
        _iLogger = iLogger;
        _iOrderPartnerShipLogRepository = iOrderPartnerShipLogRepository;
        _iOrdersRepository = iOrdersRepository;
    }

    public Orders? InsertTracking(TrackingObject o)
    {
        try
        {
            if (!string.IsNullOrEmpty(o.ClientOrderCode) && !string.IsNullOrEmpty(o.OrderCode) &&
                o.ClientOrderCode.ToUpper().Trim().StartsWith(TypeShipConst.PrefixOrder.ToUpper()))
            {
                string? orderCode = o.ClientOrderCode.ToUpper().Trim().Replace(TypeShipConst.PrefixOrder.ToUpper(), "");
                if (!string.IsNullOrEmpty(orderCode))
                {
                    KeyValuePair<string, string>? content =
                        GhnStatusConst.ListStatus.FirstOrDefault(x => x.Key == o.Status);
                    if (content.HasValue)
                    {
                        OrderPartnerShipLog rs = new OrderPartnerShipLog()
                        {
                            PartnerShipCode = o.OrderCode,
                            PartnerShipDetails = content.Value.Value,
                            PartnerShipStatus = o.Status,
                            PartnerShipType = CmsFunction.ConvertToInt(o.Type),
                            PartnerShipCreatedAt = DateTime.Now,
                            OrderCode = orderCode
                        };
                        _iOrderPartnerShipLogRepository.Create(rs);
                        // hoàn thành đơn hàng
                        if (o.Status == GhnStatusConst.ShipSuccess)
                        {
                            var order = this._iOrdersRepository.FindByShipCode(rs.PartnerShipCode);
                            if (order != null && order.Status != 4 && order.Status != 5)
                            {
                                order.Status = 4;
                                order.OrderStatusSuccessAt = DateTime.Now;
                                if (!string.IsNullOrEmpty(o.CODTransferDate))
                                {
                                    order.StatusPayment = 1;
                                }

                                _iLogger.LogInformation(
                                    $"Đối tác GHN xác nhận hoàn thành đơn hàng {o.ClientOrderCode} | mã ship {o.OrderCode}");
                                this._iOrdersRepository.Update(order);
                                // notification
                                return order;
                            }
                            else if (order is { Status: 4 } && order.Status != 5)
                            {
                                if (!string.IsNullOrEmpty(o.CODTransferDate))
                                {
                                    order.StatusPayment = 1;
                                    _iLogger.LogInformation(
                                        $"Đối tác GHN xác nhận hoàn thành đơn hàng {o.ClientOrderCode} | mã ship {o.OrderCode} và gửi COD {o.CODTransferDate}");
                                    this._iOrdersRepository.Update(order);
                                }
                            }
                        }

                        return null;
                    }
                }

                this._iLogger.LogInformation(
                    $"Tracking đơn hàng {o.ClientOrderCode} | mã ship {o.OrderCode} | status {o.Status}");
            }
            else
            {
                this._iLogger.LogWarning(
                    $"Tracking đơn hàng Đơn hàng {o.ClientOrderCode} mã ship {o.OrderCode} không tồn tại");
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"GHN tracking: {JsonConvert.SerializeObject(o)}");
        }

        return null;
    }
}