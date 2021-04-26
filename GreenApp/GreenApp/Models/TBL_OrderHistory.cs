using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_OrderHistory
    {
        public string id { get; set; }
        public string users_id { get; set; }
        public string order_id { get; set; }
        public string tot_payable { get; set; }
        public string cash_rendered { get; set; }
        public string cash_change { get; set; }
        public string itms_qty { get; set; }
        public string cart_datetime { get; set; }
        public string order_status { get; set; }
        public string order_choice { get; set; }

        public static async Task Void(TBL_OrderHistory voidorder)
        {
            await MobileService.GetTable<TBL_OrderHistory>().DeleteAsync(voidorder);
        }
        public static async Task<List<TBL_OrderHistory>> Read()
        {
            var orders = await MobileService.GetTable<TBL_OrderHistory>().ToListAsync();
            return orders;
        }
    }
}
