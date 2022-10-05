﻿using Ganss.XSS;

namespace CMS_Lib.Helpers;

public class HtmlSanitizerHelper
{
    public static string Sanitize(string v)
    {
        if (string.IsNullOrEmpty(v))
        {
            return v;
        }

        string s = v.Trim();
        var sanitizer = new HtmlSanitizer();
        var sanitized = sanitizer.Sanitize(v);
        return sanitized;
    }
    
    public static bool IsXss(string v)
    {
        if (string.IsNullOrEmpty(v))
        {
            return false;
        }
    
        string s = v.Trim();
        s = s.Replace("@", "*");
        s = s.Replace(".", "*");
        s = s.Replace("&", "*");
        var sanitizer = new HtmlSanitizer();
        var sanitized = sanitizer.Sanitize(s);
        return sanitized.Length != s.Length;
    }
}