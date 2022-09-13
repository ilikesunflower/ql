using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CMS_EF.DbContext;
using CMS_EF.Models.Ship;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Ship;

public interface IShipVnPostRepository : IBaseRepository<ShipVnPost>, IScoped
{
    ShipVnPost FindByStatus();
}

public class ShipVnPostRepository : BaseRepository<ShipVnPost>, IShipVnPostRepository
{
    private readonly ApplicationDbContext applicationDbContext;
    public ShipVnPostRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        this.applicationDbContext = applicationDbContext;
    }


    public ShipVnPost FindByStatus()
    {
        return this.applicationDbContext.ShipVnPost.FirstOrDefault(x => x.Flag == 0 && x.Status == true);
    }
}