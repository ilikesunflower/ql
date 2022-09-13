using Castle.Core.Internal;
using CMS_Access.Repositories;
using CMS_EF.Models;
using CMS_Lib.DI;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace CMS.Services.Loggings
{
    public interface ILoggingService : IScoped
    {
        void Infor(ILogger ilogger, string action = null, string detail = null);

        void Error(ILogger ilogger, string action = null, string detail = null, Exception exception = null);

        void System(ILogger ilogger, string action = null, string detail = null);
    }
    public class LoggingService : ILoggingService
    {
        private readonly IHttpContextAccessor _context;
        private readonly ILoggingRepository _iLoggingRepository;

        public LoggingService(IHttpContextAccessor context, ILoggingRepository iLoggingRepository)
        {
            this._context = context;
            this._iLoggingRepository = iLoggingRepository;
        }


        public void Infor(ILogger ilogger, string action, string detail)
        {
            try
            {
                int userId = Int32.Parse(_context.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                ilogger.LogInformation($"{action} - {detail} - userId: {userId}");
                Logging logging = InputData(action, detail, 1);
                if (logging != null)
                {
                    _iLoggingRepository.Create(logging);
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
        }

        public void Error(ILogger ilogger, string action, string detail, Exception exception = null)
        {
            try
            {
                int userId = Int32.Parse(_context.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                ilogger.LogError(exception, $"{action} - {detail} - userId: {userId}");
                if (exception != null)
                {
                    detail += " " + exception;
                }
                Logging logging = InputData(action, detail, 2);
                _iLoggingRepository.Create(logging);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
        }

        public void System(ILogger ilogger, string action, string detail)
        {
            try
            {
                int userId = Int32.Parse(_context.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                ilogger.LogInformation($"{action} - {detail} - userId: {userId}");
                Logging logging = InputData(action, detail, 3);
                if (logging != null)
                {
                    _iLoggingRepository.Create(logging);
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
        }

        private Logging InputData(string action, string detail, int logLevel)
        {
            int userId = Int32.Parse(_context.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (_context.HttpContext != null)
            {
                Logging logging = new Logging
                {
                    Action = action,
                    Detail = detail,
                    LogLevel = logLevel,
                    Ip = _context.HttpContext != null && _context.HttpContext.Request.Headers["X-Forwarded-For"]
                        .FirstOrDefault().IsNullOrEmpty()
                        ? _context.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                        : _context.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault(),
                    //Flag = 0,
                    UserId = userId,
                    UserFullName = _context.HttpContext.User.FindFirstValue(CmsClaimType.UserName),
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now,
                    LastModifiedAt = DateTime.Now,
                    LastModifiedBy = userId,
                    UserAgent = _context.HttpContext.Request.Headers["User-Agent"]
                };
                return logging;
            }

            return null;
        }
    }
}
