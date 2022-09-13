namespace CMS_Ship.Consts;

public class VnPostConst
{
    public static string VnPostStandard = "DONG_GIA";
    public static string VnPostExpress = "TMDT_EMS";

    public static string GetServiceName(int typeShip)
    {
        if (typeShip == TypeShipConst.Express)
        {
            return VnPostExpress;
        }else if (typeShip == TypeShipConst.Standard)
        {
            return VnPostStandard;
        }

        return null;
    }

    public static int GetServiceNameToType(string serviceName)
    {
        if (string.IsNullOrEmpty(serviceName))
        {
            return 0;
        }

        if (serviceName == VnPostStandard)
        {
            return TypeShipConst.Standard;
        }else if (serviceName == VnPostExpress)
        {
            return TypeShipConst.Express;
        }

        return 0;
    }
    
}