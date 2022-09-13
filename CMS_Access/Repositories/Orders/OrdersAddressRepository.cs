using CMS_Access.Repositories.Customers;
using CMS_EF.DbContext;
using CMS_EF.Models.Orders;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories.Orders;
public interface IOrdersAddressRepository : IBaseRepository<OrderAddress>, IScoped
{
    
}
public class OrdersAddressRepository: BaseRepository<OrderAddress>, IOrdersAddressRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public OrdersAddressRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }
    
}