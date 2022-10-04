using System.Collections.Generic;
using System.Linq;

namespace CMS.Areas.Reports.Const;

public class AfterSaleConst
{
  public static int month = 1;
  public static int quarter = 2;
  public static int year = 3;
  public static string PathSaveFile { get; } = "ReportAfterSaleFile";
  
  public static Dictionary<int, string> ListAfterSaleCost = new Dictionary<int, string>()
  {
    {month, "Tháng"},
    {quarter, "Quý"},
    {year, "Năm"}
  };

  public static string GetTimeAfterSale(int type)
  {
    return ListAfterSaleCost.Where(x => x.Key == type).Select(x => x.Value).FirstOrDefault();
  }
  public static string GetTime(int? type, int? dateM, int? dateQ, int? dateY)
  {
    if (!type.HasValue)
    {
      return "";
    }
    if (type == month)
    {
      return "Tháng " + dateM + " Năm " + dateY;
    }else if (type == quarter)
    {
      return "Quý " + dateQ + " Năm " + dateY;
    }
    else
    {
      return "Năm "  + dateY;
    }
  }
}