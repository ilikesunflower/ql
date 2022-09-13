using System;
using System.Collections.Generic;
using System.Linq;
using CMS_Access.Repositories.Customers;
using CMS_Access.Repositories.Orders;
using CMS_App_Api.Areas.Orders.Const;
using CMS_EF.Models.Customers;
using CMS_EF.Models.Orders;
using CMS_Lib.DI;

namespace CMS_App_Api.Services.Customers;

public interface ICustomerPointServices:IScoped
{
    List<CustomerPoint> FindByCustomerId(int customerId);
    CustomerPointLog FindByCustomerPointLogId(int orderId);
    void ChangePoi(CustomerPoint point, CustomerPointLog pointLog);
    void Use(List<CustomerPoint> customerPoint,CMS_EF.Models.Orders.Orders orders);
    void Revert(CMS_EF.Models.Orders.Orders orders);
    void RevertEdit(CMS_EF.Models.Orders.Orders orders);
    List<OrderPoint> FindByIdAndOrderId(int orderId);
}
public class CustomerPointServices:ICustomerPointServices
{
    private readonly ICustomerPointRepository _customerPointRepository;
    private readonly ICustomerPointLogRepository _customerPointLogRepository;
    private readonly IOrderPointRepository _orderPointRepository;

    public CustomerPointServices(ICustomerPointRepository customerPointRepository, ICustomerPointLogRepository customerPointLogRepository, IOrderPointRepository orderPointRepository)
    {
        _customerPointRepository = customerPointRepository;
        _customerPointLogRepository = customerPointLogRepository;
        _orderPointRepository = orderPointRepository;
    }

    public List<CustomerPoint> FindByCustomerId(int customerId)
    {
        return _customerPointRepository.FindByCustomerId(customerId);
    }

    public void Use(List<CustomerPoint> customerPoint,CMS_EF.Models.Orders.Orders orders)
    {
        double haveToMinis = orders.Point ?? 0;
        List<CustomerPoint> needToUpdate = new List<CustomerPoint>();
        List<CustomerPointLog> customerPointLogs = new List<CustomerPointLog>();
        List<OrderPoint> orderPoints = new List<OrderPoint>();

        List<CustomerPoint> points = customerPoint.Where(x => x.Point > 0).ToList();
        
        foreach (var point in points)
        {
            if (!(haveToMinis > 0)) continue;
            var pointAfterMinis = (point.Point ?? 0) - haveToMinis;
            
            var orderPoint = new OrderPoint
            {
                Flag = 0,
                OrderId = orders.Id,
                CreatedAt = DateTime.Now,
                CustomerPointId = point.Id,
                EndTime = point.EndTime,
                StartTime = point.StartTime
            };
            
            if (pointAfterMinis > 0)
            {
                orderPoint.Point = haveToMinis;
                haveToMinis = 0;
                point.Point = pointAfterMinis;
            }
            else
            {
                orderPoint.Point = point.Point;
                haveToMinis -= point.Point ?? 0;
                point.Point = 0;
            }
            
            orderPoints.Add(orderPoint);
            needToUpdate.Add( point );
            customerPointLogs.Add(new CustomerPointLog
            {
                OrderId = orders.Id,
                Flag = 0,
                Status = CustomerPointLogStatus.Use.Status,
                Point = point.Point,
                CustomerPointId = point.Id,
                TimeUse = DateTime.Now,
                CreatedAt = DateTime.Now
            });
        }

        if (orderPoints.Count > 0)
        {
            _orderPointRepository.CreateAll(orderPoints);
        }
        if (customerPointLogs.Count > 0)
        {
            _customerPointLogRepository.CreateAll(customerPointLogs);
        }
        if (needToUpdate.Count > 0)
        {
            _customerPointRepository.BulkUpdate(needToUpdate);
        }
    }

    public void Revert(CMS_EF.Models.Orders.Orders orders)
    {
        if (orders.OrderPoint == null)
        {
            return;
        }
        var pointIds = orders.OrderPoint.Select(x => x.CustomerPointId ?? 0).ToList();
        var customerPoint = _customerPointRepository.FindByIds(pointIds);
        if (customerPoint.Count <= 0)
        {
            return;
        }
        List<CustomerPoint> needToUpdates = new List<CustomerPoint>();
        List<CustomerPointLog> customerPointLogs = new List<CustomerPointLog>();

        foreach (OrderPoint orderPoint in orders.OrderPoint)
        {
            var needToUpdate = customerPoint.FirstOrDefault(x => x.Id == orderPoint.CustomerPointId );
            if (needToUpdate == null) continue;
            needToUpdate.Point = (needToUpdate.Point ?? 0) + (orderPoint.Point ?? 0);
            needToUpdates.Add(needToUpdate);
            customerPointLogs.Add(new CustomerPointLog
            {
                OrderId = orders.Id,
                Flag = 0,
                Status =  CustomerPointLogStatus.Revert.Status,
                Point = needToUpdate.Point,
                CustomerPointId = needToUpdate.Id,
                TimeUse = DateTime.Now,
                CreatedAt = DateTime.Now
            });
        }
        if (customerPointLogs.Count > 0)
        {
            _customerPointLogRepository.CreateAll(customerPointLogs);
        }
        if (needToUpdates.Count > 0)
        {
            _customerPointRepository.BulkUpdate(needToUpdates);
        }
    } public void RevertEdit(CMS_EF.Models.Orders.Orders orders)
    {
        if (orders.OrderPoint == null)
        {
            return;
        }
        var pointIds = orders.OrderPoint.Select(x => x.CustomerPointId ?? 0).ToList();
        var customerPoint = _customerPointRepository.FindByIds(pointIds);
        if (customerPoint.Count <= 0)
        {
            return;
        }
        List<CustomerPoint> needToUpdates = new List<CustomerPoint>();
        List<CustomerPointLog> customerPointLogs = new List<CustomerPointLog>();
        List<int> idOrders = new List<int>();
        foreach (OrderPoint orderPoint in orders.OrderPoint)
        {
            idOrders.Add(orderPoint.Id);
            var needToUpdate = customerPoint.FirstOrDefault(x => x.Id == orderPoint.CustomerPointId );
            if (needToUpdate == null) continue;
            needToUpdate.Point = (needToUpdate.Point ?? 0) + (orderPoint.Point ?? 0);
            needToUpdates.Add(needToUpdate);
            customerPointLogs.Add(new CustomerPointLog
            {
                OrderId = orders.Id,
                Flag = 0,
                Status =  CustomerPointLogStatus.Revert.Status,
                Point = needToUpdate.Point,
                CustomerPointId = needToUpdate.Id,
                TimeUse = DateTime.Now,
                CreatedAt = DateTime.Now
            });
        }
        if (customerPointLogs.Count > 0)
        {
            _customerPointLogRepository.CreateAll(customerPointLogs);
        }
        if (needToUpdates.Count > 0)
        {
            _customerPointRepository.BulkUpdate(needToUpdates);
        }
        _orderPointRepository.DeleteAll(idOrders);
    }

    public CustomerPointLog FindByCustomerPointLogId(int orderId)
    {
        var rs = _customerPointLogRepository.FindAll().FirstOrDefault(x => x.OrderId == orderId)!;
        return rs;
    }

    public void ChangePoi(CustomerPoint point, CustomerPointLog pointLog)
    {
        _customerPointRepository.Update(point);
        _customerPointLogRepository.Update(pointLog);
    }

    public List<OrderPoint> FindByIdAndOrderId(int orderId)
    {
        return _customerPointRepository.FindByIdAndOrderId(orderId);
    }
    
}