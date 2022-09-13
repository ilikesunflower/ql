using CMS_EF.DbContext;
using CMS_Lib.DI;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Access.Repositories
{
    public interface IDatabaseTransaction : IScoped
    {
        IDbContextTransaction BeginTransaction();
        void Commit(IDbContextTransaction transaction);
        void Rollback(IDbContextTransaction transaction);
    }
    public class EntityDatabaseTransaction : IDatabaseTransaction
    {
        private readonly DatabaseFacade _database;

        public EntityDatabaseTransaction(ApplicationDbContext context)
        {
            _database = context.Database;
        }
        public IDbContextTransaction BeginTransaction()
        {
            return _database.BeginTransaction();
        }
        public void Commit(IDbContextTransaction transaction)
        {
            transaction.Commit();
        }

        public void Rollback(IDbContextTransaction transaction)
        {
            transaction.Commit();
        }
    }
}
