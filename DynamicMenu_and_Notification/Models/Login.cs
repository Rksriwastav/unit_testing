using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicMenu_and_Notification.Models
{
    public class Login
    {
        public int UserId { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
        public int UserRoleId { set; get; }
        public String RoleName { get; set; }
        public string ResetPasswordCode { set; get; }

        public int Id { get; set; }
        public string Email { get; set; }
        public string GivenName { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Identifier { get; set; }
        public bool IsActive { get; set; }




    }
}