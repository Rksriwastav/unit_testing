using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiForTesting.Models
{
    public class Login
    {
        public int UserId { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
        public int UserRoleId { set; get; }
        public String RoleName { get; set; }
        public string ResetPasswordCode { set; get; }
        public int ActivationCode { get; set; }
        public bool IsEmailVerified { set; get; }
    }
}