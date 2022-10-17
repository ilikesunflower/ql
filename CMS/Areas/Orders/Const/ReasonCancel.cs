using System.Collections.Generic;

namespace CMS.Areas.Orders.Const;

public class ReasonCancel
{
    public static List<ModelReason> ListReasonCancel = new List<ModelReason>()
    {
       new ModelReason
       {
           Type=  0,
           Name = "Không chọn"
       }, new ModelReason
       {
           Type=  1,
           Name = "Hủy đặt thêm sản phẩm"
       }, new ModelReason
       {
           Type=  2,
           Name = "Hủy đổi hình thức thanh toán"
       }, new ModelReason
       {
           Type=  3,
           Name = "Hủy cần hàng gấp"
       }, new ModelReason
       {
           Type=  4,
           Name = "Hủy đổi phương thức vận chuyển"
       },
    };

    public class ModelReason
    {
        public string Name { get; set; }
        public int Type { get; set; }
    }


}