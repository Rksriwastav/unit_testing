using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiForTesting.Models
{
    public class loginModel
    {
        public int UserId { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
    }
}