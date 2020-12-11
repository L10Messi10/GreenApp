using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Annotations;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_Products
    {
        //needs to insert color field 
        public string id { get; set; }
        public string prod_name { get; set; }
        public string prod_desc { get; set; }
        public string prod_price { get; set; }
        public string unit_desc { get; set; }
        public string prod_av { get; set; }
        public string category_name { get; set; }
        public string img_uri { get; set; }

        public static async Task<List<TBL_Products>> Read()
        {
            var products = await MobileService.GetTable<TBL_Products>().ToListAsync();
            return products;
        }
    }
}
