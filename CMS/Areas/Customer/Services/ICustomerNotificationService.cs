using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CMS.Extensions.Notification;
using CMS_Access.Repositories.Customers;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using CMS_Lib.Services.HttpContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Customer.Services;

public interface ICustomerNotificationService : IScoped
{
    public void SendCustomerNotification(CustomerNotificationObject o);
    public void SendCustomerNotification(int customerId,CustomerNotificationObject o);
}

public class CustomerNotificationService : ICustomerNotificationService
{
    private readonly ILogger<CustomerNotificationService> _iLogger;
    private readonly INotificationBackgroundTaskQueue _iNotificationBackgroundTaskQueue;
    private IFeNotificationRepository _iFeNotificationRepository;
    private ICustomerRepository _iCustomerRepository;
    private IHttpContextService _iHttpContextService;
    private readonly string domainFe;
    private readonly string tokenFe;
    
    public CustomerNotificationService(ILogger<CustomerNotificationService> iLogger
        ,INotificationBackgroundTaskQueue iNotificationBackgroundTaskQueue,
        IFeNotificationRepository iFeNotificationRepository, IHttpContextService iHttpContextService,
        IConfiguration iConfiguration, ICustomerRepository iCustomerRepository)
    {
        _iLogger = iLogger;
        _iNotificationBackgroundTaskQueue = iNotificationBackgroundTaskQueue;
        _iFeNotificationRepository = iFeNotificationRepository;
        _iHttpContextService = iHttpContextService;
        _iCustomerRepository = iCustomerRepository;
        domainFe = iConfiguration.GetSection("AppSetting:FEDomain").Get<string>();
        tokenFe = iConfiguration.GetSection("AppSetting:FEToken").Get<string>();
    }


    public void SendCustomerNotification(CustomerNotificationObject o)
    {
        try
        {
            _iNotificationBackgroundTaskQueue.QueueBackgroundWorkItem((serviceScopeFactory, token) =>
            {
                _iCustomerRepository = serviceScopeFactory.CreateScope().ServiceProvider
                    .GetRequiredService<ICustomerRepository>();
                _iFeNotificationRepository = serviceScopeFactory.CreateScope().ServiceProvider
                    .GetRequiredService<IFeNotificationRepository>();
                _iHttpContextService = serviceScopeFactory.CreateScope().ServiceProvider
                    .GetRequiredService<IHttpContextService>();
                var listCustomer = _iCustomerRepository.FindAll().Where(x => x.Status == 1).Select(x => x.Id).ToList();
                if (listCustomer is { Count: > 0 })
                {
                    FeNotification rs = new FeNotification()
                    {
                        Detail = o.Detail,
                        Flag = 0,
                        Link = o.Link,
                        Title = o.Title,
                        CreatedAt = DateTime.Now,
                    };

                    rs.FeNotificationCustomer = new List<FeNotificationCustomer>();
                    foreach (var v in listCustomer)
                    {
                        rs.FeNotificationCustomer.Add(new FeNotificationCustomer()
                        {
                            CustomerId = v,
                            CreatedAt = DateTime.Now,
                            FeNotification = rs,
                            IsRead = false,
                            Flag = 0
                        });
                    }
                    _iFeNotificationRepository.Create(rs);
                    SendNotificationFe(_iHttpContextService, listCustomer,o);
                }
                return Task.CompletedTask;
            });
        }
        catch (Exception ex)
        {
            // ignored
            this._iLogger.LogError(ex,"Gửi thông báo đến toàn bộ khách hàng");
        }
    }

    public void SendCustomerNotification(int customerId, CustomerNotificationObject o)
    {
        try
        {
            _iNotificationBackgroundTaskQueue.QueueBackgroundWorkItem((serviceScopeFactory, token) =>
            {
                _iFeNotificationRepository = serviceScopeFactory.CreateScope().ServiceProvider
                    .GetRequiredService<IFeNotificationRepository>();
                _iHttpContextService = serviceScopeFactory.CreateScope().ServiceProvider
                    .GetRequiredService<IHttpContextService>();
                FeNotification rs = new FeNotification()
                {
                    Detail = o.Detail,
                    Flag = 0,
                    Link = o.Link,
                    Title = o.Title,
                    CreatedAt = DateTime.Now,
                };
                rs.FeNotificationCustomer = new List<FeNotificationCustomer>()
                {
                    new FeNotificationCustomer()
                    {
                        CustomerId = customerId,
                        FeNotification = rs,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                        Flag = 0
                    }
                };
                _iFeNotificationRepository.Create(rs);
                SendNotificationFe(_iHttpContextService, new List<int>(){customerId},o);
                return Task.CompletedTask;
            });
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex,$"SendCustomerNotification đến khách hàng {customerId}");
        }
    }

    private void SendNotificationFe(IHttpContextService iHttpContextService,List<int> customer , CustomerNotificationObject data)
    {
        try
        {
            string url = $"{domainFe}/Notifications/Notification/Send?token={tokenFe}";
            var body = new
            {
                Customers = customer,
                Title = data.Title,
                Detail = data.Detail,
                Link = data.Link
            };
            var response = iHttpContextService
                .PostJsonAsync(new HttpClient(), url,body, new Dictionary<string, string>()).Result;
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    this._iLogger.LogInformation($"send notification to FE: {res}");
                }   
            }
            else
            {
                string res = response.Content.ReadAsStringAsync().Result;
                this._iLogger.LogError($"send notification to FE: {res}");
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}

public class CustomerNotificationObject
{
    public string Title { get; set; }
    
    public string Detail { get; set; }
    
    public string Link { get; set; }
}