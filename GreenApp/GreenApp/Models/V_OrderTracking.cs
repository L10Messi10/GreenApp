using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class V_OrderTracking
    {
        public string id { get; set; }
        public string order_id { get; set; }
        public string cart_datetime { get; set; }
        public string tot_payable { get; set; }
        public string track_status { get; set; }
        public string track_desc { get; set; }
        public string track_time { get; set; }
        public string track_num { get; set; }

        public static async Task<List<V_OrderTracking>> Read()
        {
            var tracking = await MobileService.GetTable<V_OrderTracking>().ToListAsync();
            return tracking;
        }
    }
}
