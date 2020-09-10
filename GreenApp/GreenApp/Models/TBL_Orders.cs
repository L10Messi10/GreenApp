using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_Orders
    {
        public string id { get; set; }
        public string users_id { get; set; }
        public string tot_payable { get; set; }
        public string cash_rendered { get; set; }
        public string cash_change { get; set; }
        public string order_date { get; set; }
        public string order_status { get; set; }
        public string stat { get; set; }

        public static async Task Insert(TBL_Orders order)
        {
            await MobileService.GetTable<TBL_Orders>().InsertAsync(order);
        }
        public static async Task Update(TBL_Orders orders)
        {
            await MobileService.GetTable<TBL_Orders>().UpdateAsync(orders);
        }
        public static async Task<List<TBL_Orders>> Read()
        {
            var orders = await MobileService.GetTable<TBL_Orders>().ToListAsync();
            return orders;
        }
    }
}
