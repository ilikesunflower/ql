using System.Collections.Generic;
using System.Linq;
using CMS.Areas.Orders.Const;
using CMS_Ship.Models;

namespace CMS.Areas.Orders.Models;

public class ShipmentViewModel
{
    public ShipmentViewModel(List<CalculateFee> ghnCost, List<CalculateFee> vnPostCost)
    {
        ShipmentPartners = new List<ShipmentPartner>
        {
            new ShipmentPartner
            {
                Name = "GHN",
                Type = 1,
                ShipmentTypes = (from shipmentTypes in ShipmentCost.ShipmentTypes
                    join calculateFee in ghnCost on shipmentTypes.Type equals calculateFee.TypeService into calculateFee
                    from calculateFeeDefault in calculateFee.DefaultIfEmpty()
                    select new ShipmentType
                    {
                        Name = shipmentTypes.Name,
                        Icon = shipmentTypes.Icon,
                        Type = shipmentTypes.Type,
                        Cost = calculateFeeDefault?.Total
                    }).ToList()
            },
            // new ShipmentPartner
            // {
            //     Name = "VNPost",
            //     Type = 2,
            //     ShipmentTypes = (from shipmentTypes in ShipmentCost.ShipmentTypeEs
            //         join calculateFee in vnPostCost on shipmentTypes.Type equals calculateFee.TypeService into
            //             calculateFee
            //         from calculateFeeDefault in calculateFee.DefaultIfEmpty()
            //         select new ShipmentType
            //         {
            //             Name = shipmentTypes.Name,
            //             Icon = shipmentTypes.Icon,
            //             Type = shipmentTypes.Type,
            //             Cost = calculateFeeDefault?.Total
            //         }).ToList()
            // },
            new ShipmentPartner
            {
                Name = "Nhận hàng tại kho",
                Type = 0,
                ShipmentTypes = new List<ShipmentType>()
            },
            new ShipmentPartner()
            {
                Name = "Đối tác khác",
                Type = 3,
                ShipmentTypes = new List<ShipmentType>()
            }
        };
    }

    public List<ShipmentPartner> ShipmentPartners { set; get; }
}