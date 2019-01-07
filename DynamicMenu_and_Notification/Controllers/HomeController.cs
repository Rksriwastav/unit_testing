using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using DynamicMenu_and_Notification.Models;
using Microsoft.Owin.Security;
using MySql.Data.MySqlClient;
using System.Web;
using System.Security.Claims;
using Microsoft.Owin.Security.Cookies;

namespace DynamicMenu_and_Notification.Controllers
{
    public class HomeController : Controller
    {
        #region Forgot Password Implement Using Gmail Link

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("Login", "Home");
        }
        [HttpGet]
        public ActionResult login()
        {
            Login login = new Login();
            return View("login", login);
        }
        [HttpPost]
        public ActionResult login(Login loginDetail)
        {
            if (loginDetail.UserName != null && loginDetail.Password != null)
            {
                Login login = GetLoginDetails(loginDetail);
                if (login.UserName != null && login.Password != null)
                {
                    List<SubMenu> subMenus = GetSubMenu();
                    List<Menus> menus = subMenus.Where(x => x.RoleId == login.UserRoleId).Select(x => new Menus
                    {
                        MainMenuId = x.MenuId,
                        MainMenuName = x.Menuname,
                        SubMenuId = x.Id,
                        SubMenuName = x.SubMenus,
                        ControllerName = x.Controller,
                        ActionName = x.Action,
                        RoleId = x.RoleId

                    }).ToList();
                    Session["menu"] = menus;
                    Session["login"] = login;
                }
                else
                    return Json(new { status = false, Message = "Plz fill valid username and password" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, Message = "Plz fill user credential" }, JsonRequestBehavior.AllowGet);

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);

        }

