using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_OrderTracking
    {
        public string id { get; set; }
        public string order_id { get; set; }
        public string track_status { get; set; }
        public string track_desc { get; set; }
        public string track_time { get; set; }
        public string track_num { get; set; }

        public static async Task Insert(TBL_OrderTracking tracking)
        {
            await MobileService.GetTable<TBL_OrderTracking>().InsertAsync(tracking);
        }
        public static async Task<List<TBL_OrderTracking>> Read()
        {
            var tracking = await MobileService.GetTable<TBL_OrderTracking>().ToListAsync();
            return tracking;
        }
    }
}
