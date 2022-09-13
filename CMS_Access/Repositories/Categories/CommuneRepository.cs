using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Categories;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Categories;

public interface ICommuneRepository : IBaseRepository<Commune>, IScoped
{
    public Commune? FindByCode(string districtCode);

}
public class CommuneRepository : BaseRepository<Commune>, ICommuneRepository
{
    private readonly ApplicationDbContext applicationDbContext;

    public CommuneRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        this.applicationDbContext = applicationDbContext;

    }
    public Commune? FindByCode(string districtCode)
    {
        return applicationDbContext.Commune!.FirstOrDefault(x => x.Flag == 0 && x.Code == districtCode);
    }
}