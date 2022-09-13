namespace CMS_App_Api.Helpers.Consts
{
    public class MessageCode
    {
        private MessageCode(int code, string value)
        {
            Value = value;
            Code = code;
        }
        public string Value { get; set; }
        public int Code { get; set; }

        public static MessageCode Success => new MessageCode(200, "successful");
        public static MessageCode UserEmpty => new MessageCode(201, "Tài khoản hoặc mật khẩu không đúng");
        public static MessageCode LockedOut => new MessageCode(202, "Tài khoản bị đã bị khóa, liên hệ quản trị viên");
        public static MessageCode BadRequest => new MessageCode(400, "Dữ liệu không hợp lệ");
        public static MessageCode Unauthorized => new MessageCode(401, "Phiên làm việc đã hết, đăng nhập lại hệ thống");
        public static MessageCode LoginErr => new MessageCode(500,"Lỗi xác thực không thành công");
        public static MessageCode SecurityTokenExpired => new MessageCode(419, "TokenExpired");
        public static MessageCode System => new MessageCode(500, "Lỗi hệ thống");
    }
}
