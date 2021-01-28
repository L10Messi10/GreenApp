using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_Order_Details
    {
        public string id { get; set; }
        public string order_id { get; set; }
        public string prod_id { get; set; }
        public string qty { get; set; }
        public string sell_price { get; set; }
        public string sub_total { get; set; }
        public string discount { get; set; }

        public static async Task Insert(TBL_Order_Details orderdetails)
        {
            await MobileService.GetTable<TBL_Order_Details>().InsertAsync(orderdetails);
        }
        public static async Task Update(TBL_Order_Details orderdetails)
        {
            await MobileService.GetTable<TBL_Order_Details>().UpdateAsync(orderdetails);
        }
        public static async Task Delete(TBL_Order_Details orderdetails)
        {
            await MobileService.GetTable<TBL_Order_Details>().DeleteAsync(orderdetails);
        }
        public static async Task<List<TBL_Order_Details>> Read()
        {
            var orderDetails = await MobileService.GetTable<TBL_Order_Details>().ToListAsync();
            return orderDetails;
        }
    }
}
