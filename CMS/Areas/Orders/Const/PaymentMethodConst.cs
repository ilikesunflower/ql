using System.Collections.Generic;
using System.Linq;

namespace CMS.Areas.Orders.Const;

public class PaymentMethodConst
{

    public static PaymentMethodConst COD => new(0, "Thanh toán khi nhận hàng","Thanh toán khi nhận hàng");
    // public static PaymentMethodConst Credit => new(1, "Thẻ tín dụng","Chuyển khoản Ngân hàng");
    public static PaymentMethodConst Debit => new(2, "Chuyển khoản Ngân hàng","Chuyển khoản Ngân hàng");
    // public static PaymentMethodConst Prudential => new(3, "Tổng giá trị đơn hàng sẽ được Daiichi thanh toán từ ngân sách của phòng ban");
    
    public static List<PaymentMethodConst> PaymentMethods = new()
    {
        COD,Debit
    };

    public static string BindPaymentMethod(int status)
    {
        var methodConst = PaymentMethods.FirstOrDefault( x => x.Status == status );
        return methodConst!.Name ?? "";
    }
    
    public static string BindStatus(int? status)
    {
        if (status == PaymentMethodConst.COD.Status)
        {
            return $"<span class='status badge bg-info text-white'>{PaymentMethodConst.COD.NameChart}</span>";
        }else if (status == PaymentMethodConst.Debit.Status)
        {
            return $"<span class='status badge bg-primary text-white'>{PaymentMethodConst.Debit.NameChart}</span>";
        }
        // else if (status == PaymentMethodConst.Prudential.Status)
        // {
        //     return $"<span class='status badge bg-success text-white'>{PaymentMethodConst.Prudential.NameChart}</span>";
        // }
        return "";
    }

    private PaymentMethodConst(int status, string name, string nameChart)
    {
        Status = status;
        Name = name;
        NameChart = nameChart;
    }

    public int Status { get; }
    public string Name { get; }
    
   public string NameChart { get; }

}