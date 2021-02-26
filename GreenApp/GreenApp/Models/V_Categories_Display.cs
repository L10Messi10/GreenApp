using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class V_Categories_Display
    {
        public string id { get; set; }
        public string category_name { get; set; }
        public string cat_desc { get; set; }
        public string catimg_uri { get; set; }
        public string tot_products { get; set; }
        public static async Task<List<V_Categories_Display>> Read()
        {
            var categories = await MobileService.GetTable<V_Categories_Display>().ToListAsync();
            return categories;
        }
    }
}
