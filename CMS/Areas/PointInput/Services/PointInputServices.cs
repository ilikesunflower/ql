using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using ClosedXML.Excel;
using CMS_Access.Repositories;
using CMS_Access.Repositories.Customers;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using CMS.Areas.Customer.Services;
using CMS.Areas.PointInput.Models.PointInputs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace CMS.Areas.PointInput.Services;

public interface IPointInputServices : IScoped
{
    ExcelDataPointViewModel ReadDataFromExcelAndValidate(IFormFile formFile);
    void ImportData(ExcelDataPointViewModel dataFile);
    void Delete(int id);
    void Delete(List<int> id);
    void SendNotification(int id);
}

public class PointInputServices : IPointInputServices
{
    private readonly ICustomerService _customerService;
    private readonly IDatabaseTransaction _databaseTransaction;
    private readonly ICustomerPointRepository _customerPointRepository;
    private readonly IHistoryFileChargePointRepository _historyFileChargePointRepository;
    private readonly ICustomerNotificationService _customerNotificationService;

    public PointInputServices(ICustomerService customerService,
        IHistoryFileChargePointRepository historyFileChargePointRepository,
        ICustomerPointRepository customerPointRepository, IDatabaseTransaction databaseTransaction,
        ICustomerNotificationService customerNotificationService)
    {
        _customerService = customerService;
        _historyFileChargePointRepository = historyFileChargePointRepository;
        _customerPointRepository = customerPointRepository;
        _databaseTransaction = databaseTransaction;
        _customerNotificationService = customerNotificationService;
    }


