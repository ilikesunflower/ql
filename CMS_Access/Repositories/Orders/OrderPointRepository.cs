using CMS_EF.DbContext;
using CMS_EF.Models.Orders;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Orders;


public interface IOrderPointRepository: IBaseRepository<OrderPoint>, IScoped
{
    
}
public class OrderPointRepository: BaseRepository<OrderPoint>,IOrderPointRepository
{
    private ApplicationDbContext _applicationDbContext;
    public OrderPointRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }
    
    
    
}