using System;
using System.Collections.Generic;

namespace CMS.Areas.Admin.Const
{
    public class DashBoardConst
    {
        public static int WareHouse = 5;
        public Dictionary<int, string> ListAreas = new Dictionary<int, string>()
        {
            {1 , "Miền Bắc"},
            {2 , "Miền Trung"},
            {3 , "Miền Nam"},
            {4 , "HCM"},
            {WareHouse , "Kho"},
        };
    }

}