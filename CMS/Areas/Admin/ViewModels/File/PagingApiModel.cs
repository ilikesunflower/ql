using CMS.Filters;
using CMS.Services.Uris;
using CMS_EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.Admin.ViewModels.File
{
    public class PagingApiModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public Uri NextPage { get; set; }
        public List<Files> ListFiles { get; set; }
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
        public string msg { get; set; }
        public PagingApiModel(List<Files> data, IPaginationFilter validFilter , Uri nextPage, string Msg)
        {
            this.PageNumber = validFilter.PageNumber;
            this.PageSize = validFilter.PageSize;
            this.ListFiles = data;
            this.msg = Msg;
            this.Succeeded = true;
            this.Errors = null;
            this.NextPage = nextPage;
        }
    }
}
