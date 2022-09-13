using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Core.Internal;
using ClosedXML.Excel;
using CMS.Areas.Coupons.Models;
using CMS.Areas.Customer.Services;
using CMS.DataTypes;
using CMS.Models;
using CMS_Access.Repositories.Customers;
using CMS_EF.DbContext;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using CMS_Lib.Helpers;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Coupons.Services;
public interface ICouponService : IScoped
{
    void SaveDataCoupon(IFormFile file,string pathFile, int userId);
    bool CheckDataCoupon(IFormFile file);
    bool CheckSameCoupon(IFormFile file);
    bool DeleteCouponByIdFile(HistoryFileCoupon historyFileCoupon);
    bool DeleteAllCouponByIdFile(List<int> ids);
    IQueryable<CustomerCoupon> GetListCustomerCoupons(int idFile);
    void SendNotificationHistoryFileCoupon(int id);
}

public class CouponService : ICouponService
{
    private readonly ILogger<CouponService> _iLogger;
    private readonly IHistoryFileCouponRepository _iHistoryFileCouponRepository;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ICustomerRepository _iCustomerRepository;
    private readonly ICustomerCouponRepository _iCustomerCouponRepository;
    private readonly ICustomerNotificationService _iCustomerNotificationService;

    public CouponService( ILogger<CouponService>  iLogger, IHistoryFileCouponRepository iHistoryFileCouponRepository, ApplicationDbContext applicationDbContext,
        ICustomerRepository iCustomerRepository, ICustomerCouponRepository iCustomerCouponRepository, ICustomerNotificationService iCustomerNotificationService)
    {
        _iLogger = iLogger;
        _iHistoryFileCouponRepository = iHistoryFileCouponRepository;
        _applicationDbContext = applicationDbContext;
        _iCustomerRepository = iCustomerRepository;
        _iCustomerCouponRepository = iCustomerCouponRepository;
        _iCustomerNotificationService = iCustomerNotificationService;
    }

    public void SaveDataCoupon(IFormFile file,string pathFile, int userId)
    {
        using var transaction = _applicationDbContext.Database.BeginTransaction();
        try
        {
            var workbook = new XLWorkbook(file.OpenReadStream());
            IXLWorksheet ws = workbook.Worksheet(1);
            IXLRange range = ws.RangeUsed();
            string codeFile = range.Cell(2, 3).GetString();
            string orgName = range.Cell(3, 3).GetString();
            var historyCouponFile = new HistoryFileCoupon()
            {
                LinkFile = pathFile,
                Code = codeFile,
                CreatedBy = userId,
                CreatedAt = DateTime.Now,
                FileName = Path.GetFileName(file.FileName),
                OrgName = orgName
            };
            _iHistoryFileCouponRepository.Create(historyCouponFile);
            List<CustomerCoupon> listCoupon = new List<CustomerCoupon>();
            int rowCount = range.RowCount();
            for (int i = 6; i <= rowCount; i++)
            {
                string userName = range.Cell(i, 2).GetString()!.Trim();
                int price = CmsFunction.ConvertToInt(range.Cell(i, 3).GetString()) ?? 0;
                DateTime? dateStart =  null;
                DateTime? dateEnd = null;
                var startCell = range.Cell(i, 4);
                var endCell = range.Cell(i, 5);
                if (!startCell.GetString().IsNullOrEmpty() && startCell.DataType == XLDataType.DateTime)
                {
                    dateStart = (DateTime) startCell.Value;
                } 
                if (!endCell.GetString().IsNullOrEmpty() && endCell.DataType == XLDataType.DateTime)
                {
                    dateEnd = (DateTime) endCell.Value;
                }
                var time = new TimeRange(dateStart, dateEnd);
                if (time.Start > time.End)
                {
                    throw new NullReferenceException($"Row {i} ngày kết thúc không được nhỏ hơn ngày bắt đầu");
                }
                int customerId = 0;
                if (!userName.IsNullOrEmpty())
                {
                    var customer = _iCustomerRepository.FindByUserName(userName);
                    customerId = customer == null ? 0 : customer.Id;
                }

                if (customerId != 0 && price > 0 && dateStart != null && dateEnd != null && dateStart <= dateEnd)
                {
                    listCoupon.Add(new CustomerCoupon()
                    {
                        CustomerId = customerId,
                        StartTimeUse = time.Start,
                        EndTimeUse = time.End,
                        Status = 0,
                        LastModifiedAt = DateTime.Now,
                        ReducedPrice = price,
                        HistoryFileCoupon = historyCouponFile.Id
                    });
                }
            }

            if (listCoupon.Count > 0)
            {
                var listSave =  _iCustomerCouponRepository.CreateAll(listCoupon);
                foreach (var item in listSave)
                {
                    // this._iCustomerNotificationService.SendCustomerNotification(item.CustomerId.Value, new CustomerNotificationObject()
                    // {
                    //     Title = "Quý Khách hàng được tặng 1 mã COUPON",
                    //     Detail = $"Đặt hàng ngay để sử dụng COUPON (thời gian áp dụng: từ {item.StartTimeUse.Value.ToString("dd/MM")} đến {item.StartTimeUse.Value.ToString("dd/MM/yyyy")})",
                    //     Link = null
                    // });
                    item.Code = HashHelper.HashCharacter(item.Id.ToString());
                    _iCustomerCouponRepository.Update(item);
                }
            }
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            this._iLogger.LogError(e, $"SaveDataCoupon:  - {userId}");
            throw;
        }
    }

