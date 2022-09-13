using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.WareHouse;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.WareHouse;

public interface IWhTransactionRepository : IBaseRepository<WhTransaction>, IScoped
{
    WhTransaction FindByOrderIdWh(int orderId, int orderIdWh);
    WhTransaction FindByOrderIdWhStatus(int orderId, int orderIdWh, int status);
    WhTransaction FindByOrderIdStatus(int orderId, int status);
}

public class WhTransactionRepository : BaseRepository<WhTransaction>, IWhTransactionRepository
{
    private readonly ApplicationDbContext applicationDbContext;

    public WhTransactionRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        this.applicationDbContext = applicationDbContext;
    }

    public WhTransaction FindByOrderIdWh(int orderId, int orderIdWh)
    {
        return this.applicationDbContext.WhTransaction!.FirstOrDefault(x =>
            x.OrderId == orderId && x.OrderIdKiot == orderIdWh);
    }

    public WhTransaction FindByOrderIdWhStatus(int orderId, int orderIdWh, int status)
    {
        return this.applicationDbContext.WhTransaction!.FirstOrDefault(x =>
            x.OrderId == orderId && x.OrderIdKiot == orderIdWh && x.Status == status );
    }

    public WhTransaction FindByOrderIdStatus(int orderId, int status)
    {
        return this.applicationDbContext.WhTransaction!.FirstOrDefault(x => x.OrderId == orderId && x.Status == status);
    }
}