    public ExcelDataPointViewModel ReadDataFromExcelAndValidate(IFormFile formFile)
    {
        ExcelDataPointViewModel fileData = new ExcelDataPointViewModel
        {
            FileName = formFile.FileName
        };
        XLWorkbook workbook = new XLWorkbook(formFile.OpenReadStream());
        IXLWorksheet ws = workbook.Worksheet(1);
        IXLRange range = ws.RangeUsed();
        int rowCount = range.RowCount();

        List<CMS_EF.Models.Customers.Customer> customers = _customerService.FindAllEnable();

        IXLCell codeCell = range.Cell(2, 3);
        IXLCell releaseByCell = range.Cell(3, 3);

        if (IsNullOrEmptyIxlCell(codeCell))
        {
            throw new NullReferenceException("Mã phiếu yêu cầu ô C:2 rỗng");
        }

        if (_historyFileChargePointRepository.IsCodeAlreadyExist(codeCell.GetString()))
        {
            throw new Exception("Mã phiếu yêu cầu đã tồn tại");
        }

        if (IsNullOrEmptyIxlCell(releaseByCell))
        {
            throw new NullReferenceException("Bộ phận phát hành điểm: ô C:3 rỗng");
        }

        fileData.Code = codeCell.GetString();
        fileData.ReleaseBy = releaseByCell.GetString();

        var listData = new EditableList<ExcelDataListPointViewModel>();

        for (var i = 6; i <= rowCount; i++)
        {
            IXLCell customerUserNameCell = range.Cell(i, 2);
            IXLCell plusPointCell = range.Cell(i, 3);
            IXLCell minusPointCell = range.Cell(i, 4);
            IXLCell startCell = range.Cell(i, 6);
            IXLCell endCell = range.Cell(i, 7);
  
            bool isCustomerUserNameCellNull = IsNullOrEmptyIxlCell(customerUserNameCell);
            bool isStartCellNull = IsNullOrEmptyIxlCell(startCell);
            bool isEndCellNull = IsNullOrEmptyIxlCell(endCell);
            bool isPlusPointCellNull = IsNullOrEmptyIxlCell(plusPointCell);
            bool isMinusPointCellNull = IsNullOrEmptyIxlCell(minusPointCell);

            if (isCustomerUserNameCellNull &&
                isStartCellNull &&
                isEndCellNull &&
                isPlusPointCellNull &&
                isMinusPointCellNull)
            {
                continue;
            }

            if (isCustomerUserNameCellNull)
            {
                throw new NullReferenceException($"Tài khoản nhận điểm ô B:{i} rỗng");
            }

            if (isStartCellNull)
            {
                throw new NullReferenceException($"Ngày bắt đầu ô F:{i} rỗng");
            }

            if (isEndCellNull)
            {
                throw new NullReferenceException($"Ngày kết thúc ô G:{i} rỗng");
            }

            var customerUserName = customerUserNameCell.GetString();

            var customer = customers.FirstOrDefault(x => x.UserName == customerUserName.Trim());
            if (customer == null)
            {
                throw new NullReferenceException(
                    $"Khách hàng {customerUserName} ô B:{i} chưa tồn tại trên hệ thống. Vui lòng thêm trước khi nhập dữ liệu");
            }

            ExcelDataListPointViewModel dataRow = new ExcelDataListPointViewModel()
            {
                CustomerId = customer.Id,
                CustomerUserName = customer.UserName
            };

            if (isPlusPointCellNull)
            {
                dataRow.PlusPoint = 0;
            }
            else
            {
                if (double.TryParse(plusPointCell.GetString(), out var plusPoint))
                {
                    if (plusPoint < 0)
                    {
                        throw new NullReferenceException($"Số điểm trừ ô C:{i} phải lớn hơn 0");
                    }

                    dataRow.PlusPoint = plusPoint;
                }
                else
                {
                    throw new NullReferenceException($"Số điểm nạp ô C:{i} không phải kiểu số");
                }
            }

            if (isMinusPointCellNull)
            {
                dataRow.MinusPoint = 0;
            }
            else
            {
                if (double.TryParse(minusPointCell.GetString(), out var minusPoint))
                {
                    if (minusPoint < 0)
                    {
                        throw new NullReferenceException($"Số điểm trừ ô D:{i} phải lớn hơn 0");
                    }

                    dataRow.MinusPoint = minusPoint;
                }
                else
                {
                    throw new NullReferenceException($"Số điểm trừ ô D:{i} không phải kiểu số");
                }
            }

            dataRow.Point = dataRow.PlusPoint - dataRow.MinusPoint;
            if (startCell.DataType == XLDataType.DateTime)
            {
                dataRow.Start = (DateTime) startCell.Value;
            }
            else
            {
                throw new NullReferenceException($"Ngày bắt đầu ô F:{i} không phải kiểu ngày tháng");
            }

            if (endCell.DataType == XLDataType.DateTime)
            {
                var endTime = (DateTime) endCell.Value;
                var endTimeDate = new DateTime(endTime.Year, endTime.Month, endTime.Day, 23, 59, 59); 
                dataRow.End = endTimeDate;
            }
            else
            {
                throw new NullReferenceException($"Ngày kết thúc ô G:{i} không phải kiểu ngày tháng");
            }

            if (dataRow.Start > dataRow.End)
            {
                throw new NullReferenceException($"Row {i} ngày kết thúc không được nhỏ hơn ngày bắt đầu");
            }

            listData.Add(dataRow);
        }

        if (listData.Count < 1)
        {
            throw new NullReferenceException("Danh sách điểm không được để trống");
        }

        fileData.ListPoint = listData;

        return fileData;
    }

