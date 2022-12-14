using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Castle.Core.Internal;
using CMS.Areas.Admin.Const;
using CMS.Areas.Admin.ViewModels.Home;
using CMS.Areas.Admin.ViewModels.Home.OrderDetail;
using CMS.Areas.Admin.ViewModels.Home.ToProduct;
using CMS.Areas.Customer.Const;
using CMS_Access.Repositories.Orders;
using CMS_Access.Repositories.Products;
using CMS_Lib.DI;
using CMS.Areas.Orders.Const;
using CMS.DataTypes;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MoreLinq;

namespace CMS.Areas.Admin.Services.Home
{
    public interface IDashBoardService : IScoped
    {
        CharDataModel GetDataSalesDay(DateTime dateStart, DateTime dateEnd);
        CharDataModel GetDataSalesMonth(DateTime dateStart, DateTime dateEnd);
        CharDataToProductModel GetDataToProduct(DateTime dateStart, DateTime dateEnd, int typeStatus,int page);
        CharDataModel GetDataSaleGroup(DateTime dateStart, DateTime dateEnd);
        // List<SeriesChart> GetDataAreaPrice(DateTime dateStart, DateTime dateEnd);
        // List<SeriesChart> GetDataAreaQuantity(DateTime dateStart, DateTime dateEnd);

        List<SeriesCharArea> GetDataArea(DateTime dateStart, DateTime dateEnd);
        
        List<CMS_EF.Models.Products.Products> GetDataToRating(DateTime dateStart, DateTime dateEnd);
        List<OrderDetailViewModel> GetDataOrderDetail();
    }

    public class DashBoardService : IDashBoardService
    {
        private readonly IOrdersRepository _iOrdersRepository;
        private readonly IProductRepository _iProductRepository;
        private readonly IOrderProductRepository _iOrderProductRepository;
        public DashBoardService(IOrdersRepository iOrdersRepository, IProductRepository iProductRepository, IOrderProductRepository iOrderProductRepository)
        {
            _iOrdersRepository = iOrdersRepository;
            _iProductRepository = iProductRepository;
            _iOrderProductRepository = iOrderProductRepository;
        }

        public CharDataModel GetDataSalesDay(DateTime dateStart, DateTime dateEnd)
        {
            
            var data = _iOrdersRepository.FindAll().Where(x => x.OrderAt.HasValue &&
                                                              x.OrderAt.Value >= dateStart && x.OrderAt <= dateEnd && x.Status != OrderStatusConst.StatusOrderCancel)
                .GroupBy(x => new {
                    Month = x.OrderAt.Value.Month, Day = x.OrderAt.Value.Day,
                    Year = x.OrderAt.Value.Year})
                .Select(x => new
                {
                    Month = x.Key.Month,
                    Day = x.Key.Day,
                    Year = x.Key.Year,
                    Date =  new DateTime(x.Key.Year, x.Key.Month, x.Key.Day, 0, 0, 0).ToString("dd/MM/yyyy"),
                    Prices = x.Sum(x => x.Total ?? 0),
                    Count = x.Count()
                }).OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day).ToList();
            var chartDataModel = new CharDataModel()
            {
                Categories = data.Select(x => x.Date).ToList(),
                Prices = data.Select(x => x.Prices).ToList(),
                LineData = data.Select(x => x.Count).ToList()
            };
            return chartDataModel;
        }

       
        public CharDataModel GetDataSalesMonth(DateTime dateStart, DateTime dateEnd)
        {
           var data = _iOrdersRepository.FindAll().Where(x => x.OrderAt.HasValue &&
                                                               x.OrderAt.Value >= dateStart && x.OrderAt <= dateEnd && x.Status != OrderStatusConst.StatusOrderCancel)
                .GroupBy(x => new {
                    Month = x.OrderAt.Value.Month, 
                    Year = x.OrderAt.Value.Year})
                .Select(x => new
                {
                    Month = x.Key.Month,
                    Year = x.Key.Year,
                    Date =  new DateTime(x.Key.Year, x.Key.Month,1, 0, 0, 0).ToString("MM/yyyy"),
                    Prices = x.Sum(x => x.Total ?? 0),
                    Count = x.Count()
                }).OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();
            var chartDataModel = new CharDataModel()
            {
                Categories = data.Select(x => x.Date).ToList(),
                Prices = data.Select(x => x.Prices).ToList(),
                LineData = data.Select(x => x.Count).ToList()

            };
            return chartDataModel;
        }
        public CharDataModel GetDataSaleGroup(DateTime dateStart, DateTime dateEnd)
        {

 
           var rs = _iOrdersRepository.FindAll().Where(x => x.OrderAt.HasValue &&
                                                            x.OrderAt.Value >= dateStart && x.OrderAt <= dateEnd &&
                                                            x.Status != OrderStatusConst.StatusOrderCancel)
               
               .Include(x => x.Customer)
               .Select(x => new
               {
                   Name = GetNameTypeGroup(x.Customer),
                   x.Total
               }).ToList().Where(x => !string.IsNullOrEmpty(x.Name) && x.Total > 0)
                .GroupBy(x => x.Name)
                .Select(x => new
                {
                    TypeGroup = x.Key,
                    Prices = x.Sum(x => x.Total ?? 0)
                }).OrderByDescending(x => x.Prices);
           var data = rs.ToList();
            var chartDataModel = new CharDataModel()
            {
                Categories = data.Select(x => x.TypeGroup).ToList(),
                Prices = data.Select(x => x.Prices).ToList()
            };
            return chartDataModel;
        }

