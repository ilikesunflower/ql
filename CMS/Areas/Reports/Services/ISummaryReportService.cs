#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using CMS_Access.Repositories.Orders;
using CMS_Lib.DI;
using CMS.Areas.Customer.Const;
using CMS.Areas.Orders.Const;
using CMS.Areas.Reports.Models.SummaryReports;

namespace CMS.Areas.Reports.Services;

public interface ISummaryReportService : IScoped
{
    IQueryable<CMS_EF.Models.Orders.Orders> GetListOrders(string txtSearch,DateTime? start, DateTime? end,int? paymentStatus,int? status,bool isUsePoint);
    List<ExportExcelModel> GetListOrdersExcel(string txtSearch,DateTime? start, DateTime? end,int? paymentStatus,int? status,bool isUsePoint);
}

public class SummaryReportService : ISummaryReportService
{
    private readonly IOrdersRepository _iOrdersRepository;

    public SummaryReportService(IOrdersRepository iOrdersRepository)
    {
        _iOrdersRepository = iOrdersRepository;
    }

    public IQueryable<CMS_EF.Models.Orders.Orders> GetListOrders(string txtSearch,DateTime? start, DateTime? end,int? paymentStatus,int? status,bool isUsePoint)
    {
        return _iOrdersRepository.GetOrderIncludeProductAndAddressAndCustomer( txtSearch,start, end,paymentStatus,status,isUsePoint).Where(x => x.Status != OrderStatusConst.StatusOrderCancel);
    }

    public List<ExportExcelModel> GetListOrdersExcel(string txtSearch,DateTime? start, DateTime? end,int? paymentStatus,int? status,bool isUsePoint)
    {
        var orders = GetListOrders(txtSearch,start, end,paymentStatus,status,isUsePoint).Where(x => x.Status != OrderStatusConst.StatusOrderCancel).ToList();

        return (from order in orders
            from orderProduct in order.OrderProduct
            select new ExportExcelModel
            {
                Code = order.Code ?? "",
                OrderAtDate = order.OrderAt,
                OrderAt = order.OrderAt?.ToString("dd/MM/yyyy HH:mm") ?? "",
                OrderStatusConfirmAt = order.OrderStatusConfirmAt?.ToString("dd/MM/yyyy HH:mm") ?? "",
                PaymentType = PaymentMethodConst.BindPaymentMethod(order.PaymentType ?? 0),
                StatusPayment = OrderStatusPayment.BindStatusStr(order.StatusPayment ?? 0),
                Status = OrderStatusConst.GetStatusStr(order.Status ?? 0),
                ShipPartner = ShipConst.GetShipment(order.ShipPartner ?? 0, order.ShipType ?? 0),
                CodeShip = order.CodeShip ?? "",
                ProductName = orderProduct.ProductName ?? "",
                Quantity = orderProduct.Quantity ?? 0,
                Price = order.Total ?? 0,
                PriceShip = order.PriceShip ?? 0,
                UserName = order!.Customer?.UserName ?? "",
                Org =  BindIdOrg(order.Customer),
                Name = order.OrderAddress?.Name ?? "",
                Phone = order.OrderAddress?.Phone ?? "",
                Email = order.OrderAddress?.Email ?? "",
                Address = $"{order.OrderAddress?.Address ?? ""}, Xã/Phường: {order.OrderAddress?.Commune?.Name}, Quận/Huyện: {order.OrderAddress?.District?.Name}, Tỉnh/Thành phố: {order.OrderAddress?.Province?.Name}",
                AddressNote = order.OrderAddress?.Note ?? "",
                Note = order.Note ?? ""
            })
            .OrderByDescending(x => x.OrderAtDate)
            .ToList();
    }

    public static string BindIdOrg(CMS_EF.Models.Customers.Customer? customer)
    {
        return customer!.TypeGroup == CustomerTypeGroupConst.PhongBan
            ? customer.Org
            : CustomerTypeGroupConst.GetCustomerTypeGroup(customer.TypeGroup ?? 0);
    }
}