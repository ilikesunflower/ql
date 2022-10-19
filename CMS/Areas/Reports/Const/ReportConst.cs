using System;
using System.Collections.Generic;
using Castle.Core.Internal;
using ClosedXML.Excel;
using CMS_Lib.Util;

namespace CMS.Areas.Reports.Const
{
    public class ReportConst
    {
        public static void SetExcelRangeBgColor(IXLRange w, string color = "#D9D9D9", bool bold =true, int font = 11)
        {
            if (!string.IsNullOrEmpty(color))
            {
                if (bold)
                {
                    w.Style.Font.Bold = true;
                }
                w.Style.Fill.BackgroundColor = XLColor.FromHtml(color);
             w.Style.Font.FontSize = font;
                w.Style.Font.FontName = "Times New Roman";
            }
        }

        public static void SetText(IXLCell cell, string v, string color = "#000000", int font = 11)
        {
            cell.Style.Font.FontSize = font;
            cell.Style.Font.FontName = "Times New Roman";
            cell.SetValue(v);
            cell.Style.Alignment.WrapText = true;
            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }
        
        public static void SetTextTitle(IXLCell cell, string v, string color = "#000000",  int font = 11)
        {
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontSize = font;
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
            // range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

    }

}