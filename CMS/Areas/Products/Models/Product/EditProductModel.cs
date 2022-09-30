using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;
using Microsoft.AspNetCore.Http;

namespace CMS.Areas.Products.Models.Product;

public class EditProductModel
{
    public int Id { get; set; }

    [MaxLength(250, ErrorMessage = "Mã hàng phải nhỏ hơn 250 kí tự!")]
    [ValidXss]
    [Required(ErrorMessage = "Vui lòng nhập mã hàng.")]
    public string Sku { get; set; }

    [ValidXss]
    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
    public string Name { get; set; }

    public double Weight { get; set; }

    public int Price { get; set; }
    public int PriceSale { get; set; }

    [ValidScript] public string Description { get; set; }

    [ValidScript] public string Lead { get; set; }

    [ValidScript] public string Specifications { get; set; }

    public int ProductPurposeId { get; set; }

    [ValidXss] public string Unit { get; set; }

    public IFormFile Image { get; set; }
    public bool IsHot { get; set; }
    public bool IsNew { get; set; }

    public bool IsBestSale { get; set; }
    public bool IsPromotion { get; set; }
    public bool CheckEdit { get; set; }
    public bool IsPublic { get; set; }
    public int ProductSex { get; set; }
    public int ProductAge { get; set; }

    public List<IFormFile> Images { get; set; }
    public List<int> ProductCategory { get; set; }

    public int QuantityStock { get; set; }

    [ValidXss] public string CodeStock { get; set; }

    [ValidXss] public string Name1 { get; set; }

    [ValidXss] public string Name2 { get; set; }

    [ValidXss] public string Name3 { get; set; }

    [ValidXss] public List<string> ImageList { get; set; }

    [ValidXss] public List<string> Properties1 { get; set; }

    [ValidXss] public List<string> Properties2 { get; set; }

    [ValidXss] public List<string> Properties3 { get; set; }

    [ValidXss] public List<string> ListSkuMh { get; set; }

    [ValidXss] public List<string> ListName { get; set; }

    public List<double> ListPrice { get; set; }

    public List<int> ListQuantity { get; set; }
}