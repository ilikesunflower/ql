using System;
using System.Collections.Generic;
using System.Linq;
using CMS_EF.DbContext;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories.Orders;

public interface IOrdersRepository : IBaseRepository<CMS_EF.Models.Orders.Orders>, IScoped
{
    CMS_EF.Models.Orders.Orders FindByShipCode(string code);
    CMS_EF.Models.Orders.Orders FindByCode(string code);
    CMS_EF.Models.Orders.Orders FindByCodeIncludeProduct(string code);
    CMS_EF.Models.Orders.Orders FindByCodeWithProduct(string code);
    CMS_EF.Models.Orders.Orders FindByCodeWithProductAndPoint(string code);

    IQueryable<CMS_EF.Models.Orders.Orders> GetOrderIncludeProductAndAddressAndCustomer(string txtSearch,
        DateTime? start, DateTime? end, int? paymentStatus, int? status,bool isUsePoint);

    List<CMS_EF.Models.Orders.Orders> FindByCodes(List<string> codes);
}

public class OrdersRepository : BaseRepository<CMS_EF.Models.Orders.Orders>, IOrdersRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public OrdersRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

    public override CMS_EF.Models.Orders.Orders FindById(int id)
    {
        return _applicationDbContext.Orders.FirstOrDefault(x => x.Flag == 0 && x.Id == id);
    }

    public CMS_EF.Models.Orders.Orders FindByShipCode(string code)
    {
        return this._applicationDbContext.Orders.FirstOrDefault(x => x.Flag == 0 && x.CodeShip == code);
    }

    public CMS_EF.Models.Orders.Orders FindByCode(string code)
    {
        return this._applicationDbContext.Orders.Include(x => x.OrderAddress)
            .Include(x => x.OrderAddress.Province)
            .Include(x => x.OrderAddress.District)
            .Include(x => x.OrderAddress.Commune)
            .FirstOrDefault(x => x.Flag == 0 && x.Code == code);
    }

    public CMS_EF.Models.Orders.Orders FindByCodeIncludeProduct(string code)
    {
        var rs = _applicationDbContext.Orders
            .Where(x => x.Flag == 0 && x.Code == code)
            .Include(x => x.OrderAddress)
            .Include(x => x.OrderAddress.Province)
            .Include(x => x.OrderAddress.District)
            .Include(x => x.OrderAddress.Commune)
            .Include(x => x.OrderProduct.Where(z => z.Flag == 0))
            .ThenInclude(x => x.OrderProductSimilarProperty.Where(z => z.Flag == 0))
            .FirstOrDefault();
        return rs;
    }

    public CMS_EF.Models.Orders.Orders FindByCodeWithProduct(string code)
    {
        return _applicationDbContext.Orders
            .Where(x => x.Flag == 0 && x.Code == code)
            .Include(x => x.OrderProduct)
            .FirstOrDefault();
    }

    public CMS_EF.Models.Orders.Orders FindByCodeWithProductAndPoint(string code)
    {
        return _applicationDbContext.Orders
            .Where(x => x.Flag == 0 && x.Code == code)
            .Include(x => x.OrderProduct)
            .Include(x => x.OrderPoint)
            .FirstOrDefault();
    }

    public IQueryable<CMS_EF.Models.Orders.Orders> GetOrderIncludeProductAndAddressAndCustomer(string txtSearch,
        DateTime? start, DateTime? end, int? paymentStatus, int? status,bool isUsePoint)
    {
        var queryOrders = _applicationDbContext.Orders.Where(x => x.Flag == 0);
        if (!string.IsNullOrEmpty(txtSearch))
        {
            queryOrders = queryOrders.Where(x => EF.Functions.Like(x.Code, "%" + txtSearch.Trim() + "%"));
        }

        if (isUsePoint)
        {
            queryOrders = queryOrders.Where(x => x.Point != null && x.Point > 0 );
        }
        if (start != null)
        {
            queryOrders = queryOrders.Where(x => x.OrderAt >= start);
        }

        if (end != null)
        {
            queryOrders = queryOrders.Where(x => x.OrderAt <= end);
        }

        if (paymentStatus != null)
        {
            queryOrders = queryOrders.Where(x => x.StatusPayment == paymentStatus);
        }

        if (status != null)
        {
            queryOrders = queryOrders.Where(x => x.Status == status);
        }

        return queryOrders
            .Include(x => x.Customer)
            .Include(x => x.OrderAddress).ThenInclude(x => x.Province)
            .Include(x => x.OrderAddress).ThenInclude(x => x.District)
            .Include(x => x.OrderAddress).ThenInclude(x => x.Commune)
            .Include(x => x.OrderProduct);
    }

    public List<CMS_EF.Models.Orders.Orders> FindByCodes(List<string> codes)
    {
        return _applicationDbContext.Orders.Where(x => x.Flag == 0 && codes.Contains(x.Code)).ToList();
    }
}