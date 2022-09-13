using CMS_Access.Repositories.Customers;
using CMS_EF.DbContext;
using CMS_EF.Models.Orders;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories.Orders;
public interface IOrderProductRepository : IBaseRepository<OrderProduct>, IScoped
{
    
}
public class OrderProductRepository: BaseRepository<OrderProduct>, IOrderProductRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public OrderProductRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }
    
}