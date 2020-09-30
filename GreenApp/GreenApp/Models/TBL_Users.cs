using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_Users
    {
        public string Id { get; set; }
        public string full_name { get; set; }
        public string address { get; set; }  
        public string mobile_num { get; set; }
        public string emailadd { get; set; }
        public string password { get; set; }
        public DateTime datereg { get; set; }
        public string propic { get; set; }
        public string picstr { get; set; }

        public static async Task Update(TBL_Users userdetails)
        {
            await MobileService.GetTable<TBL_Users>().UpdateAsync(userdetails);
        }

        public static async Task Insert(TBL_Users userdetails)
        {
            await MobileService.GetTable<TBL_Users>().InsertAsync(userdetails);
        }
    }
}
