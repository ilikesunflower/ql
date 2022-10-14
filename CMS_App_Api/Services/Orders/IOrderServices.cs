using System;
using System.Collections.Generic;
using System.Linq;
using CMS_Access.Repositories;
using CMS_Access.Repositories.Orders;
using CMS_App_Api.Areas.Orders.Const;
using CMS_App_Api.Areas.Orders.Models;
using CMS_App_Api.Services.Customers;
using CMS_App_Api.Services.Products;
using CMS_App_Api.Services.WareHouses;
using CMS_EF.Models.Customers;
using CMS_EF.Models.Orders;
using CMS_EF.Models.Products;
using CMS_Lib.DI;
using CMS_Lib.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace CMS_App_Api.Services.Orders
{
    public interface IOrderServices : IScoped
    {
        public CMS_EF.Models.Orders.Orders Create(CMS_EF.Models.Orders.Orders orders);
        public CMS_EF.Models.Orders.Orders Edit(OrderEditApiModel orders);
        public void Cancel(string code,string message);
    }
    public class OrderServices : IOrderServices
    {
        private readonly ILogger<OrderServices> _iLogger;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IProductServices _productServices;
        private readonly IOrderProductRepository _iOrderProductRepository;
        private readonly ICustomerPointServices _customerPointServices;
        private readonly ICustomerCouponServices _customerCouponServices;
        private readonly IOrdersAddressRepository _iOrdersAddressRepository;
        private readonly ICustomerCardServices _customerCardServices;
        private readonly IWareHouseService _iWareHouseService;
        private readonly IOrderPartnerShipLogRepository _orderPartnerShipLogRepository;


        public OrderServices(ILogger<OrderServices> iLogger,IProductServices productServices, ICustomerPointServices customerPointServices, ICustomerCouponServices customerCouponServices, IOrdersRepository ordersRepository, IDatabaseTransaction databaseTransaction, 
            ICustomerCardServices customerCardServices, IWareHouseService iWareHouseService, IOrderProductRepository iOrderProductRepository,
            IOrdersAddressRepository iOrdersAddressRepository,IOrderPartnerShipLogRepository orderPartnerShipLogRepository )
        {
            _iLogger = iLogger;
            _productServices = productServices;
            _customerPointServices = customerPointServices;
            _customerCouponServices = customerCouponServices;
            _ordersRepository = ordersRepository;
            _databaseTransaction = databaseTransaction;
            _customerCardServices = customerCardServices;
            _iWareHouseService = iWareHouseService;
            _iOrderProductRepository = iOrderProductRepository;
            _iOrdersAddressRepository = iOrdersAddressRepository;
            _orderPartnerShipLogRepository = orderPartnerShipLogRepository;
        }

        public CMS_EF.Models.Orders.Orders Create(CMS_EF.Models.Orders.Orders orders)
        {
            //check product
            List<int?> productSimilarIds = orders.OrderProduct.Select(x => x.ProductSimilarId ).ToList();
            List<ProductSimilar> productSimilars = _productServices.GetAllProductSimilarByIds(productSimilarIds);
            if (!orders.OrderProduct.All( orderProduct => productSimilars.Any( similar => similar.Id == orderProduct.ProductSimilarId && similar.QuantityWh >= orderProduct.Quantity ) ))
            {
                this._iLogger.LogError($"Đơn hàng {orders.Code} - {orders.CustomerId} Số lượng sản phẩm trong kho không đủ! Vui lòng thử lại");
                throw new Exception("Số lượng sản phẩm trong kho không đủ! Vui lòng thử lại");
            }
            
            //check point 
            List<CustomerPoint> customerPoint = new List<CustomerPoint>();
            if (orders.Point > 0)
            {
                customerPoint = _customerPointServices.FindByCustomerId(orders.CustomerId ?? 0);
                var allPoint = customerPoint.Sum(x => x.Point) ?? 0;
                if ( allPoint < orders.Point)
                {
                    this._iLogger.LogError($"Đơn hàng {orders.Code} - {orders.CustomerId} Số lượng điểm của bạn không đủ! Vui lòng thử lại");
                    throw new Exception("Số lượng điểm của bạn không đủ! Vui lòng thử lại");
                }
            }
            
            //check discount 
            CustomerCoupon customerCoupon = null;
            if (!string.IsNullOrEmpty(orders.CouponCode))
            {
                customerCoupon = _customerCouponServices.FindCouponActiveByCustomerId(orders.CustomerId ?? 0,orders.CouponCode);
                if (customerCoupon == null)
                {
                    this._iLogger.LogError($"Đơn hàng {orders.Code} - {orders.CustomerId} Không tìm thấy coupon hoặc đã được sử dụng! Vui lòng thử lại");
                    throw new Exception("Không tìm thấy coupon hoặc đã được sử dụng! Vui lòng thử lại");
                }
            }

            IDbContextTransaction transaction = _databaseTransaction.BeginTransaction();

            try
            {
                //minis product quantity
                List<ProductSimilar> productSimilarsChange = new List<ProductSimilar>();
                foreach (var orderProduct in orders.OrderProduct)
                {
                    var productSimilar = productSimilars.FirstOrDefault(x => x.Id ==  orderProduct.ProductSimilarId );
                    if (productSimilar == null) continue;
                    productSimilar.QuantityWh = Math.Max(0, (productSimilar.QuantityWh ?? 0) - (orderProduct.Quantity ?? 0));
                    productSimilar.QuantityUse = Math.Max(orderProduct.Quantity ?? 0, (productSimilar!.QuantityUse ?? 0) + (orderProduct.Quantity ?? 0));
                    productSimilarsChange.Add(productSimilar);
                }

                if (productSimilarsChange.Count > 0)
                {
                    _productServices.ChangeRange(productSimilarsChange);
                }
            
                //Create order
                var orderCreated = _ordersRepository.Create(orders);

                orderCreated.Code = DateTime.Now.ToString("yyMMdd")+HashHelper.HashCharacter(orderCreated.Id.ToString());
                _ordersRepository.Update(orderCreated);
                
                //create log
                _orderPartnerShipLogRepository.Create(new OrderPartnerShipLog()
                {
                    OrderCode = orderCreated.Code,
                    PartnerShipCreatedAt = DateTime.Now,
                    PartnerShipDetails = "Đặt hàng thành công",
                });
                
                //remove product in cart
                _customerCardServices.Paid(productSimilarIds,orders.CustomerId);


                // minis point
                if (orders.Point > 0 && customerPoint.Count > 0)
                {
                    _customerPointServices.Use(customerPoint,orderCreated);
                }
            
                // minis discount
                if (customerCoupon != null)
                {
                    _customerCouponServices.Use(customerCoupon, orderCreated);
                }
                
                _databaseTransaction.Commit(transaction);
                
                // tạo order sang kiot việt
                _iWareHouseService.CreateOrder(orders);
                
                //return order
                return orderCreated;
            }
            catch
            {
                _databaseTransaction.Rollback(transaction);
                throw;
            }
        }
        
        public void Cancel(string code,string message)
        {
            //find order
            var orders =  _ordersRepository.FindByCodeWithProductAndPoint(code);
            if (orders == null)
            {
                throw new Exception("Không tìm thấy đơn hàng");
            }
            
            var transaction = _databaseTransaction.BeginTransaction();
            try
            {
                //find point and revert point
                if (orders.Point != null )
                {
                    _customerPointServices.Revert(orders);
                }
            
                //find discount and revert discount
                if (orders.Point != null)
                {
                    _customerCouponServices.Revert(orders);
                }

                //revert product quantity
                List<int?> productSimilarIds = orders.OrderProduct.Select(x => x.ProductSimilarId ).ToList();
                if (productSimilarIds.Count > 0 )
                {
                    List<ProductSimilar> productSimilars = _productServices.GetAllProductSimilarByIds(productSimilarIds);
                    List<ProductSimilar> productSimilarsChange = new List<ProductSimilar>();
            
                    foreach (OrderProduct orderProduct in orders.OrderProduct)
                    {
                        var productSimilar = productSimilars.FirstOrDefault(x => x.Id ==  orderProduct.ProductSimilarId );
                        if (productSimilar != null)
                        {
                            productSimilar!.QuantityWh +=  orderProduct.Quantity;
                            productSimilar!.QuantityUse = Math.Max(0, (productSimilar!.QuantityUse ?? 0) - (orderProduct.Quantity ?? 0));
                            productSimilarsChange.Add(productSimilar);   
                        }
                    }
                    _productServices.ChangeRange(productSimilarsChange);
                }
                
                //change status
                orders.Note = message;
                orders.ReasonNote = message;
                orders.Status = OrderStatusConst.Failed.Status;
                orders.LastModifiedAt = DateTime.Now;
                orders.OrderStatusCancelAt = DateTime.Now;
                _ordersRepository.Update(orders);
              
                _databaseTransaction.Commit(transaction);
                _iWareHouseService.DeleteOrder(orders);
                
                // hủy order trong kiot việt
                if (orders.OrderIdWh.HasValue)
                {
                    _iWareHouseService.DeleteOrder(orders);
                }
                
            }
            catch
            {
                _databaseTransaction.Rollback(transaction);
                throw;
            }
           
        }
        
        public CMS_EF.Models.Orders.Orders Edit(OrderEditApiModel data)
        {
       
            CMS_EF.Models.Orders.Orders order = data.Order;
            //check point 
            //lay gia tri order cu - lay ra poi cu
            var orderOld = _ordersRepository.FindAll().Where( x => x.Id == order.Id).Include(x => x.OrderPoint.Where(x => x.Flag == 0)).FirstOrDefault();
        
                // List<CustomerPoint> customerPoint = new List<CustomerPoint>();
                // if (data.IsChangePoi)
                // {
                //     //check số lượng
                //     customerPoint = _customerPointServices.FindByCustomerId(order.CustomerId.Value);
                //     var pointNew = customerPoint.Sum(x => x.Point) ?? 0;
                //     var listOrderPoi = _customerPointServices.FindByIdAndOrderId(order.Id);
                //     var pointOld = listOrderPoi.Sum(x => x.Point) ?? 0;
                //     var sumPoint = pointNew + pointOld;
                //     if (data.Order.Point > sumPoint)
                //     {
                //         throw new Exception("Số lượng điểm của bạn không đủ! Vui lòng thử lại");
                //     }
                // }
           
           //update CustomerPoi vaf tao log


            //check discount 
            CustomerCoupon customerCoupon = null;
            //CustomerCoupon mơi
            // kiem tra xem co ma do ton tai ko.
            if (!string.IsNullOrEmpty(order.CouponCode))
            {
                customerCoupon = _customerCouponServices.FindCouponActiveEdit(order.CustomerId.Value, order.CouponCode, orderOld.CouponCode);
                //customer mới
                if (customerCoupon == null)
                {
                    throw new Exception("Không tìm thấy coupon hoặc đã được sử dụng! Vui lòng thử lại");
                }
            }
       

            IDbContextTransaction transaction = _databaseTransaction.BeginTransaction();

            try
            {
                if(orderOld.CouponCode != order.CouponCode)
                {
                    //rever customerCoupon cu
                    _customerCouponServices.Revert(orderOld);
                    //xu dung coupon moi
                    if (customerCoupon != null)
                    {
                        _customerCouponServices.Use(customerCoupon, order);
                    }
                }
                // if (data.IsChangePoi)
                // {
                //   _customerPointServices.RevertEdit(orderOld);
                //   if (order.Point > 0 && customerPoint.Count > 0)
                //   {
                //       _customerPointServices.Use(customerPoint, order );
                //   }
                // }
                //update address
                var newAddress = data.OrderAddress;

                var address = _iOrdersAddressRepository.FindAll().Where(x => x.OrderId == order.Id).FirstOrDefault();
                if(address == null)
                {
                    newAddress.OrderId = order.Id;
                    _iOrdersAddressRepository.Create(newAddress);
                }
                else
                {
                    address.Name = newAddress.Name;
                    address.Address = newAddress.Address;
                    address.Phone = newAddress.Phone;
                    address.Email = newAddress.Email;
                    address.ProvinceCode = newAddress.ProvinceCode;
                    address.DistrictCode = newAddress.DistrictCode;
                    address.CommuneCode = newAddress.CommuneCode;
                    address.Note = newAddress.Note;
                    _iOrdersAddressRepository.Update(address);

                }

                //minis product quantity

                //quantity similar delete
                List<ProductSimilar> listSimilarDeleleChange = new List<ProductSimilar>();
                if(data.ListOrderProductDelete.Count != 0)
                {
                    List<int?> deleteSimilar = data.ListOrderProductDelete.Select(x => x.ProductSimilarId).ToList();
                    List<ProductSimilar> productSimilarDelete = _productServices.GetAllProductSimilarByIds(deleteSimilar);
                    foreach (var orderProduct in data.ListOrderProductDelete)
                    {
                        var productSimilar = productSimilarDelete.FirstOrDefault(x => x.Id == orderProduct.ProductSimilarId);
                        productSimilar.QuantityWh = Math.Max(0, (productSimilar.QuantityWh + orderProduct.Quantity).Value);
                        listSimilarDeleleChange.Add(productSimilar);
                    }
                }
                List<ProductSimilar> listSimilarCreateChange = new List<ProductSimilar>();

                if (data.ListOrderProductCreate.Count != 0)
                {
                    List<int?> createSimilar = data.ListOrderProductCreate.Select(x => x.ProductSimilarId).ToList();
                    List<ProductSimilar> productSimilarCreate = _productServices.GetAllProductSimilarByIds(createSimilar);
                    foreach(var orderProduct in data.ListOrderProductCreate)
                    {
                        var productSimilar = productSimilarCreate.FirstOrDefault(x => x.Id == orderProduct.ProductSimilarId);
                        productSimilar.QuantityWh = Math.Max(0, (productSimilar.QuantityWh - orderProduct.Quantity).Value);
                        listSimilarCreateChange.Add(productSimilar);
                    }
                }

                List<ProductSimilar> listSimilarUpdateChange = new List<ProductSimilar>();
                if(data.ListOrderProductUpdate.Count != 0)
                {
                    List<int?> updateSimilar = data.ListOrderProductUpdate.Select(x => x.ProductSimilarId).ToList();
                    List<OrderProduct> listProductOrder = _iOrderProductRepository.FindAll().Where(x => x.OrderId == data.Order.Id && updateSimilar.Contains(x.ProductSimilarId)).ToList();
                    List<ProductSimilar> productSimilarUpdate = _productServices.GetAllProductSimilarByIds(updateSimilar);
                   foreach(var orderProduct in data.ListOrderProductUpdate)
                    {
                        var productSimilar = productSimilarUpdate.FirstOrDefault(x => x.Id == orderProduct.ProductSimilarId);
                        var orderProductChange = listProductOrder.FirstOrDefault(x => x.ProductSimilarId == orderProduct.ProductSimilarId);
                        productSimilar.QuantityWh = Math.Max(0, (productSimilar.QuantityWh + orderProductChange.Quantity - orderProduct.Quantity).Value);
                        listSimilarUpdateChange.Add(productSimilar);
                    }
                }
                _productServices.ChangeRange(listSimilarUpdateChange);
                _productServices.ChangeRange(listSimilarCreateChange);
                _productServices.ChangeRange(listSimilarDeleleChange);

                if(data.ListOrderProductDelete.Count > 0)
                {
                    _iOrderProductRepository.DeleteAll(data.ListOrderProductDelete);
                }
                if(data.ListOrderProductUpdate.Count > 0)
                {
                    _iOrderProductRepository.BulkUpdate(data.ListOrderProductUpdate);

                }
                if(data.ListOrderProductCreate.Count > 0)
                {
                    _iOrderProductRepository.CreateAll(data.ListOrderProductCreate);
                }
               
                //Create order
                _ordersRepository.Update(order);
          
         
                _databaseTransaction.Commit(transaction);
                if (data.IsChangeProduct)
                {
                    // cập nhật wh
                    this._iWareHouseService.EditOrder(order);
                }
                //return order
                return order;
            }
            catch
            {
                _databaseTransaction.Rollback(transaction);
                throw;
            }
        }

       
    }
}
