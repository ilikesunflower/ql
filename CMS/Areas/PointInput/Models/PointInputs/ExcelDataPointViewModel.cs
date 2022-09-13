using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace CMS.Areas.PointInput.Models.PointInputs;

public class ExcelDataPointViewModel
{
    public int CreateBy { set; get; } 
    public string FileName { set; get; } 
    public string Code { set; get; } 
    public string ReleaseBy { set; get; }
    public string LinkFile { get; set; }
    public List<ExcelDataListPointViewModel> ListPoint { set; get; }
}

public class ExcelDataListPointViewModel
{
    public int CustomerId { set; get; } 
    public string CustomerUserName { set; get; } 
    public double PlusPoint { set; get; } 
    public double MinusPoint { set; get; } 
    public double Point { set; get; } 
    public DateTime Start { set; get; } 
    public DateTime End { set; get; } 
}