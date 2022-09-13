using System;
using System.Collections.Generic;
using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Orders;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories.Orders;
public interface IOrderProductRateCommentRepository : IBaseRepository<OrderProductRateComment>, IScoped
{
    IQueryable<OrderProductRateComment> GetAllProductComment();
    bool ChangeStatus(int id, bool status);
    int DeleteAllCommentRate(List<int> ids);

    // IQueryable<OrderProductRateComment> GetOrderCommentDetails();
} 
public class OrderProductRateCommentRepository : BaseRepository<OrderProductRateComment>, IOrderProductRateCommentRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public OrderProductRateCommentRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

    public IQueryable<OrderProductRateComment> GetAllProductComment()
    {
        var rs = _applicationDbContext.OrderProductRateComment.Where(x => x.Flag == 0)
            .Include(x => x.Customer)
            .Include(x => x.Product)
            .Include(x => x.OrderProduct)
            .Include(x => x.Orders);
        var kk = rs.ToList();
        return rs;
    }

   // public IQueryable<OrderProductRateComment> GetOrderCommentDetails()
   //  {
   //      return 
   //  }

    public bool ChangeStatus(int id, bool status)
    {
        //thay đổi status ratecomment
        var comment = _applicationDbContext.OrderProductRateComment.Find(id);
        DateTime t = DateTime.Now;
        comment.Status = status;
        comment.LastModifiedAt = t;
        _applicationDbContext.OrderProductRateComment.Update(comment);
        //thay dổi product
        var product = _applicationDbContext.Products.Find(comment.ProductId);
        if (product != null)
        {
            if (status)
            {
                product.Rate = Math.Max(0, ((product.Rate == null ? 0 : product.Rate ) + comment.Rate ) ?? 0);
                product.RateCount = Math.Max(0, ((product.RateCount == null ? 0 : product.RateCount)  + 1) ?? 0);
                product.TotalComment =  Math.Max(0,((product.TotalComment == null ? 0 : product.TotalComment)  + 1) ?? 0);

            }
            else
            {
                product.Rate = Math.Max(0, ((product.Rate == null ? 0 : product.Rate) - comment.Rate ) ?? 0);
                product.RateCount = Math.Max(0, ((product.RateCount == null ? 0 : product.RateCount)  - 1) ?? 0);
                product.TotalComment =  Math.Max(0,((product.TotalComment == null ? 0 : product.TotalComment)  - 1) ?? 0);
            }
        }
        _applicationDbContext.Products.Update(product);

        _applicationDbContext.SaveChanges();
        return true;
    }

    public int DeleteAllCommentRate(List<int> ids)
    {
        var listProduct = new List<CMS_EF.Models.Products.Products>();
        foreach (var item in ids)
        {
            var comment = FindById(item);
            if (comment.Status ?? false)
            {
                var product = _applicationDbContext.Products.Find(comment.ProductId);
                if (product != null)
                {
                    product.Rate = Math.Max(0, ((product.Rate == null ? 0 : product.Rate) - comment.Rate ) ?? 0);
                    product.RateCount = Math.Max(0, ((product.RateCount == null ? 0 : product.RateCount)  - 1) ?? 0);
                    product.TotalComment =  Math.Max(0,((product.TotalComment == null ? 0 : product.TotalComment)  - 1) ?? 0);
                    listProduct.Add(product);
                }
            }
        }

        if (listProduct.Count > 0)
        {
            _applicationDbContext.Products.UpdateRange(listProduct);
        }

       var rs = DeleteAll(ids);
        return rs;
    }
}