    public void ImportData(ExcelDataPointViewModel dataFile)
    {
        IDbContextTransaction transaction = _databaseTransaction.BeginTransaction();
        try
        {
            var historyFileChargePoint = new HistoryFileChargePoint
            {
                Code = dataFile.Code,
                ReleaseBy = dataFile.ReleaseBy,
                Flag = 0,
                IsSentNotification = 0,
                CreatedAt = DateTime.Now,
                CreatedBy = dataFile.CreateBy,
                FileName = dataFile.FileName,
                LinkFile = dataFile.LinkFile
            };
            _historyFileChargePointRepository.Create(historyFileChargePoint);
            List<CustomerPoint> customerPoints = dataFile.ListPoint.Select(excelDataListPointViewModel =>
                    new CustomerPoint
                    {
                        Flag = 0,
                        Point = excelDataListPointViewModel.Point,
                        AddPoint = excelDataListPointViewModel.PlusPoint,
                        MinusPoint = excelDataListPointViewModel.MinusPoint,
                        CustomerId = excelDataListPointViewModel.CustomerId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = dataFile.CreateBy,
                        EndTime = excelDataListPointViewModel.End,
                        StartTime = excelDataListPointViewModel.Start,
                        LastModifiedAt = DateTime.Now,
                        HistoryFileChargeFileId = historyFileChargePoint.Id
                    })
                .ToList();
            _customerPointRepository.CreateAll(customerPoints);

            _databaseTransaction.Commit(transaction);
        }
        catch
        {
            _databaseTransaction.Rollback(transaction);
            throw;
        }
    }

    public void Delete(int id)
    {
        var transaction = _databaseTransaction.BeginTransaction();
        try
        {
            var file = _historyFileChargePointRepository.FindById(id);
            if (file == null)
            {
                throw new NullReferenceException("Không tìm thấy file");
            }

            _historyFileChargePointRepository.Delete(file);
            var customerPoints = _customerPointRepository.FindByFileId(file.Id).ToList();
            _customerPointRepository.DeleteAll(customerPoints);


            _databaseTransaction.Commit(transaction);
        }
        catch
        {
            _databaseTransaction.Rollback(transaction);
            throw;
        }
    }

    public void Delete(List<int> id)
    {
        var transaction = _databaseTransaction.BeginTransaction();
        try
        {
            var files = _historyFileChargePointRepository.FindByIds(id);
            if (files == null || files.Count == 0)
            {
                throw new NullReferenceException("Không tìm thấy files");
            }

            _historyFileChargePointRepository.DeleteAll(files);
            var customerPoints = _customerPointRepository.FindByFileIds(files.Select(x => x.Id).ToList()).ToList();
            _customerPointRepository.DeleteAll(customerPoints);

            _databaseTransaction.Commit(transaction);
        }
        catch
        {
            _databaseTransaction.Rollback(transaction);
            throw;
        }
    }

    public void SendNotification(int id)
    {
        var file = _historyFileChargePointRepository.FindById(id);
        if (file == null)
        {
            throw new NullReferenceException("Không tìm thấy files");
        }
        if (file.IsSentNotification == 1)
        {
            throw new NullReferenceException("Đã thông báo trước đó! không thể tiếp tục gửi!");
        }
        var customerPoints = _customerPointRepository.FindByFileId(file.Id).ToList();
        file.IsSentNotification = 1;
        _historyFileChargePointRepository.Update(file);
        foreach (var customerPoint in customerPoints)
        {
            var title = customerPoint.Point > 0
                ? $"Quý Khách hàng được tặng {customerPoint.Point} điểm thưởng"
                : $"Quý Khách hàng bị trừ {customerPoint.Point * -1} điểm thưởng";

            var detail = customerPoint.Point > 0 
                ? $"Điểm thưởng sẽ hết hạn vào {customerPoint.EndTime?.ToString("dd/MM/yyyy")}. NHANH TAY SỬ DỤNG NGAY!" 
                : "Vui lòng liên hệ hotline: 0886991247 để biết thêm thông tin";
            
            var link = customerPoint.Point > 0 ? "/danh-sach-san-pham" : "";
            _customerNotificationService.SendCustomerNotification(new CustomerNotificationObject()
            {
                Title = title,
                Detail = detail,
                Link = link
            });
        }
    }

    private bool IsNullOrEmptyIxlCell(IXLCell cell)
    {
        return cell.IsEmpty() || string.IsNullOrEmpty(cell.GetString());
    }
}