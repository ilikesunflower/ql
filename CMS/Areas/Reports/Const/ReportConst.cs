using System;
using System.Collections.Generic;
using Castle.Core.Internal;
using ClosedXML.Excel;
using CMS_Lib.Util;

namespace CMS.Areas.Reports.Const
{
    public class ReportConst
    {
        public static void SetExcelRangeBgColor(IXLRange w, string color = "#D9D9D9")
        {
            if (!string.IsNullOrEmpty(color))
            {
                w.Style.Font.Bold = true;
                w.Style.Fill.BackgroundColor = XLColor.FromHtml(color);
             w.Style.Font.FontSize = 11;
                w.Style.Font.FontName = "Times New Roman";
            }
        }

        public static void SetText(IXLCell cell, string v, string color = "#000000")
        {
            cell.Style.Font.FontSize = 11;
            cell.Style.Font.FontName = "Times New Roman";
            cell.SetValue(v);
            cell.Style.Alignment.WrapText = true;
            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }
        
        public static void SetTextTitle(IXLCell cell, string v, string color = "#000000")
        {
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontSize = 11;
            cell.Style.Font.FontName = "Times New Roman";
            cell.SetValue(v);
            cell.Style.Alignment.WrapText = true;
            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }
        public static void MergeStyleExcel(IXLRange range)
        {
            range.Merge();
            range.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

    }

}