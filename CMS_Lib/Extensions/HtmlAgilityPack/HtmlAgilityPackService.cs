using System;
using System.Linq;
using HtmlAgilityPack;

namespace CMS_Lib.Extensions.HtmlAgilityPack;

public class HtmlAgilityPackService
{
    public static string DeleteBase64(string html)
    {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var result = "";
            foreach (var node in htmlDoc.DocumentNode.DescendantNodes())
            {
                var attr = node.GetAttributes("style");
                var listAttr = attr.ToList();
                if (listAttr[0] != null)
                {
                    foreach (var atrbu in  listAttr)
                    {
                        if (atrbu == null)
                        {
                      
                        }
                        var stringAtrr = atrbu.Value;
                        var check = stringAtrr.IndexOf("base64");
                        if (check > -1)
                        {
                            var indexF = stringAtrr.IndexOf("cursor");
                            var newAtrr = stringAtrr.Remove(indexF, stringAtrr.Length  - indexF);
                            node.Attributes.Remove("style");
                            node.Attributes.Add("style",newAtrr );
                        }
             
                    } 
                   
                }
            }

            return htmlDoc.DocumentNode.OuterHtml;
    }

    // public static HtmlDocument CallBackStringTextHtml(HtmlDocument html, string name)
    // {
    //     var htmlNodes = html.DocumentNode.SelectSingleNode(name);
    //     string rs = "";
    //     if (htmlNodes.ChildNodes.Count == 0)
    //     {
    //         return html;
    //     }
    //     else
    //     {
    //         foreach (var node in htmlNodes.ChildNodes)
    //         {
		  //
    //             var attr = node.GetAttributes("style");
    //      
    //             var listAttr = attr.ToList();
    //             foreach (var atrbu in  listAttr)
    //             {
    //                 if (atrbu == null)
    //                 {
    //                     break;
    //                 }
    //                 var stringAtrr = atrbu.Value;
    //                 var check = stringAtrr.IndexOf("base64");
    //                 if (check > -1)
    //                 {
    //                     var indexF = stringAtrr.IndexOf("cursor");
    //                     var newAtrr = stringAtrr.Remove(indexF, stringAtrr.Length  - indexF);
    //                     node.Attributes.Remove("style");
    //                     node.Attributes.Add("style",newAtrr );
    //                 }
    //          
    //             }
    //             var kk = 
    //             rs += CallBackStringTextHtml(node.OuterHtml, node.Name);
    //         }
    //         return html;
    //     }
    // }
}