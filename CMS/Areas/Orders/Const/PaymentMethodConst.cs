using System.Collections.Generic;
using System.Linq;

namespace CMS.Areas.Orders.Const;

public class PaymentMethodConst
{

    public static PaymentMethodConst COD => new(0, "Thanh toán khi nhận hàng");
    public static PaymentMethodConst Credit => new(1, "Thẻ tín dụng");
    public static PaymentMethodConst Debit => new(2, "Chuyển khoản Ngân hàng");
    public static PaymentMethodConst Prudential => new(3, "Tổng giá trị đơn hàng sẽ được Daiichi thanh toán từ ngân sách của phòng ban");
    
    public static List<PaymentMethodConst> PaymentMethods = new()
    {
        COD,Credit,Debit,Prudential
    };

    public static string BindPaymentMethod(int status)
    {
        var methodConst = PaymentMethods.FirstOrDefault( x => x.Status == status );
        return methodConst!.Name ?? "";
    }

    private PaymentMethodConst(int status, string name)
    {
        Status = status;
        Name = name;
    }

    public int Status { get; }
    public string Name { get; }
}