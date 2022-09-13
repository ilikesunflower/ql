namespace CMS_Lib.Helpers;

public class HashHelper
{
    public static string? HashCharacter(string k)
    {
        string hashCode = $"{k.GetHashCode():X}";
        return hashCode;
    }
}