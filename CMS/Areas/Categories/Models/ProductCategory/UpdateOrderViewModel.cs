using System.Collections.Generic;

namespace CMS.Areas.Categories.Models.ProductCategory
{
    public class UpdateOrderViewModel
    {
        public List<int> Ids { set; get; }
        public int Parent { set; get; }
    }
}