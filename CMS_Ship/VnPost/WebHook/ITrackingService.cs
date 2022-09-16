using CMS_Access.Repositories.Orders;
using CMS_EF.Models.Orders;
using CMS_Lib.DI;
using CMS_Ship.Consts;
using CMS_Ship.VnPost.WebHook.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CMS_Ship.VnPost.WebHook;

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
        if (!string.IsNullOrEmpty(o.OrderCode) &&
            o.OrderCode.ToUpper().Trim().StartsWith(TypeShipConst.PrefixOrder.ToUpper()))
        {
            try
            {
                string? orderCode = o.OrderCode.ToUpper().Trim().Replace(TypeShipConst.PrefixOrder.ToUpper(), "");
                if (!string.IsNullOrEmpty(orderCode))
                {
                    OrderPartnerShipLog rs = new OrderPartnerShipLog()
                    {
                        PartnerShipCode = o.ItemCode,
                        PartnerShipStatus = $"{o.OrderStatusId}",
                        PartnerShipType = VnPostConst.GetServiceNameToType(o.ServiceName),
                        PartnerShipDetails = VnPostStatusConst.ListStatus.FirstOrDefault(x => x.Key == o.OrderStatusId)
                            .Value,
                        PartnerShipCreatedAt = DateTime.Now,
                        OrderCode = orderCode
                    };
                    this._iOrderPartnerShipLogRepository.Create(rs);
                    if (o.OrderStatusId == VnPostStatusConst.ShipSuccess)
                    {
                        // hoàn thành đơn hàng
                        var data = this._iOrdersRepository.FindByShipCode(rs.PartnerShipCode);
                        if (data != null && data.Status != 4 && data.Status != 5)
                        {
                            data.Status = 4;
                            data.OrderStatusSuccessAt = DateTime.Now;
                            this._iOrdersRepository.Update(data);
                            this._iLogger.LogInformation($"Hệ thống giao hàng đã giao thành công đơn id: {data.Code}");
                            return data;
                        }
                    }
                    else if (o.OrderStatusId == VnPostStatusConst.CodSuccess)
                    {
                        var data = this._iOrdersRepository.FindByShipCode(rs.PartnerShipCode);
                        if (data != null && data.StatusPayment != 1)
                        {
                            data.StatusPayment = 1;
                            this._iOrdersRepository.Update(data);
                            this._iLogger.LogInformation(
                                $"Hệ thống giao hàng đơn hàng {o.OrderCode} | mã ship {o.ItemCode} đã chuyển tiền COD thành công: {o.TotalFreightIncludeVatEvaluation}");
                            return data;
                        }
                    }

                    this._iLogger.LogInformation(
                        $"VnPost Tracking đơn hàng {o.OrderCode} | mã ship {o.ItemCode} | status {o.OrderStatusId}");
                    return null;
                }
                else
                {
                    _iLogger.LogWarning(
                        $"VnPost tracking đơn hàng {o.OrderCode} mã ship {o.ItemCode} không tồn tại trong hệ thống");
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex, $"VnPost tracking {JsonConvert.SerializeObject(o)}");
            }
        }
        else
        {
            _iLogger.LogWarning(
                $"VnPost tracking đơn hàng {o.OrderCode} mã ship {o.ItemCode} không tồn tại trong hệ thống");
        }

        return null;
    }
}