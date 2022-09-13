using System;
using System.Collections.Generic;
using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Identity;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace CMS_Access.Repositories
{
    public interface IApplicationUserRepository : IBaseRepository<ApplicationUser>, IScoped
    {
        int UpdateApplicationUser(ApplicationUser user, List<RoleInput> listRole, int id);
        public int UpdateProfileUser(ApplicationUser userProfileViewModel, int id);

        int InsertApplicationUser(ApplicationUser user, List<RoleInput> listRole);

        bool DeleteApplicationUser(int id);

        ApplicationUser FindByUsername(String userName);
        ApplicationUser FindByEmail(String email);

        void SaveUserToken(int userId, string token, string type);
        void DeleteUserToken(int userId, string type);
    }

    public class ApplicationUserRepository : BaseRepository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ApplicationUserRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context,
            UserManager<ApplicationUser> userManager) : base(applicationDbContext, context)
        {
            this.userManager = userManager;
        }

        public override IQueryable<ApplicationUser> FindAll()
        {
            return ApplicationDbContext.Users.Where(x => x.Flag == 0);
        }

        public int UpdateApplicationUser(ApplicationUser userViewModel, List<RoleInput> listRole, int id)
        {
            using IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                ApplicationUser user = ApplicationDbContext.Users.FirstOrDefault(x => x.Id == id && x.Flag == 0);
                if (user != null)
                {
                    user.Address = userViewModel.Address;
                    user.BirthDay = userViewModel.BirthDay;
                    user.FullName = userViewModel.FullName;
                    user.Image = userViewModel.Image;
                    user.Sex = userViewModel.Sex;
                    user.Email = userViewModel.Email;
                    user.PhoneNumber = userViewModel.PhoneNumber;
                    user.IsActive = userViewModel.IsActive;
                    if (listRole != null && listRole.Count > 0)
                    {
                        List<ApplicationUserRole> insertUserRole = new List<ApplicationUserRole>();
                        List<ApplicationUserRole> deleteUserRole = new List<ApplicationUserRole>();
                        foreach (var item in listRole)
                        {
                            if (item.IsSelected)
                            {
                                if (!ApplicationDbContext.UserRoles.Any(x =>
                                        x.UserId == user.Id && x.RoleId == item.Id))
                                {
                                    ApplicationUserRole insert = new ApplicationUserRole
                                    {
                                        RoleId = item.Id,
                                        UserId = user.Id
                                    };
                                    insertUserRole.Add(insert);
                                }
                            }
                            else
                            {
                                var o = ApplicationDbContext.UserRoles.FirstOrDefault(x =>
                                    x.UserId == user.Id && x.RoleId == item.Id);
                                if (o != null)
                                {
                                    deleteUserRole.Add(o);
                                }
                            }
                        }

                        if (insertUserRole.Count > 0)
                        {
                            ApplicationDbContext.UserRoles.AddRange(insertUserRole);
                            ApplicationDbContext.SaveChanges();
                        }

                        if (deleteUserRole.Count > 0)
                        {
                            ApplicationDbContext.UserRoles.RemoveRange(deleteUserRole);
                            ApplicationDbContext.SaveChanges();
                        }
                    }

                    ApplicationDbContext.Users.Update(user);
                    ApplicationDbContext.SaveChanges();
                    if (!string.IsNullOrEmpty(userViewModel.PasswordHash))
                    {
                        var checkpass =
                            userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash,
                                userViewModel.PasswordHash);
                        if (checkpass == PasswordVerificationResult.Success)
                        {
                            transaction.Rollback();
                            return -3; // Mật khẩu đã được sử dụng
                        }

                        string resetToken = userManager.GeneratePasswordResetTokenAsync(user).Result;
                        var resultPass = userManager.ResetPasswordAsync(user, resetToken, userViewModel.PasswordHash)
                            .Result;
                        if (!resultPass.Succeeded)
                        {
                            transaction.Rollback();
                            return -2; // không cập nhật được password
                        }
                    }

                    transaction.Commit();
                    return 1;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                return -1;
            }

            return -1;
        }

        public int UpdateProfileUser(ApplicationUser userProfileViewModel, int id)
        {
            try
            {
                ApplicationUser user = ApplicationDbContext.Users.FirstOrDefault(x => x.Id == id && x.Flag == 0);
                if (user != null)
                {
                    user.FullName = userProfileViewModel.FullName;
                    user.Image = userProfileViewModel.Image;
                    user.Sex = userProfileViewModel.Sex;
                    user.Email = userProfileViewModel.Email;
                    user.PhoneNumber = userProfileViewModel.PhoneNumber;
                    ApplicationDbContext.Users.Update(user);
                    ApplicationDbContext.SaveChanges();
                    return 1;
                }
            }
            catch (Exception)
            {
                return -1;
            }

            return -1;
        }


        public int InsertApplicationUser(ApplicationUser userViewModel, List<RoleInput> listRole)
        {
            using IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                var user = new ApplicationUser
                {
                    UserName = userViewModel.UserName,
                    FullName = userViewModel.FullName,
                    Email = userViewModel.Email,
                    BirthDay = userViewModel.BirthDay,
                    Address = userViewModel.Address,
                    Image = userViewModel.Image,
                    Sex = userViewModel.Sex,
                    IsActive = userViewModel.IsActive,
                    PhoneNumber = userViewModel.PhoneNumber,
                    Flag = 0,
                    Type = userViewModel.Type
                };
                var result = userManager.CreateAsync(user, userViewModel.PasswordHash).Result;
                if (result.Succeeded)
                {
                    if (listRole is { Count: > 0 })
                    {
                        List<ApplicationUserRole> insertUserRole = new List<ApplicationUserRole>();
                        foreach (var item in listRole)
                        {
                            if (item.IsSelected)
                            {
                                if (!ApplicationDbContext.UserRoles.Any(x =>
                                        x.UserId == user.Id && x.RoleId == item.Id))
                                {
                                    ApplicationUserRole insert = new ApplicationUserRole
                                    {
                                        RoleId = item.Id,
                                        UserId = user.Id
                                    };
                                    insertUserRole.Add(insert);
                                }
                            }
                        }

                        if (insertUserRole.Count > 0)
                        {
                            ApplicationDbContext.UserRoles.AddRange(insertUserRole);
                            ApplicationDbContext.SaveChanges();
                        }
                    }

                    transaction.Commit();
                    return user.Id;
                }
                else
                {
                    transaction.Rollback();
                    return -1;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                return -1;
            }
        }

        public bool DeleteApplicationUser(int id)
        {
            ApplicationUser user = ApplicationDbContext.Users.FirstOrDefault(x => x.Id == id && x.Flag == 0);
            if (user != null)
            {
                using IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction();
                try
                {
                    ApplicationDbContext.UserRoles.RemoveRange(
                        ApplicationDbContext.UserRoles.Where(x => x.UserId == user.Id));
                    ApplicationDbContext.SaveChanges();
                    ApplicationDbContext.UserClaims.RemoveRange(
                        ApplicationDbContext.UserClaims.Where(x => x.UserId == user.Id));
                    ApplicationDbContext.SaveChanges();
                    ApplicationDbContext.UserLogins.RemoveRange(
                        ApplicationDbContext.UserLogins.Where(x => x.UserId == user.Id));
                    ApplicationDbContext.SaveChanges();
                    ApplicationDbContext.UserTokens.RemoveRange(
                        ApplicationDbContext.UserTokens.Where(x => x.UserId == user.Id));
                    ApplicationDbContext.SaveChanges();
                    user.Flag = -1;
                    var guid = Guid.NewGuid();
                    user.NormalizedUserName = $"{user.NormalizedUserName}_{guid}";
                    ApplicationDbContext.Users.Update(user);
                    ApplicationDbContext.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            return false;
        }

        public override int DeleteAll(List<int> listId, bool isSoftDelete = true)
        {
            using IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                for (int i = 0; i < listId.Count; i++)
                {
                    ApplicationUser user =
                        ApplicationDbContext.Users.FirstOrDefault(x => x.Id == listId[i] && x.Flag == 0);
                    if (user != null)
                    {
                        ApplicationDbContext.UserRoles.RemoveRange(
                            ApplicationDbContext.UserRoles.Where(x => x.UserId == user.Id));
                        ApplicationDbContext.SaveChanges();
                        ApplicationDbContext.UserClaims.RemoveRange(
                            ApplicationDbContext.UserClaims.Where(x => x.UserId == user.Id));
                        ApplicationDbContext.SaveChanges();
                        ApplicationDbContext.UserLogins.RemoveRange(
                            ApplicationDbContext.UserLogins.Where(x => x.UserId == user.Id));
                        ApplicationDbContext.SaveChanges();
                        ApplicationDbContext.UserTokens.RemoveRange(
                            ApplicationDbContext.UserTokens.Where(x => x.UserId == user.Id));
                        ApplicationDbContext.SaveChanges();
                        user.Flag = -1;
                        var guid = Guid.NewGuid();
                        user.NormalizedUserName = $"{user.NormalizedUserName}_{guid}";
                        ApplicationDbContext.Users.Update(user);
                        ApplicationDbContext.SaveChanges();
                    }
                }

                transaction.Commit();
                return listId.Count;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return -1;
            }
        }

        public ApplicationUser FindByUsername(string userName)
        {
            return ApplicationDbContext.Users.FirstOrDefault(x => x.UserName == userName && x.Flag == 0);
        }

        public ApplicationUser FindByEmail(string email)
        {
            return ApplicationDbContext.Users.FirstOrDefault(x => x.Email == email && x.Flag == 0);
        }


        public void SaveUserToken(int userId, string token, string type)
        {
            var userToken = ApplicationDbContext.UserTokens.FirstOrDefault(x => x.UserId == userId && x.Name == type);
            if (userToken != null)
            {
                userToken.Value = token;
                ApplicationDbContext.UserTokens.Update(userToken);
                ApplicationDbContext.SaveChanges();
            }
            else
            {
                ApplicationUserToken rs = new ApplicationUserToken();
                rs.UserId = userId;
                rs.LoginProvider = type;
                rs.Name = type;
                rs.Value = token;
                ApplicationDbContext.UserTokens.Add(rs);
                ApplicationDbContext.SaveChanges();
            }
        }

        public void DeleteUserToken(int userId, string type)
        {
            var userToken = ApplicationDbContext.UserTokens.Where(x => x.UserId == userId && x.Name == type);
            if (userToken.Any())
            {
                ApplicationDbContext.UserTokens.RemoveRange(userToken);
                ApplicationDbContext.SaveChanges();
            }
        }

        public override ApplicationUser FindById(int id)
        {
            return ApplicationDbContext.Users.FirstOrDefault(x => x.Id == id && x.Flag == 0);
        }

        public override bool IsCheckById(int id)
        {
            return ApplicationDbContext.Users.Any(x => x.Id == id && x.Flag == 0);
        }
    }
}