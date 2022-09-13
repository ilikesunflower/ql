using System.Collections.Generic;
using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories.Customers;

public interface ICustomerCardRepository : IBaseRepository<CustomerCard>, IScoped
{
    List<CustomerCard> FindByIds(List<int?> productSimilarIds,int? customerId);
}

public class CustomerCardRepository: BaseRepository<CustomerCard>, ICustomerCardRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public CustomerCardRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

    public List<CustomerCard> FindByIds(List<int?> productSimilarIds,int? customerId)
    {
        return _applicationDbContext.CustomerCard.Where(x => x.Flag == 0 && productSimilarIds.Contains(x.ProductSimilarId) && x.CustomerId == customerId ).ToList();
    }
}