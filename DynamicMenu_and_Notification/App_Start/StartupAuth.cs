using DotNetOpenAuth.GoogleOAuth2;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Membership.OpenAuth;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: OwinStartupAttribute(typeof(DynamicMenu_and_Notification.App_Start.Startup))]
namespace DynamicMenu_and_Notification.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

namespace DynamicMenu_and_Notification.App_Start
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/Home/login"),
                SlidingExpiration = true
            });
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "92297057626-260dm5jvt9789cqr7339bvth6hbr5nfr.apps.googleusercontent.com",
                ClientSecret = "AuLTO3CO0_BXQsRSmMC98ZYG",
               CallbackPath = new PathString("/GoogleLoginCallback")
            });
        }
    }

}