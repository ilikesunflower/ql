using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Categories;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Categories;

public interface IDistrictRepository : IBaseRepository<District>, IScoped
{
    public District? FindByCode(string districtCode);

}
public class DistrictRepository : BaseRepository<District>, IDistrictRepository
{
    private readonly ApplicationDbContext applicationDbContext;

    public DistrictRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        this.applicationDbContext = applicationDbContext;

    }
    public District? FindByCode(string districtCode)
    {
        return applicationDbContext.District!.FirstOrDefault(x => x.Flag == 0 && x.Code == districtCode);
    }
}