using System.Collections.Generic;

namespace CMS.Areas.Admin.ViewModels.Home;

public class CharDataModel
{
    public List<string> Categories { get; set; }
    public List<double> Prices { get; set; }
    public string FilterStatus { get; set; }
}
public class SeriesChart
{
    public string Name { get; set; }
    public double Y { get; set; }
}

public class SeriesCharArea
{
    public int? AreaId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public double Quantity { get; set; }
}