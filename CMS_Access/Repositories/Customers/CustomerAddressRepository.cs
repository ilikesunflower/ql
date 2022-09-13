using System.Collections.Generic;
using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories.Customers;

public interface ICustomerAddressRepository : IBaseRepository<CustomerAddress>, IScoped
{
    CustomerAddress GetValueCustomerAddress(int id);
}
public class CustomerAddressRepository : BaseRepository<CustomerAddress>, ICustomerAddressRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public CustomerAddressRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }
    public  CustomerAddress GetValueCustomerAddress(int id)
    {
        var data = _applicationDbContext.CustomerAddress.Where(x => x.Id == id)
            .Include(x => x.ProvinceCodeNavigation)
            .Include(x => x.DistrictCodeNavigation)
            .Include(x => x.CommuneCodeNavigation).FirstOrDefault();
        return data;
    }
}