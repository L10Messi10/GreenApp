using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static GreenApp.App;

namespace GreenApp.Models
{
    public class TBL_Feedback
    {
        public string id { get; set; }
        public string response { get; set; }
        public static async Task Insert(TBL_Feedback response)
        {
            await MobileService.GetTable<TBL_Feedback>().InsertAsync(response);
        }
    }
}
