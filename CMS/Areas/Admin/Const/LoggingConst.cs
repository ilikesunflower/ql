namespace CMS.Areas.Admin.Const
{
    public class LoggingConst
    {
        public static string BindDataType(int type)
        {
            string msg = "";
            switch (type)
            {
                case 1:
                    msg = "<small class=\"badge bg-primary badge-sm\">Thông báo</small>";
                    break;
                case 2:
                    msg = "<small class=\"badge bg-danger badge-sm\">Lỗi</small>";
                    break;
                case 3:
                    msg = "<small class=\"badge bg-success badge-sm\">Hệ thống</small>";
                    break;
            }
            return msg;
        }
    }
}
