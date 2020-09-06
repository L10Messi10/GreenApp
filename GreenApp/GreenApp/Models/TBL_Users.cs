using System;
using System.Collections.Generic;
using System.Text;

namespace GreenApp.Models
{
    public class TBL_Users
    {
        public string Id { get; set; }
        public string full_name { get; set; }
        public string mobile_num { get; set; }
        public string emailadd { get; set; }
        public string password { get; set; }
        public DateTime datereg { get; set; }
    }
}