        public List<SubMenu> GetSubMenu()
        {
            List<SubMenu> subMenus = new List<SubMenu>();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            MySqlCommand mySqlCommand = new MySqlCommand("proc_menu_submenurecord", mySqlConnection);
            mySqlCommand.CommandType = CommandType.StoredProcedure;
            using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand))
            {
                using (DataTable dataTable = new DataTable())
                {
                    mySqlDataAdapter.Fill(dataTable);
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        SubMenu submenu = new SubMenu();
                        submenu.Id = Convert.ToInt32(dr["Id"]);
                        submenu.SubMenus = dr["SubMenu"].ToString();
                        submenu.Controller = dr["Controller"].ToString();
                        submenu.Action = dr["Action"].ToString();
                        submenu.MainMenuId = Convert.ToInt32(dr["MainMenuId"]);
                        submenu.RoleId = Convert.ToInt32(dr["RoleId"]);
                        submenu.MenuId = Convert.ToInt32(dr["MainMenuId"]);
                        submenu.Menuname = dr["MainMenu"].ToString();
                        subMenus.Add(submenu);
                    }
                }
            }
            return subMenus;

        }

        private Login GetLoginDetails(Login login)
        {
            Login loginDetail = new Login();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            MySqlCommand mySqlCommand = new MySqlCommand("SELECT * FROM dynamic_menu.tbl_login where UserName='" + login.UserName + "'  and Password='" + login.Password + "'", mySqlConnection);
            using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand))
            {
                using (DataTable dataTable = new DataTable())
                {
                    mySqlDataAdapter.Fill(dataTable);
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        loginDetail.UserId = Convert.ToInt32(dr["Id"]);
                        loginDetail.UserName = dr["UserName"].ToString();
                        loginDetail.Password = dr["Password"].ToString();
                        loginDetail.UserRoleId = Convert.ToInt32(dr["RoleId"]);

                    }
                }
            }
            return loginDetail;
        }

        public ActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        public ActionResult ForgotPasswordDetail(string userName)
        {
            Login loginCredential = new Login();
            string message = string.Empty;
            List<Login> login = GetLoginDetail();
            var loginDetail = login.Where(x => x.UserName == userName).FirstOrDefault();
            if (loginDetail != null)
            {
                string resetCode = Guid.NewGuid().ToString();
                SendVerificationLinkEmail(loginDetail.UserName, resetCode, "ResetPassword");
                loginDetail.ResetPasswordCode = resetCode;
                //Save Data Here
                loginCredential.ResetPasswordCode = loginDetail.ResetPasswordCode;
                loginCredential.UserId = loginDetail.UserId;
                UpdateLoginDetail(loginCredential);
                return Json(new { Status = true, message = "Reset password link has been sent to your emailId" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, message = "UserName not found" }, JsonRequestBehavior.AllowGet);
            }

        }

        private void UpdateLoginDetail(Login loginCredential)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            string userNameExistQuery = string.Format(@"update tbl_login set ResetPasswordCode='" + loginCredential.ResetPasswordCode + "' where Id='{0}'", loginCredential.UserId);
            using (mySqlConnection)
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCommand = new MySqlCommand(userNameExistQuery, mySqlConnection);
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
            }

        }

        private List<Login> GetLoginDetail()
        {
            List<Login> logins = new List<Login>();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            MySqlCommand mySqlCommand = new MySqlCommand("SELECT * FROM dynamic_menu.tbl_login ", mySqlConnection);
            using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand))
            {
                mySqlConnection.Open();
                using (DataTable dataTable = new DataTable())
                {
                    mySqlDataAdapter.Fill(dataTable);
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        Login loginDetail = new Login();
                        loginDetail.UserId = Convert.ToInt32(dr["Id"]);
                        loginDetail.UserName = dr["UserName"].ToString();
                        loginDetail.ResetPasswordCode = dr["ResetPasswordCode"].ToString();
                        logins.Add(loginDetail);
                    }
                }
            }
            return logins;
        }

        [NonAction]
        public bool IsEmailExist(string userName)
        {
            Login loginDetail = new Login();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            string userNameExistQuery = string.Format(@"select * from tbl_login where UserName='{0}'", userName);
            using (mySqlConnection)
            {
                MySqlCommand command = new MySqlCommand(userNameExistQuery, mySqlConnection);
                mySqlConnection.Open();
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (Convert.ToInt64(reader["is-exists"].ToString()) > 0)
                        {
                            mySqlConnection.Close();
                            return true;
                        }
                    }
                }
                mySqlConnection.Close();
            }
            return false;
        }


        [NonAction]
        public void SendVerificationLinkEmail(string emailId, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Home/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("kumarritesh480@gmail.com");
            var toEmail = new MailAddress(emailId);
            var fromEmailPassword = "8076280923";
            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "your account successfully created!";
                body = "<br/><br/> Your Account Id" + "Successfull Created .Plz click below to verify your account" +
                    "<br/><br/><a href='" + link + "'>" + link + "</a>";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/><br/>Plz Click on link to reset your password"
                    + "<br/><br/><a href=" + link + ">Reset Password Link</a>";
            }
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }


        public ActionResult ResetPassword(string id)
        {
            List<Login> login = GetLoginDetail();
            var loginDetail = login.Where(x => x.ResetPasswordCode == id).FirstOrDefault();
            if (loginDetail != null)
            {
                ResetPasswordModel resetPasswordModel = new ResetPasswordModel();
                resetPasswordModel.ResetCode = id;
                return View(resetPasswordModel);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (model != null)
            {
                Login loginCredential = new Login();
                List<Login> login = GetLoginDetail();
                var loginDetail = login.Where(x => x.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                if (loginDetail != null)
                {
                    loginDetail.Password = System.Web.Helpers.Crypto.Hash(model.NewPassword);
                    loginDetail.ResetPasswordCode = model.ResetCode;
                    loginCredential.Password = model.NewPassword;
                    loginCredential.ResetPasswordCode = loginDetail.ResetPasswordCode;
                    loginCredential.UserId = loginDetail.UserId;
                    UpdatePassword(loginCredential);
                    message = "New password updated successfully";
                }
            }
            else
            {
                message = "Something Invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }

        private void UpdatePassword(Login loginCredential)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            string userNameExistQuery = string.Format(@"update tbl_login set Password='" + loginCredential.Password + "' where Id='{0}'", loginCredential.UserId);
            using (mySqlConnection)
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCommand = new MySqlCommand(userNameExistQuery, mySqlConnection);
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
            }
        }

        #endregion

        public void SignIn(string ReturnUrl = "/", string type = "")
        {
            if (Request.IsAuthenticated)
            {
                if (type == "Google")
                {
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "Home/GoogleLoginCallback" }, "Google");
                }
            }
        }

        [AllowAnonymous]
        public ActionResult GoogleLoginCallback()
        {
            var claimsPrincipal = HttpContext.User.Identity as ClaimsIdentity;

            var loginInfo = GoogleLoginViewModel.GetLoginInfo(claimsPrincipal);
            if (loginInfo == null)
            {
                return RedirectToAction("Index");
            }

            List<Login> login = GetLoginDetail();
            var loginDetail = login.FirstOrDefault(x => x.UserName == loginInfo.emailaddress);
            if (loginDetail == null)
            {
                loginDetail = new Login
                {
                    UserName = loginInfo.emailaddress,

                };
                login.Add(loginDetail);
                //db.SaveChanges();
            }

            var ident = new ClaimsIdentity(
                    new[] { 
									// adding following 2 claim just for supporting default antiforgery provider
									new Claim(ClaimTypes.NameIdentifier, loginDetail.UserName),
                                    new Claim(ClaimTypes.Name,  loginDetail.UserName),
                                    new Claim(ClaimTypes.Email, loginDetail.UserName),
									// optionally you could add roles if any
									new Claim(ClaimTypes.Role, "User")
                    },
                    CookieAuthenticationDefaults.AuthenticationType);


            HttpContext.GetOwinContext().Authentication.SignIn(
                        new AuthenticationProperties { IsPersistent = true }, ident);
            return Redirect(Url.Content("~/"));

        }

    }
}