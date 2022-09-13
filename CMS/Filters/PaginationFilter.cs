using CMS_Lib.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Filters
{
    public interface IPaginationFilter : IScoped
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public IPaginationFilter GetNextPage();

    }
    public class PaginationFilter: IPaginationFilter
    {
        private int pageNumber;
        private int pageSize;
        public int PageNumber
        {
            get => pageNumber;
            set => pageNumber = value < 1 ? 1 : value;
        }
        public int PageSize 
        {
            get => pageSize;
            set => pageSize = value < 20 ? 20 : value;
        }
        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 20;
        }
        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize < 20 ? 20 : pageSize;
        }

        public IPaginationFilter GetNextPage()
        {
            return new PaginationFilter(this.pageNumber + 1, this.pageSize);
        }



       
    }
}
