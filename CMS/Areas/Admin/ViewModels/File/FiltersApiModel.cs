using CMS.Filters;
using CMS.Services.Uris;
using CMS_EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.Admin.ViewModels.File
{
    public class FiltersApiModel
    {
        public  IDictionary<string, string> ListTypes { set; get; }
        public List<string> ListCreatedAt { set; get; }
        
    }
}
