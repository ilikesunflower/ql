using System.Collections.Generic;
using System.Linq;
using Novacode;

namespace CMS.Areas.Orders.Const;

public class PreOrderStatus
{
    public static PreOrderStatus NotProcessedYet => new(0,"Chưa được xử lý");
    
    public static PreOrderStatus Confirmed => new(1,"Đã xác nhận và tạo đơn");

    public static PreOrderStatus Cancel => new(2, "Đã hủy");

    public PreOrderStatus(int status,string name)
    {
        Name = name;
        Status = status;
    }

    public static List<PreOrderStatus> PreOrderStatusList = new()
    {
        new PreOrderStatus(-1, "Tất cả trạng thái"),
        NotProcessedYet,
        Confirmed,
        Cancel
    };

    public static string BindStatus(int status)
    {
        PreOrderStatus select = PreOrderStatusList.FirstOrDefault( x => x.Status == status );
        switch (select?.Status ?? 0)
        {
            case 0:
                return $"<span class='status badge bg-secondary text-white'>{select?.Name}</span>";
            case 1:
                return $"<span class='status badge bg-success text-white'>{select?.Name}</span>";
            default:
                return $"<span class='status badge bg-warning text-white'>{select?.Name}</span>";
        }
    }
    
    public int Status { get; set; }
    public string Name { get; set; }
}