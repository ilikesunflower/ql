using System.Collections.Generic;

namespace CMS.Areas.Orders.Models;

public class ProductCartModel
{
    public int ProductId { get; set; }
    public string Image { get; set; }
    public string NameProduct { get; set; }
    public int ProductSimilarId { get; set; }
    public int? QuantityWH { get; set; }
    public double? Price { get; set; }
    public int QuantityBy { get; set; }
    public int Ord { get; set; }
    public double? Weight { get; set; }
    public List<PropertiesModel> ListProperties { get; set; }
}

public class PropertiesModel
{
    public int PropertiesValueId { get; set; }
    public string PropertiesName { get; set; }
    public string PropertiesValueName { get; set; }
}

public class ProductSimilarModel
{
    public int Id { get; set; }

    public int QuantityBy { get; set; }
    public int Order { get; set; }
    public int? Price { get; set; }
}