using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApiForTesting.Models;
namespace WebApiForTesting.Controllers
{
    public class HomeController : ApiController
    {
        public IHttpActionResult login(loginModel loginModel)
        {

            bool successfulLogin = GetLogin(loginModel.UserName, loginModel.Password);
            if (successfulLogin)
                return Ok();

            else
                return Ok("UserName or Password Does Not Exist");
        }

        private bool GetLogin(string userName, string password)
        {

            loginModel loginDetail = new loginModel();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            mySqlConnection.Open();
            MySqlCommand mySqlCommand = new MySqlCommand("SELECT * FROM dynamic_menu.tbl_login where UserName='" + userName + "'  and Password='" + password + "'", mySqlConnection);

            MySqlDataReader dr = mySqlCommand.ExecuteReader();
            if (dr.Read())
            {
                mySqlConnection.Close();
                return true;
            }
            else
                return false;
        }

        public IHttpActionResult CreateCredential(Login loginModel)
        {
            if (loginModel != null)
            {
                SaveCredential(loginModel);
                return Content(HttpStatusCode.OK, loginModel);
            }
            return BadRequest();
        }

        private void SaveCredential(Login loginModel)
        {
            loginModel loginDetail = new loginModel();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            mySqlConnection.Open();
            MySqlCommand mySqlCommand = new MySqlCommand("proc_insert", mySqlConnection);
            mySqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            mySqlCommand.Parameters.AddWithValue("userId", loginModel.UserId);
            mySqlCommand.Parameters.AddWithValue("username", loginModel.UserName);
            mySqlCommand.Parameters.AddWithValue("password", loginModel.Password);
            mySqlCommand.Parameters.AddWithValue("roleid", loginModel.UserRoleId);
            mySqlCommand.Parameters.AddWithValue("IsEmailVerified", loginModel.IsEmailVerified);
            mySqlCommand.Parameters.AddWithValue("ActivationCode", loginModel.ActivationCode);
            mySqlCommand.Parameters.AddWithValue("ResetPasswordCode", loginModel.ResetPasswordCode);
            mySqlCommand.ExecuteNonQuery();
            mySqlConnection.Close();
        }


        public HttpResponseMessage GetUserById(int userId)
        {
            Login logins = GetUserByUserId(userId);
            if (logins == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            else
                return Request.CreateResponse(HttpStatusCode.OK, logins);
        }

        private Login GetUserByUserId(int userId)
        {
            DataTable datatable = new DataTable();
            Login login = new Login();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            mySqlConnection.Open();
            MySqlCommand mySqlCommand = new MySqlCommand("ProcGetUserById", mySqlConnection);
            mySqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            mySqlCommand.Parameters.AddWithValue("@userId", userId);

            using (MySqlDataAdapter da = new MySqlDataAdapter(mySqlCommand))
            {
                da.Fill(datatable);
                foreach (DataRow dr in datatable.Rows)
                {
                    login.UserId = Convert.ToInt32(dr["Id"]);
                    login.UserName = dr["UserName"].ToString();
                    login.Password = dr["Password"].ToString();
                    login.UserRoleId = Convert.ToInt32(dr["RoleId"]);
                    login.IsEmailVerified = Convert.ToBoolean(dr["IsEmailVerified"]);
                    login.ActivationCode = Convert.ToInt32(dr["ActivationCode"]);
                    login.ResetPasswordCode = dr["ResetPasswordCode"].ToString();

                }
            }
            return login;
        }
    }
}
