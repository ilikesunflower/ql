using System.Collections.Generic;

namespace CMS.Areas.Orders.Models;

public class ShipmentPartner
{
    public string Name { set; get; }
    public int Type { set; get; }
    public List<ShipmentType> ShipmentTypes { set; get; }
}