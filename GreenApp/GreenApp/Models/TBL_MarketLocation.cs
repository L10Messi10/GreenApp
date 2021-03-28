using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_MarketLocation
    {
        public string id { get; set; }
        public double m_lat { get; set; }
        public double m_long { get; set; }
        public double limit_distance { get; set; }

        public static async Task<List<TBL_MarketLocation>> Read()
        {
            var marketLocations = await MobileService.GetTable<TBL_MarketLocation>().ToListAsync();
            return marketLocations;
        }
    }
}
