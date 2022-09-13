using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using CMS_EF.DbContext;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories
{
    public interface IBaseRepository<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        T Create(T entity);
        List<T> CreateAll(List<T> entity);

        List<T> CreateAllBulk(List<T> entity);
        
        void Update(T entity);

        List<T> BulkUpdate(List<T> entity);
        void Delete(T entity, bool isSoftDelete = true);
        T FindById(int id);
        bool IsCheckById(int id);
        int DeleteAll(List<int> listId, bool isSoftDelete = true);
        int DeleteAll(List<T> entity, bool isSoftDelete = true);

        List<T> AttachRange(List<T> entity);

        ApplicationDbContext GetDbContext();
    }
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected IHttpContextAccessor Context { get; set; }

        protected int UserId { get; set; }

        public BaseRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context)
        {
            this.ApplicationDbContext = applicationDbContext;
            this.Context = context;
            UserId = Int32.Parse(context?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        }

        public virtual IQueryable<T> FindAll()
        {
            var f1 = typeof(T).GetProperties().FirstOrDefault(x => x.Name == "Flag");
            if (f1 != null)
            {
                var ftParameter = Expression.Parameter(typeof(T));
                var ftFlagProperty = Expression.Property(ftParameter, "Flag");
                var ftFlagClause = Expression.Equal(ftFlagProperty, Expression.Constant(0));
                return this.ApplicationDbContext.Set<T>().Where(Expression.Lambda<Func<T, bool>>(ftFlagClause, ftParameter)).AsNoTracking();
            }
            return this.ApplicationDbContext.Set<T>().AsNoTracking();
        }

        public virtual IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.ApplicationDbContext.Set<T>().Where(expression).AsNoTracking();
        }

        public virtual T Create(T entity)
        {
            this.ApplicationDbContext.Set<T>().Add(entity);
            this.ApplicationDbContext.SaveChanges();
            return entity;
        }

        public List<T> CreateAllBulk(List<T> entity)
        {
            this.ApplicationDbContext.BulkInsert(entity);
            return entity;
        }

        public virtual void Update(T entity)
        {
            this.ApplicationDbContext.Set<T>().Update(entity);
            this.ApplicationDbContext.SaveChanges();
        }

        public List<T> BulkUpdate(List<T> entity)
        {
            this.ApplicationDbContext.Set<T>().UpdateRange(entity);
            this.ApplicationDbContext.SaveChanges();
            // var bulkConfig = new BulkConfig { SetOutputIdentity = true};
            // this.ApplicationDbContext.BulkUpdate(entity, bulkConfig);
            return entity;
        }
        

        public virtual void Delete(T entity, bool isSoftDelete = true)
        {
            if (isSoftDelete)
            {
                var f1 = typeof(T).GetProperties().FirstOrDefault(x => x.Name == "Flag");
                if (f1 != null)
                {
                    f1.SetValue(entity, -1);
                    this.ApplicationDbContext.Set<T>().Update(entity);
                    this.ApplicationDbContext.SaveChanges();
                }
                else
                {
                    this.ApplicationDbContext.Set<T>().Remove(entity);
                    this.ApplicationDbContext.SaveChanges();
                }
            }
            else
            {
                this.ApplicationDbContext.Set<T>().Remove(entity);
                this.ApplicationDbContext.SaveChanges();
            }
        }

        public virtual T FindById(int id)
        {
            var f1 = typeof(T).GetProperties().FirstOrDefault(x => x.Name == "Flag");
            if (f1 != null)
            {
                var ftParameter = Expression.Parameter(typeof(T));
                var ftFlagProperty = Expression.Property(ftParameter, "Flag");
                var ftFlagClause = Expression.Equal(ftFlagProperty, Expression.Constant(0));
                var idName = ApplicationDbContext.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Single().Name;
                var ftIdProperty = Expression.Property(ftParameter, idName);
                var ftIdClause = Expression.Equal(ftIdProperty, Expression.Constant(id));
                return this.ApplicationDbContext.Set<T>().Where(Expression.Lambda<Func<T, bool>>(ftIdClause, ftParameter)).Where(Expression.Lambda<Func<T, bool>>(ftFlagClause, ftParameter)).FirstOrDefault();
            }
            return this.ApplicationDbContext.Set<T>().Find(id);
        }

        public virtual bool IsCheckById(int id)
        {
            var f1 = typeof(T).GetProperties().FirstOrDefault(x => x.Name == "Flag");
            var ftParameter = Expression.Parameter(typeof(T));
            var idName = ApplicationDbContext.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Single().Name;
            var ftIdProperty = Expression.Property(ftParameter, idName);
            var ftIdClause = Expression.Equal(ftIdProperty, Expression.Constant(id));
            if (f1 != null)
            {
                var ftFlagProperty = Expression.Property(ftParameter, "Flag");
                var ftFlagClause = Expression.Equal(ftFlagProperty, Expression.Constant(0));
                return this.ApplicationDbContext.Set<T>().Where(Expression.Lambda<Func<T, bool>>(ftFlagClause, ftParameter)).Any(Expression.Lambda<Func<T, bool>>(ftIdClause, ftParameter));
            }
            else
            {
                return this.ApplicationDbContext.Set<T>()
                    .Any(Expression.Lambda<Func<T, bool>>(ftIdClause, ftParameter));
            }
        }

        public virtual int DeleteAll(List<int> listId, bool isSoftDelete = true)
        {
            var idName = ApplicationDbContext.Model.FindEntityType(typeof(T))!.FindPrimaryKey()!.Properties.Single().Name;
            List<T> data = ApplicationDbContext.Set<T>().Where(x => listId.Contains(EF.Property<int>(x, idName))).ToList();
            return DeleteAll(data, isSoftDelete);
        }

        public int DeleteAll(List<T> entity, bool isSoftDelete = true)
        {
            if (isSoftDelete)
            {
                var checkFlag = typeof(T).GetProperties().FirstOrDefault(x => x.Name == "Flag");
                if (checkFlag != null)
                {
                    entity.ForEach(item =>
                    {
                        var f1 = item.GetType().GetProperties().FirstOrDefault(x => x.Name == "Flag");
                        if (f1 != null)
                        {
                            f1.SetValue(item, -1);
                        }
                    });
                    this.ApplicationDbContext.Set<T>().UpdateRange(entity);
                    this.ApplicationDbContext.SaveChanges();
                }
                else
                {
                    this.ApplicationDbContext.Set<T>().RemoveRange(entity);
                    this.ApplicationDbContext.SaveChanges();
                }
            }
            else
            {
                this.ApplicationDbContext.Set<T>().RemoveRange(entity);
                this.ApplicationDbContext.SaveChanges();
            }
            return entity.Count;
        }

        public List<T> CreateAll(List<T> entity)
        {
            this.ApplicationDbContext.Set<T>().AddRange(entity);
            this.ApplicationDbContext.SaveChanges();
            return entity;
        }
        
        public List<T> AttachRange(List<T> entity)
        {
            this.ApplicationDbContext.Set<T>().AttachRange(entity);
            this.ApplicationDbContext.SaveChanges();
            return entity;
        }

        public ApplicationDbContext GetDbContext()
        {
            return this.ApplicationDbContext;
        }
    }
}