        public static string GetNameTypeGroup(CMS_EF.Models.Customers.Customer rs)
        {
            if (rs == null) return string.Empty;
            var stg = CustomerTypeGroupConst.GetCustomerTypeGroup(rs.TypeGroup ?? 0);
            return stg;
        }
        
        public List<SeriesCharArea> GetDataArea(DateTime dateStart, DateTime dateEnd)
        {
            var data = _iOrdersRepository.FindAll().Where(x => x.OrderAt.HasValue &&
                                                               x.OrderAt.Value >= dateStart &&
                                                               x.OrderAt <= dateEnd &&
                                                               x.Status != OrderStatusConst.StatusOrderCancel)
                .Include(x => x.OrderAddress)
                .ThenInclude(x => x.Province)
                .Include(x => x.OrderProduct.Where(x => x.Flag == 0))
                .Select(x => new 
                {
                    AreaId = x.ShipPartner == ShipConst.WareHouse ? AreaCost.WareHouse : x.OrderAddress.Province.Area,
                    Total =x.Total,
                    Quantity =  x.OrderProduct.Sum(x => x.Quantity)
                }).ToList().GroupBy(x => x.AreaId)
                .Select(x => new SeriesCharArea
                {
                    AreaId = x.Key,
                    Name = AreaCost.GetArea(x.Key ?? 0),
                    Price = x.Sum(z => z.Total ?? 0),
                    Quantity = x.Sum(z => z.Quantity ?? 0),
                }).OrderBy(x => x.Name);
            return data.ToList();
        }
        public CharDataToProductModel GetDataToProduct(DateTime dateStart, DateTime dateEnd, int typeStatus , int page)
        {
            CharDataToProductModel newData = new CharDataToProductModel();
            var query = _iOrderProductRepository.FindAll()
                .Include(x => x.Order)
                .Where(x => x.Flag == 0 && x.Order.Flag == 0 &&
                            x.Order.OrderAt.HasValue
                            && x.Order.OrderAt >= dateStart && x.Order.OrderAt <= dateEnd && x.Order.Status != OrderStatusConst.StatusOrderCancel)
                .GroupBy(x => new
                {
                    ProductName = x.ProductName,
                });
            if (typeStatus == FilterToProductConst.StatusQuantity)
            {
                var data = query
                    .Select(x => new
                    {
                        ProductName = x.Key.ProductName,
                        Quantity = x.Sum(x => x.Quantity ?? 0)
                    }).OrderByDescending(x => x.Quantity).Take(page).ToList();
                newData = new CharDataToProductModel()
                {
                    Categories = data.Select(x => x.ProductName).ToList(),
                    Prices = data.Select(x => (double) x.Quantity).ToList(),
                    FilterStatus = FilterToProductConst.GetValue(FilterToProductConst.StatusQuantity),
                    ValueSuffix = " pcs"

                };
                return newData;
            }
            else
            {
                var data = query
                    .Select(x => new
                    {
                        ProductName = x.Key.ProductName,
                        Prices = x.Sum(x => (x.Price ?? 0) * (x.Quantity ?? 0))
                    }).OrderByDescending(x => x.Prices).Take(page).ToList();
                newData = new CharDataToProductModel()
                {
                    Categories = data.Select(x => x.ProductName).ToList(),
                    Prices = data.Select(x => (double) x.Prices).ToList(),
                    FilterStatus =FilterToProductConst.GetValue(FilterToProductConst.StatusPrice),
                    ValueSuffix = " VND"
                };
                return newData;
            }
        }

        public List<CMS_EF.Models.Products.Products> GetDataToRating(DateTime dateStart, DateTime dateEnd)
        {
            var data = _iProductRepository.FindAll().Where(x => x.RateCount > 0).OrderByDescending(x =>  (x.RateCount > 0 ? (x.Rate / x.RateCount) : 0)).Take(10).ToList();
            return data;
        }

        public List<OrderDetailViewModel> GetDataOrderDetail()
        {
            var rs = new List<OrderDetailViewModel>();
            DateTime t = DateTime.Now;
            DateTime start = new DateTime(t.Year, t.Month, t.Day, 0, 0, 0);
            DateTime  end = new DateTime(t.Year, t.Month, t.Day, 23, 59, 59);
            var query = _iOrdersRepository.FindAll().Where(x => x.OrderAt >= start && x.OrderAt <= end);
            List<OrderDetailViewModel> data = OrderDetailCost.ListOrderDetail;
            foreach (OrderDetailViewModel item in data)
            {
                OrderDetailViewModel i = item;
                var p = query.Where(x => i.ListStatus.Contains(x.Status.Value));
                i.CountOrder = p.Count();
                i.PriceOrder = p.Sum(x => x.Total.Value);
                i.Date = start.ToString("dd/MM/yyyy");
                rs.Add(i);
            }

            return rs;
        }
    }
}