using System.Collections.Generic;
using System.Linq;
using CMS.Areas.OrderComment.Models;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace CMS.Areas.OrderComment.Const;

public class OrderComment
{
    public static bool GetOrderIndex(int index, List<CommnentOrder> commnet)
    {
        if (index == 1)
        {
            return true;
        }
        var first = 0;
        for (int i = 0; i < commnet.Count; i++)
        {
            first += commnet[i].Count;
            if (index == (first + 1))
            {
                return true;
            }
        }

        return false;
    }
    public static int GetOrderCount(string code, List<CommnentOrder> commnet)
    {

        var count = commnet.Where(x => x.Code == code).Select(x => x.Count).FirstOrDefault();
        return count;
    }

    public static List<string> SplitDefaultComment(string commentDefault)
    {
        List<string> listSplit = new List<string>();
        if (commentDefault == null)
        {
            return listSplit;
        }
        var count = commentDefault.Split('/');
        foreach (var word in count)
        {
            listSplit.Add(word);
        }
        return listSplit;
    }
}