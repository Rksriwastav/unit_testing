using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicMenu_and_Notification.Models
{
    public class SubMenu
    {
        public int Id { get; set; }
        public string SubMenus { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int MainMenuId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { set; get; }
        public string Menuname { set; get; }
    }
}