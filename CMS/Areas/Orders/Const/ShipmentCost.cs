using System.Collections.Generic;
using CMS.Areas.Orders.Models;

namespace CMS.Areas.Orders.Const;

public class ShipmentCost
{
    public static List<ShipmentType> ShipmentTypes = new()
    {
        // new ShipmentType
        // {
        //     Name = "Giao hàng nhanh",
        //     Icon = "fa-solid fa-truck-fast",
        //     Type = 1
        // },
        new ShipmentType
        {
            Name = "Giao hàng tiêu chuẩn",
            Icon = "fa-solid fa-truck",
            Type = 2
        }
    };
    public static List<ShipmentType> ShipmentTypeEs = new()
    {
        new ShipmentType
        {
            Name = "Giao hàng nhanh",
            Icon = "fa-solid fa-truck-fast",
            Type = 1
        },
        new ShipmentType
        {
            Name = "Giao hàng tiêu chuẩn",
            Icon = "fa-solid fa-truck",
            Type = 2
        }
    };
}