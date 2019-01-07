using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicMenu_and_Notification.Models
{
    public class ResetPasswordModel
    {
        public string NewPassword { set; get; }
        public string ConfirmPassword { set; get; }
        public string ResetCode { set; get; }
    }
}