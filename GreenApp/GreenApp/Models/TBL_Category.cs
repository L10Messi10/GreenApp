using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_Category
    {
        public string id { get; set; }
        public string category_name { get; set; }
        public string cat_desc { get; set; }
        public string catimg_uri { get; set; }


        public static async Task<List<TBL_Category>> Read()
        {
            var categories= await MobileService.GetTable<TBL_Category>().ToListAsync();
            return categories;
        }
    }
}
