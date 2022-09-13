using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Categories;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Categories;

public interface IProvinceRepository : IBaseRepository<Province>, IScoped
{
    public Province? FindByCode(string code);
}
public class ProvinceRepository : BaseRepository<Province>, IProvinceRepository
{
    private readonly ApplicationDbContext applicationDbContext;

    public ProvinceRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        this.applicationDbContext = applicationDbContext;
    } 
    public Province? FindByCode(string code)
    {
        return applicationDbContext.Province?.FirstOrDefault(x => x.Flag == 0 && x.Code == code);
    }
}