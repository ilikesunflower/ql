using System.Collections.Generic;

namespace CMS.Areas.Categories.Models
{
    public class ItemCategoryViewModel
    {
        public List<CMS_EF.Models.Products.ProductCategory> ListCategory  { get; set; }
        
        public int? Pid { get; set; }
    }
}