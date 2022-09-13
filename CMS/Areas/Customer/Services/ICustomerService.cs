using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using CMS.Areas.Customer.Const;
using CMS.Areas.Customer.Models.Customer;
using CMS.Services.Emails;
using CMS_Access.Repositories.Customers;
using CMS_Lib.DI;
using CMS_Lib.Helpers;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Customer.Services;

public interface ICustomerService : IScoped
{
    List<CMS_EF.Models.Customers.Customer> FindAllEnable();
    IQueryable<CMS_EF.Models.Customers.Customer> GetAll();
    CMS_EF.Models.Customers.Customer Create(CreateViewModel model);
    public bool ResetPassWord(int id);
}

public class CustomerService : ICustomerService
{
    private readonly ILogger<CustomerService> _ILogger;
    private readonly ICustomerRepository _ICustomerRepository;
    private readonly IEmailService _iEmailService;

    public CustomerService(ILogger<CustomerService> iLogger, ICustomerRepository iCustomerRepository,
        IEmailService iEmailService)
    {
        _ILogger = iLogger;
        _ICustomerRepository = iCustomerRepository;
        _iEmailService = iEmailService;
    }

    public List<CMS_EF.Models.Customers.Customer> FindAllEnable()
    {
        return _ICustomerRepository.FindAll().Where(x => x.Flag == 0).ToList();
    }

    public IQueryable<CMS_EF.Models.Customers.Customer> GetAll()
    {
        return this._ICustomerRepository.FindAll();
    }

    public  CMS_EF.Models.Customers.Customer Create(CreateViewModel model)
    {
        string password = $"@a{HashHelper.HashCharacter(model.UserName)}";
        CMS_EF.Models.Customers.Customer rs = new CMS_EF.Models.Customers.Customer()
        {
            Detail = model.Detail,
            Email = model.Email,
            UserName = model.UserName,
            FullName = model.FullName,
            Phone = model.Phone,
            Status = 1,
            TypeGroup = CustomerTypeGroupConst.PhongBan,
            Org = model.Org,
            Type = CustomerConst.TypeOrgPru,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        };
        var data = this._ICustomerRepository.Create(rs); 
        SendEmail(new List<string>(){data.Email},"[Prugift.vn] Đăng ký tài khoản thành công",
            "Tài khoản của bạn đã được kích hoạt trên hệ thống Prugift.vn:",data.UserName,password);
        return data;
    }

    public bool ResetPassWord(int id)
    {
       CMS_EF.Models.Customers.Customer data = this._ICustomerRepository.FindById(id);
       if (data != null)
       {
           DateTime t = DateTime.Now;
           string password = $"@a{HashHelper.HashCharacter(data.UserName + t.Ticks)}";
           data.Password = BCrypt.Net.BCrypt.HashPassword(password);
           this._ICustomerRepository.Update(data);
           if (!string.IsNullOrEmpty(data.Email))
           {
               SendEmail(new List<string>() { data.Email }, "[Prugift.vn] Mật khẩu của bạn đã được đổi thành công",
                   "Mật khẩu của bạn đã được đổi trên hệ thống Prugift.vn:", data.UserName, password);
           }

           return true;
       }

       return false;
    }

    private void SendEmail(List<string> email, string subject, string  title, string userName, string passWord)
    {
        string content = $"<div style='font-size: 15px;'>\n" +
                         "            <div>\n" +
                         $"                <span style=\"white-space:pre-wrap\">{title}</span>\n" +
                         "            </div>\n" +
                         "            <div>\n" +
                         "                <ul>\n" +
                         "                    <li>\n" +
                         $"                        <span style=\"white-space:pre-wrap\">Tên Đăng nhập: <span style=\"color: black;\">{userName}</span></span>\n" +
                         "                    </li>\n" +
                         "                    <li>\n" +
                         $"                        <span style=\"white-space:pre-wrap\">Mật Khẩu: <span style=\"color: black;\">{passWord}</span></span>\n" +
                         "                    </li>\n" +
                         "                </ul>\n" +
                         "            </div>\n" +
                         "            <div>\n" +
                         "                <span  style=\"white-space:pre-wrap\">Vui lòng sử dụng tài khoản được cung cấp cho các lần đăng nhập tiếp theo.</span>\n" +
                         "            </div>\n" +
                         "            <div>\n" +
                         "                <span style=\"white-space:pre-wrap\">Truy cập website <a href=\"https://prugift.vn\" target=\"_blank\">prugift.vn tại đây</a> \n" +
                         "                </span>\n" +
                         "            </div>\n" +
                         "            <div>\n" +
                         "                <span  style=\"white-space:pre-wrap\">Mọi thắc mắc và hỗ trợ vui lòng liên hệ hotline: </span>0886 991 247\n" +
                         "            </div>\n" +
                         "            <div><br></div>\n" +
                         "            <div>\n" +
                         "                <span style=\"white-space:pre-wrap\">Chân thành cảm ơn!</span>\n" +
                         "            </div>\n" +
                         "        </div>";
        Message msg = new Message(email, subject, content);
       this._iEmailService.SendEmailAsync(msg);
    }
}