    public bool CheckDataCoupon(IFormFile file)
    {
        var workbook = new XLWorkbook(file.OpenReadStream());
        IXLWorksheet ws = workbook.Worksheet(1);
        IXLRange range = ws.RangeUsed();
        var codeFile =  range.Cell(2, 3).GetString();
        if (codeFile!.Trim().IsNullOrEmpty())
        {
            return false;
        }
        return true;
    }   
    public bool CheckSameCoupon(IFormFile file)
    {
        var workbook = new XLWorkbook(file.OpenReadStream());
        IXLWorksheet ws = workbook.Worksheet(1);
        IXLRange range = ws.RangeUsed();
        var codeFile =  range.Cell(2, 3).GetString();
        var listFileCoupon = _iHistoryFileCouponRepository.GetListByCode(codeFile!.Trim());
        if (!listFileCoupon.IsNullOrEmpty() && listFileCoupon.Count > 0)
        {
            return false;
        }
        return true;
    }

    public bool DeleteCouponByIdFile(HistoryFileCoupon historyFileCoupon)
    {
        using var transaction = _applicationDbContext.Database.BeginTransaction();
        try
        {
            _iHistoryFileCouponRepository.Delete(historyFileCoupon);
            var listCoupon = _iCustomerCouponRepository.FindAll()
                .Where(x => x.HistoryFileCoupon == historyFileCoupon.Id).ToList();
            if (listCoupon.Count > 0)
            {
                _iCustomerCouponRepository.DeleteAll(listCoupon);
            }
            transaction.Commit();
            return true;
        }
        catch (Exception e)
        {
            this._iLogger.LogError(e, $"Xóa file coupon bị lỗi:  ");
            transaction.Rollback();
            return false;
        }
    }

    public bool DeleteAllCouponByIdFile(List<int> ids)
    {
        using var transaction = _applicationDbContext.Database.BeginTransaction();
        try
        {
            
          _iHistoryFileCouponRepository.DeleteAll(ids);
            var listCoupon = _iCustomerCouponRepository.FindAll()
                .Where(x => ids.Contains(x.HistoryFileCoupon.Value)).ToList();
            if (listCoupon.Count > 0)
            {
                _iCustomerCouponRepository.DeleteAll(listCoupon);
            }
            transaction.Commit();
            return true;
        }
        catch (Exception e)
        {
            this._iLogger.LogError(e, $"Xóa file coupon bị lỗi:  ");
            transaction.Rollback();
            return false;
        }
    }

   public  IQueryable<CustomerCoupon> GetListCustomerCoupons(int idFile)
   {
       return _iCustomerCouponRepository.FindAll().Where(x => x.HistoryFileCoupon == idFile)
           .Include(x => x.Customer);
   }

   public void SendNotificationHistoryFileCoupon(int id)
   {
       var customerCoupons = _iCustomerCouponRepository.FindAll().Where(x => x.HistoryFileCoupon == id).ToList();
       foreach (var item in customerCoupons)
       {
           this._iCustomerNotificationService.SendCustomerNotification(item.CustomerId.Value, new CustomerNotificationObject()
           {
               Title = "Quý Khách hàng được tặng 1 mã COUPON",
               Detail = $"Đặt hàng ngay để sử dụng COUPON : { item.Code} (thời gian áp dụng: từ {item.StartTimeUse.Value.ToString("dd/MM/yyyy")} đến {item.EndTimeUse.Value.ToString("dd/MM/yyyy")})",
               Link = "/danh-sach-san-pham"
           });
       }

       var historyFile = _iHistoryFileCouponRepository.FindById(id);
       historyFile.IsSentNotification = true;
       _iHistoryFileCouponRepository.Update(historyFile);
   }
}