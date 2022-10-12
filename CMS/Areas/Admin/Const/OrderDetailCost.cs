using System.Collections.Generic;
using CMS.Areas.Admin.ViewModels.Home.OrderDetail;
using CMS.Areas.Orders.Const;

namespace CMS.Areas.Admin.Const;

public class OrderDetailCost
{
      public static List<OrderDetailViewModel> ListOrderDetail = new List<OrderDetailViewModel>()
        {
            new OrderDetailViewModel()
            {
                Name = "Đơn chờ xử lý",
                CountOrder = 0,
                PriceOrder = 0,
                Type = 0, 
                ListStatus= new List<int>(){OrderStatusConst.StatusWaitCustomerConfirm, OrderStatusConst.StatusCustomerSuccess, OrderStatusConst.StatusOrderConfirm}
            
            }, new OrderDetailViewModel()
            {
                Name = "Đơn đang giao",
                CountOrder = 0,
                PriceOrder = 0,
                Type = 1, 
                ListStatus= new List<int>(){OrderStatusConst.StatusOrderShip}
            
            }, new OrderDetailViewModel()
            {
                Name = "Đơn hoàn thành",
                CountOrder = 0,
                PriceOrder = 0,
                Type = 2, 
                ListStatus= new List<int>(){OrderStatusConst.StatusOrderSuccess}

            }, new OrderDetailViewModel()
            {
                Name = "Đơn hủy",
                CountOrder = 0,
                PriceOrder = 0,
                Type = 3, 
                ListStatus= new List<int>(){OrderStatusConst.StatusOrderCancel}

            },
           
        };
}