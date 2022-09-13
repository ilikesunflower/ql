using System;
using System.Collections.Generic;
using CMS.Areas.Admin.Const;
using Microsoft.Extensions.Configuration;

namespace CMS.Areas.Admin.ViewModels.Home
{
    public class IndexViewModel
    {
        public bool IsDataSales { get; set; }
        public bool IsProductBest { get; set; }
        public bool IsGroupCustomer { get; set; }
        public bool IsDataAreas { get; set; }
        public bool IsProductRating { get; set; }
        public bool IsCustomerActive { get; set; }
    }
}
