using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_Token
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string push_token { get; set; }

        public static async Task Insert(TBL_Token token)
        {
            await MobileService.GetTable<TBL_Token>().InsertAsync(token);
        }
        public static async Task Update(TBL_Token token)
        {
            await MobileService.GetTable<TBL_Token>().UpdateAsync(token);
        }
    }
}
