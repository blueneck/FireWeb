using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FireWeb.Models
{
    public class LoginModel
    {
        public int id { get; set; }
        public string userId { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string mail { get; set; }
        public string remark { get; set; }
       
    }
}