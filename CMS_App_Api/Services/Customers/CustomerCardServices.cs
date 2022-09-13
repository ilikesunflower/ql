using CMS_Access.Repositories.Customers;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using System.Collections.Generic;

namespace CMS_App_Api.Services.Customers;
public interface ICustomerCardServices : IScoped
{
    void Paid(List<int?> productSimilarIds,int? customerId);
}
public class CustomerCardServices : ICustomerCardServices
{
    private readonly ICustomerCardRepository _customerCardRepository;

    public CustomerCardServices(ICustomerCardRepository customerCardRepository)
    {
        _customerCardRepository = customerCardRepository;
    }

    public void Paid(List<int?> productSimilarIds,int? customerId)
    {
        List<CustomerCard> paidProducts = _customerCardRepository.FindByIds(productSimilarIds,customerId);
        _customerCardRepository.DeleteAll(paidProducts, false);
    }
}