namespace CMS_Lib.Util
{
    public sealed class CmsClaimType
    {
        public const string UserName = "UserName";
        public const string Avatar = "Avatar";
        public const string MaxFileSize = "MaxFileSize";
        public const string ClaimType = "ClaimType";
        public const string UserType = "UserType";
        public const string IsActiveUser = "IsActiveUser";
        public const string ControllerAction = "ControllerAction";
        public const string AreaControllerAction = "CONTROLLER@ACTION";
        public const string AppSetting = "AppSetting";
        public const string Controller = "Controller";
        public const string Menu = "MENU";
        public const string MenuActive = "MenuActive";

        public static bool HasControllerActionUser()
        {
            return false;
        }
    }
}
