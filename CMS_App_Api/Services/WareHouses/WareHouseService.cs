using System;
using System.Threading.Tasks;
using CMS_Access.Repositories.Orders;
using CMS_Access.Repositories.WareHouse;
using CMS_EF.Models.WareHouse;
using CMS_Lib.DI;
using CMS_WareHouse.KiotViet;
using CMS_WareHouse.KiotViet.Consts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace CMS_App_Api.Services.WareHouses;

public interface IWareHouseService : IScoped
{
    void CreateOrder(CMS_EF.Models.Orders.Orders orders);
    void DeleteOrder(CMS_EF.Models.Orders.Orders orders);
    void EditOrder(CMS_EF.Models.Orders.Orders orders);
}

public class WareHouseService : IWareHouseService
{
    private readonly ILogger<WareHouseService> _iLogger;
    private readonly IWhTransactionRepository _iWhTransactionRepository;
    private readonly IKiotVietService _iKiotVietService;
    private readonly IOrdersRepository _iOrdersRepository;

    public WareHouseService(ILogger<WareHouseService> iLogger, IKiotVietService iKiotVietService,
        IWhTransactionRepository iWhTransactionRepository, IOrdersRepository iOrdersRepository)
    {
        this._iLogger = iLogger;
        this._iKiotVietService = iKiotVietService;
        this._iWhTransactionRepository = iWhTransactionRepository;
        this._iOrdersRepository = iOrdersRepository;
    }

    public void CreateOrder(CMS_EF.Models.Orders.Orders orders)
    {
        try
        {
            var o = this._iWhTransactionRepository.FindByOrderIdStatus(orders.Id, WhTransactionConst.Create);
            var wareHouse = this._iKiotVietService.CreateOrder(orders);
            if (wareHouse != null)
            {
                if (wareHouse.Status == 1)
                {
                    this._iLogger.LogInformation($"Đồng bộ đơn hàng {orders.Code} sang kiot việt thành công");
                    orders.OrderIdWh = wareHouse.OrderId;
                    this._iOrdersRepository.Update(orders);
                    if (o != null)
                    {
                        this._iWhTransactionRepository.Delete(o);
                    }
                }
                else if (wareHouse.Status == -1)
                {
                    // xử lý lưu db để call lại khi kiot việt lỗi
                    if (o == null)
                    {
                        WhTransaction whTransaction = new WhTransaction()
                        {
                            OrderId = orders.Id,
                            Status = WhTransactionConst.Create,
                            CreatedAt = DateTime.Now
                        };
                        this._iWhTransactionRepository.Create(whTransaction);
                    }
                    else
                    {
                        o.CreatedAt = DateTime.Now;
                        this._iWhTransactionRepository.Update(o);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex,$"Tạo order {orders.Id} bên kiot việt lỗi");
        }
    }

    public void EditOrder(CMS_EF.Models.Orders.Orders orders)
    {
        // xóa đơn hàng cũ bên kiot việt
        if (orders.OrderIdWh.HasValue)
        {
            DeleteOrder(orders);
        }
        
        // tạo đơn mới bên kiot việt
        CreateOrder(orders);
        
        
        // var task = new Task(() =>
        // {
        //     try
        //     {
        //         var isUpdateProduct = false;
        //         // xóa order kiot viet
        //         if (orders.OrderIdWh.HasValue)
        //         {
        //             var rs = this._iKiotVietService.DeleteOrderId(orders.OrderIdWh.Value);
        //             if (rs == 1)
        //             {
        //                 orders.OrderIdWh = null;
        //                 this._iOrdersRepository.Update(orders);
        //             }
        //             else if (rs == -1)
        //             {
        //                 var check = this._iWhTransactionRepository.FindByOrderIdWhStatus(orders.Id,
        //                     orders.OrderIdWh.Value,
        //                     WhTransactionConst.Cancel);
        //                 if (check == null)
        //                 {
        //                     WhTransaction whTransaction = new WhTransaction()
        //                     {
        //                         OrderId = orders.Id,
        //                         Status = WhTransactionConst.Cancel,
        //                         OrderIdKiot = orders.OrderIdWh.Value,
        //                         CreatedAt = DateTime.Now
        //                     };
        //                     this._iWhTransactionRepository.Create(whTransaction);
        //                     orders.OrderIdWh = null;
        //                     isUpdateProduct = true;
        //                 }
        //             }
        //         }
        //
        //         // tạo order kiot
        //         var o = this._iWhTransactionRepository.FindByOrderIdStatus(orders.Id, WhTransactionConst.Create);
        //         var wareHouse = this._iKiotVietService.CreateOrder(orders);
        //         if (wareHouse != null)
        //         {
        //             if (wareHouse.Status == 1)
        //             {
        //                 this._iLogger.LogInformation("Đồng bộ sang kiot việt thành công");
        //                 orders.OrderIdWh = wareHouse.OrderId;
        //                 isUpdateProduct = true;
        //                 if (o != null)
        //                 {
        //                     this._iWhTransactionRepository.Delete(o);
        //                 }
        //             }
        //             else if (wareHouse.Status == -1)
        //             {
        //                 // xử lý lưu db để call lại khi kiot việt lỗi
        //                 if (o == null)
        //                 {
        //                     WhTransaction whTransaction = new WhTransaction()
        //                     {
        //                         OrderId = orders.Id,
        //                         Status = WhTransactionConst.Create,
        //                         CreatedAt = DateTime.Now
        //                     };
        //                     this._iWhTransactionRepository.Create(whTransaction);
        //                 }
        //                 else
        //                 {
        //                     o.CreatedAt = DateTime.Now;
        //                     this._iWhTransactionRepository.Update(o);
        //                 }
        //             }
        //         }
        //
        //         if (isUpdateProduct)
        //         {
        //             this._iOrdersRepository.Update(orders);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         this._iLogger.LogError(ex,
        //             $"orderId: {orders.Id} Hủy order cũ {orders.OrderIdWh} tạo order mới trong kiot viet");
        //     }
        // });
        // task.Start();
    }

    public void DeleteOrder(CMS_EF.Models.Orders.Orders orders)
    {
        try
        {
            if (orders.OrderIdWh.HasValue)
            {
                var rs = this._iKiotVietService.DeleteOrderId(orders.OrderIdWh.Value);
                if (rs == 1)
                {
                    orders.OrderIdWh = null;
                    this._iOrdersRepository.Update(orders);
                }
                else if (rs == -1)
                {
                    var o = this._iWhTransactionRepository.FindByOrderIdWhStatus(orders.Id, orders.OrderIdWh.Value,
                        WhTransactionConst.Cancel);
                    if (o == null)
                    {
                        WhTransaction whTransaction = new WhTransaction()
                        {
                            OrderId = orders.Id,
                            Status = WhTransactionConst.Cancel,
                            OrderIdKiot = orders.OrderIdWh,
                            CreatedAt = DateTime.Now
                        };
                        this._iWhTransactionRepository.Create(whTransaction);
                        orders.OrderIdWh = null;
                        this._iOrdersRepository.Update(orders);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"Rollback hàng hóa lỗi: {orders.Code}");
        }
    }
}