namespace CMS.Models.Base
{
    public class IndexViewBase
    {
        private int _pageSize = Startup.PageSize;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }
    }
}
