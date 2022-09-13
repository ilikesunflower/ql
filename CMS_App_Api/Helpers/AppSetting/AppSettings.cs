namespace CMS_App_Api.Helpers.AppSetting
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int TokenExpires { get; set; }
        public int RefreshTokenExpires { get; set; }
        public int DefaultLockoutTimeSpan { get; set; }
    }
}
