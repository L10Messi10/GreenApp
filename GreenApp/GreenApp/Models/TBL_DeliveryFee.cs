using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_DeliveryFee
    {
        public string id { get; set; }
        public float d_fee { get; set; }
        public static async Task<List<TBL_DeliveryFee>> Read()
        {
            var del_Fee = await MobileService.GetTable<TBL_DeliveryFee>().ToListAsync();
            return del_Fee;
        }
    }
}
