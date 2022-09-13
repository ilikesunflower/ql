using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Customers;

public interface ICustomerRepository : IBaseRepository<Customer>, IScoped
{
    Customer FindByUserName(string userName);
}

public class CustomerRepository :BaseRepository<Customer>, ICustomerRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public CustomerRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

    public Customer FindByUserName(string userName)
    {
        return _applicationDbContext.Customer.FirstOrDefault(x => x.Flag == 0 && x.UserName.ToLower() == userName.ToLower());
    }
}