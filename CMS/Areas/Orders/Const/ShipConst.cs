using System.Collections.Generic;
using System.Linq;
using CMS.Areas.Orders.Models;

namespace CMS.Areas.Orders.Const;

public class ShipConst
{
    public static int GHN = 1;
    public static int VNnPost = 2;
    public static int WareHouse = 0;
    public static int OtherPartner = 3;
    
    public static List<ShipmentPartner> ShipmentModel = new List<ShipmentPartner>
    {
        new ShipmentPartner
        {
            Name = "GHN",
            Type = 1,
            ShipmentTypes = ShipmentCost.ShipmentTypes
        },
        new ShipmentPartner
        {
            Name = "VNPost",
            Type = 2,
            ShipmentTypes = ShipmentCost.ShipmentTypeEs
        },
        new ShipmentPartner
        {
            Name = "Nhận hàng tại kho",
            Type = 0,
            ShipmentTypes = new List<ShipmentType>()
        },
        new ShipmentPartner
        {
            Name = "Đối tác khác",
            Type = 3,
            ShipmentTypes = new List<ShipmentType>()
        }
    };

    public static string GetShipment(int shipPartner, int? shipType)
    {
        string rs = "";

        var parent = ShipmentModel.Where(x => x.Type == shipPartner).FirstOrDefault();
        if (parent != null)
        {
            rs += parent.Name;
            if (parent.ShipmentTypes.Count != 0)
            {
                var type = parent.ShipmentTypes.Where(x => x.Type == shipType).Select(x => x.Name).FirstOrDefault();
                if (type != null)
                {
                    rs += " - " + type;
                }
            }
        }

        return rs;
    }
}