using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace CMS_Lib.Util
{
    public sealed class CmsFunction
    {
        #region convert

        public static string SplitToLines(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            try
            {
                return text.Replace("\n", "\r\n");
            }
            catch (Exception)
            {
                return text;
            }
        }

        public static string TextAreaShowText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            try
            {
                return text.Replace("\n", "</br>");
            }
            catch (Exception)
            {
                return text;
            }
        }

        public static string DivisionNumberShowString(object a1, object a2, int type = 1)
        {
            try
            {
                if (a2 == null || (a2.ToString() == "0"))
                {
                    return "N/A";
                }

                return NumberFormatShow(RoundDouble(Convert.ToDouble(a1) / Convert.ToDouble(a2), type), true) + "";
                //return NumberFormatShow(RoundDouble(ConvertToDouble(a1.ToString()) / ConvertToDouble(a2.ToString()) ,type))+ "";
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string DivisionNumberRateShowString(object a1, object a2, int type = 1)
        {
            try
            {
                if (a2 == null || (a2.ToString() == "0"))
                {
                    // return "N/A";
                    return "";
                }

                return NumberFormatShow(RoundDouble((Convert.ToDouble(a1) / Convert.ToDouble(a2)) * 100, type), true) + "%";
                //return NumberFormatShow(RoundDouble((ConvertToDouble(a1.ToString()) / ConvertToDouble(a2.ToString())) * 100, type)) + "";
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static double? Division2Number(object a1, object a2, int type = 1)
        {
            try
            {
                if (a2 == null || (a2.ToString() == "0"))
                {
                    return 0;
                }

                return RoundDouble((Convert.ToDouble(a1) / Convert.ToDouble(a2)), type);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static double? DivisionPercentTwoNumber(object a1, object a2, int type = 1)
        {
            try
            {
                if (a2 == null || (a2.ToString() == "0"))
                {
                    return 0;
                }

                return RoundDouble((Convert.ToDouble(a1) / Convert.ToDouble(a2)) * 100, type);
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public static string NumberFormatShow(object value, bool check = true, string format = "#####" )
        {
            if (value == null)
            {
                return "";
            }

            if (string.IsNullOrEmpty(value.ToString()))
            {
                return "";
            }

            if (value.ToString() == "0" && check)
            {
                return "0";
            }
            if (value.ToString() == "0" && !check)
            {
                return "";
            }
            try
            {
                // var info = System.Globalization.CultureInfo.GetCultureInfo("");
                // return string.Format(info, "{0:#,###.#######}", value);
                var culture = new System.Globalization.CultureInfo("es-ES");
                
                return Convert.ToDouble(value)
                    .ToString("##,#.#", culture); 

               
            }
            catch (Exception)
            {
                return value + "";
            }
        }

        public static string NumberPercentFormatShow(object value, string format = "#####")
        {
            if (value == null)
            {
                return "";
            }

            if (string.IsNullOrEmpty(value.ToString()))
            {
                return "";
            }

            if (value.ToString() == "0")
            {
                return "0";
            }

            try
            {
                // var info = System.Globalization.CultureInfo.GetCultureInfo("");
                // return string.Format(info, "{0:#,###.#######}", value);
                // return $"{value:#,0.#,0###}";
                return $"{value:#,0.0}";
            }
            catch (Exception)
            {
                return value + "";
            }
        }

        public static double? RoundDouble(double? value, int type = 1)
        {
            try
            {
                if (value == null)
                {
                    return value;
                }

                return Math.Round(value.Value, type);
            }
            catch (Exception)
            {
                return value;
            }
        }

        #endregion

        public static double? ConvertToDouble(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }

            char systemSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
            double result = 0;
            try
            {
                if (s != null)
                    if (!s.Contains(","))
                        result = double.Parse(s, CultureInfo.InvariantCulture);
                    else
                        result = Convert.ToDouble(s.Replace(".", systemSeparator.ToString())
                            .Replace(",", systemSeparator.ToString()));
            }
            catch (Exception)
            {
                try
                {
                    result = Convert.ToDouble(s);
                }
                catch
                {
                    try
                    {
                        result = Convert.ToDouble(s.Replace(",", ";").Replace(".", ",").Replace(";", "."));
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            return result;
        }

        public static decimal ConvertToDecimal(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }

            char systemSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
            double result = 0;
            try
            {
                if (s != null)
                    if (!s.Contains(","))
                        result = double.Parse(s, CultureInfo.InvariantCulture);
                    else
                        result = Convert.ToDouble(s.Replace(".", systemSeparator.ToString())
                            .Replace(",", systemSeparator.ToString()));
            }
            catch (Exception)
            {
                try
                {
                    result = Convert.ToDouble(s);
                }
                catch
                {
                    try
                    {
                        result = Convert.ToDouble(s.Replace(",", ";").Replace(".", ",").Replace(";", "."));
                    }
                    catch
                    {
                        throw new Exception("Wrong string-to-double format");
                    }
                }
            }

            return Convert.ToDecimal(result);
        }

        public static int? ConvertToInt(string s)
        {
            try
            {
                if (string.IsNullOrEmpty(s))
                {
                    return null;
                }

                return Int32.Parse(s);
            }
            catch
            {
                return null;
            }
        }

        private static string RemapInternationalCharToAscii1(char c)
        {
            string s = c.ToString();
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static string RewriteUrlFriendly(string title)
        {
            if (title == null) return "";

            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char) (c | 64));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                         c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if ((int) c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii1(c));
                    if (prevlen != sb.Length) prevdash = false;
                    try
                    {
                        int check = prevdash ? sb[i - 1] : sb[i];
                        if (check >= 128)
                        {
                            sb[i] = char.Parse("-");
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }
        
        public static string RewriteUrlUpload(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = str.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace(" ","_")
                .Replace('đ', 'd').Replace('Đ', 'D').Replace("(","-").Replace(")","-").Replace("'","-").Replace("\"","-");
        }

        
        public static string GetIp(HttpRequest request)
        {
            var ip = request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = request.Headers["REMOTE_ADDR"].FirstOrDefault();
            }
            else if (string.IsNullOrEmpty(ip))
            {
                ip = request.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            return ip;
        }

        public static int GetWeekFromDateTime(DateTime date)
        {
            DateTimeFormatInfo dateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;
            Calendar calendar = dateTimeFormatInfo!.Calendar;

            return calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }
        public static DateTime? ConvertStringToDateTime(string datetime, string format = "dd/MM/yyyy")
        {
            try
            {
                return DateTime.ParseExact(datetime, format, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
      public static DateTime? ConvertStringToDateTimeH(string datetime)
        {
            try
            {
                var format = "g";
                var provider = new CultureInfo("fr-FR");
                return DateTime.ParseExact(datetime, format, provider);
            }
            catch
            {
                return null;
            }
        }
      public static DateTime? ConvertStringToDateTimeA(string datetime, string format =  "dd/MM/yyyy")
      {
          try
          {
              var datetimeS = datetime.Split(" ");
              var listString = datetimeS[0].Split("/");
              var text = "";
              foreach (var item in listString)
              {
                  if (item.Length == 1)
                  {
                      text += "0" + item + "/";
                  }else if (item.Length >= 4)
                  {
                      text += item;
                  }else if (item.Length == 2)
                  {
                      text +=  item + "/";
                  }
                
              }
              return DateTime.ParseExact(text,  format, CultureInfo.InvariantCulture);
          }
          catch(Exception e)
          {
              return null;
          }
      }
        public static DateTime? ConvertStringToDateMonth(string datetime)
        {
            try
            {
                return DateTime.ParseExact(datetime, "MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? ConvertStringToDateYear(string datetime)
        {
            try
            {
                return DateTime.ParseExact(datetime, "yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public static string BindNameParenChild(string name, int? level)
        {
            if (level == 1)
            {
                return name;
            }
            else if (level > 1)
            {
                string name1 = "";
                for (int i = 0; i < (level - 1); i++)
                {
                    name1 += "--";
                }

                name1 += " " + name;
                return name1;
            }

            return name;
        }

        public static string Trucate(string s, int max)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            if (s.Length <= max)
            {
                return s;
            }
            else
            {
                string s1 = s.Substring(0, max - 5);
                string s2 = s.Substring(s.Length - 5, 5);
                return s1 + "..." + s2;
            }
        }

        public static string TimeAgo(DateTime dateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = $"{timeSpan.Seconds} giây trước";
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1
                    ? $"{timeSpan.Minutes} phút trước"
                    : "1 phút trước";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ? $"{timeSpan.Hours} giờ trước" : "1 giờ trước";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ? $"{timeSpan.Days} ngày trước" : "1 ngày trước";
            }

            return dateTime.ToString("dd/MM/yyyy hh:mm:ss");
        }

        public static bool IsHtml(string checkString)
        {
            if (string.IsNullOrEmpty(checkString))
            {
                return false;
            }

            return Regex.IsMatch(checkString, "<(.|\n)*?>");
        }
        public static bool IsBackUrl(string checkString)
        {
            if (string.IsNullOrEmpty(checkString))
            {
                return false;
            }

            if (checkString.StartsWith("javascript:"))
            {
                return true;
            }

            if (checkString.StartsWith("../"))
            {
                return true;
            }
            
            return Regex.IsMatch(checkString, "<(.|\n)*?>");
        }


        public static string ConvertDateTimeToQuarterString(DateTime date)
        {
            return $"Quý {(date.Month + 2) / 3} {date.Year}";
        }

        public static List<DateTime> ParseQuarterToStartEndDate(string t)
        {
            try
            {
                int quarter = 0;
                int year = 0;
                if (t.StartsWith("Quý 1"))
                {
                    quarter = 1;
                    year = Int32.Parse(t.Replace("Quý 1", "").Trim());
                }
                else if (t.StartsWith("Quý 2"))
                {
                    quarter = 2;
                    year = Int32.Parse(t.Replace("Quý 2", "").Trim());
                }
                else if (t.StartsWith("Quý 3"))
                {
                    quarter = 3;
                    year = Int32.Parse(t.Replace("Quý 3", "").Trim());
                }
                else if (t.StartsWith("Quý 4"))
                {
                    quarter = 4;
                    year = Int32.Parse(t.Replace("Quý 4", "").Trim());
                }

                if (quarter > 0 && year > 0)
                {
                    int monthStart = (quarter * 3) - 2;
                    DateTime tStart = new DateTime(year, monthStart, 1);
                    DateTime tEnd = tStart.AddMonths(3);
                    return new List<DateTime>() {tStart, tEnd};
                }

                return new List<DateTime>();
            }
            catch (Exception)
            {
                return new List<DateTime>();
            }
        }
        public static int ParseQuarterToInt(string t)
        {
            try
            {
                int quarter = 0;
                if (t.StartsWith("Quý 1"))
                {
                    quarter = 1;
                }
                else if (t.StartsWith("Quý 2"))
                {
                    quarter = 2;
                }
                else if (t.StartsWith("Quý 3"))
                {
                    quarter = 3;
                }
                else if (t.StartsWith("Quý 4"))
                {
                    quarter = 4;
                }
                return quarter;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int ParseQuarterYear(string t)
        {
            int year = DateTime.Now.AddMonths(-3).Year;
            if (!string.IsNullOrEmpty(t))
            {
                if (t.StartsWith("Quý 1"))
                {
                    year = Int32.Parse(t.Replace("Quý 1", "").Trim());
                }
                else if (t.StartsWith("Quý 2"))
                {
                    year = Int32.Parse(t.Replace("Quý 2", "").Trim());
                }
                else if (t.StartsWith("Quý 3"))
                {
                    year = Int32.Parse(t.Replace("Quý 3", "").Trim());
                }
                else if (t.StartsWith("Quý 4"))
                {
                    year = Int32.Parse(t.Replace("Quý 4", "").Trim());
                }
            }

            return year;
        }

        public static int ParseIndexQuarterYear(string t)
        {
            DateTime now = DateTime.Now.AddMonths(-3);
            int index = (now.Month + 2) / 3 - 1;
            if (!string.IsNullOrEmpty(t))
            {
                if (t.StartsWith("Quý 1"))
                {
                    index = 0;
                }
                else if (t.StartsWith("Quý 2"))
                {
                    index = 1;
                }
                else if (t.StartsWith("Quý 3"))
                {
                    index = 2;
                }
                else if (t.StartsWith("Quý 4"))
                {
                    index = 3;
                }
            }

            return index;
        }

        public static string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public static string Md5Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
            
            // MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            // byte[] bs = Encoding.UTF8.GetBytes(input);
            // bs = x.ComputeHash(bs);
            // StringBuilder s = new StringBuilder();
            // foreach (byte b in bs)
            // {
            //     s.Append(b.ToString("x2").ToLower());
            // }
            //
            // String hash = s.ToString();
            // return hash;
        }
        
        public static string RemoveUnicode(string str)
        {
            try
            {
                Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                string temp = str.Normalize(NormalizationForm.FormD);
                return regex.Replace(temp, String.Empty)
                    .Replace('đ', 'd').Replace('Đ', 'D');
            }
            catch (Exception)
            {
                return str;
            }
        }

        public static string GetLayoutBackground(string str, string color = "#5a82c0")
        {
            return  $"<span class=\"status badge mr-1   text-white\" style=\"background:{color}\">{str}</span>";
        }

        public static bool IsValidUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }
            try
            {
                return  Regex.IsMatch(userName, @"^[a-zA-Z0-9?><;.,{}[\]\-_+=!@#$%\^&*|']*$");
            }
            catch (Exception)
            {
                // ignored
            }

            return false;
        }
    }
}