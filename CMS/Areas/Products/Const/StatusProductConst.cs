using CMS.Areas.Categories.Const;
using CMS_Access.Repositories.Products;

namespace CMS.Areas.Products.Const;

public class StatusProductConst
{
   public static string BindStatus(ProductIndex product)
    {
        if (product.IsPublic == true)
        {
            return StatusConst.BindStatusText(product.IsPublic);
        }
        var status = "";
        var pending = ProductCensorshipConst.Pending.Status;
        var approved = ProductCensorshipConst.Approved.Status;
        var notApproved = ProductCensorshipConst.NotApproved.Status;
        if (product.Org1Status == pending)
        {
            status += "Chờ duyệt giá, mã SP . ";
        }
        if (product.Org1Status == approved)
        {
            status += "Đã duyệt giá, mã SP> .";
        }
        if (product.Org1Status == notApproved)
        {
            status += "Xem lại giá, mã SP .";
        }
        if (product.Org2Status == pending)
        {
            status += " Chờ duyệt nội dung SP .";
        }
        if (product.Org2Status == approved)
        {
            status += " Đã duyệt nội dung SP .";
        }
        if (product.Org2Status == notApproved)
        {
            status += " Xem lại nội dung SP .";
        }
        if (product.Org3Status == pending)
        {
            status += " Chờ duyệt hình ảnh, màu sắc, thương hiệu .";
        }
        if (product.Org3Status == approved)
        {
            status += " Đã duyệt hình ảnh, màu sắc, thương hiệu .";
        }
        if (product.Org3Status == notApproved)
        {
            status += " Xem lại hình ảnh, màu sắc, thương hiệu .";
        }
        return status;
    }
}