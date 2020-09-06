using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_MarketStatus
    {
        public string id { get; set; }
        public string status { get; set; }
        public static async Task<List<TBL_MarketStatus>> Read()
        {
            var status = await MobileService.GetTable<TBL_MarketStatus>().ToListAsync();
            return status;
        }
    }
}
