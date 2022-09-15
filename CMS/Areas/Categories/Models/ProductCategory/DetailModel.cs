namespace CMS.Areas.Categories.Models.ProductCategory;

public class DetailModel
{
    public string Parent { get; set; }
    public CMS_EF.Models.Products.ProductCategory ProductCategory { get; set; }
}