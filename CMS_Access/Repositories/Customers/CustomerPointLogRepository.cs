using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Customers;

public interface ICustomerPointLogRepository : IBaseRepository<CustomerPointLog>, IScoped
{
}

public class CustomerPointLogRepository: BaseRepository<CustomerPointLog>, ICustomerPointLogRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public CustomerPointLogRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

}