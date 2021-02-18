using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_Addresses
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string Label { get; set; }
        public string building_name { get; set; }
        public string Address { get; set; }
        public double add_lat { get; set; }
        public double add_long { get; set; }
        public string Notes { get; set; }

        public static async Task Insert(TBL_Addresses addresses)
        {
            await MobileService.GetTable<TBL_Addresses>().InsertAsync(addresses);
        }
        public static async Task Update(TBL_Addresses addresses)
        {
            await MobileService.GetTable<TBL_Addresses>().UpdateAsync(addresses);
        }

        public static async Task Remove(TBL_Addresses addresses)
        {
            await MobileService.GetTable<TBL_Addresses>().DeleteAsync(addresses);
        }
        public static async Task<List<TBL_Addresses>> Read()
        {
            var addresses = await MobileService.GetTable<TBL_Addresses>().ToListAsync();
            return addresses;
        }
    }
}
