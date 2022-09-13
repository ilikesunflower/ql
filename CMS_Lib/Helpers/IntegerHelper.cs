using System;

namespace CMS_Lib.Helpers;

public class IntegerHelper
{
    public static int? ParseStringToInt(string s)
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
}