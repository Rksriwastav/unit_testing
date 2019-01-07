using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicMenu_and_Notification.Controllers;
using System.Web.Mvc;
using DynamicMenu_and_Notification.Models;
using System.Collections.Generic;

namespace LoginUnitTesting
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            HomeController homeController = new HomeController();

            var result = homeController.GetSubMenu() as List<SubMenu>;
            
            Assert.AreEqual(result,result);
        }
    }
}
