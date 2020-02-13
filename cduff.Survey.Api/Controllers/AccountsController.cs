using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Hosting;

namespace cduff.Survey.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Security;
    using Business;

    /// <summary>
    /// Logs in a user, creates an encrypted user cookie, and redirects
    /// the user to the appropriate page, given a username cookie.
    /// </summary>
    [AllowAnonymous]
    public class AccountsController : Controller
    {
        readonly IWebHostEnvironment env;
        readonly IConfiguration config;
        readonly RepManager repManager;

        public AccountsController(IWebHostEnvironment env, IConfiguration config,
            RepManager repManager)
        {
            this.env = env;
            this.config = config;
            this.repManager = repManager;
        }

        // GET: Account
        public IActionResult Index(string returnUrl = null)
        {
            SurveySecurityProfile securityProfile = LogIn().Result;

            if (securityProfile == null)
            {
                return Redirect("~/Home/Unauthorized/");
            }

            return Redirect(returnUrl);
        }

        async Task<SurveySecurityProfile> LogIn()
        {
            SurveySecurityProfile securityProfile = new SurveySecurityProfile(UserName, config);

            if (securityProfile.Role == "NotAuthorized")
            {
                return null;
            }

            string Issuer = Environment.GetEnvironmentVariable("Domain");

            int repId = repManager.Find(x => x.Username == securityProfile.UserName).SingleOrDefault().RepId;

            var claims = new List<Claim> {
                        new Claim("Username", securityProfile.UserName, ClaimValueTypes.String, Issuer),
                        new Claim(ClaimTypes.Role, securityProfile.Role, ClaimValueTypes.String, Issuer),
                        new Claim("RepId", Convert.ToString(repId), ClaimValueTypes.Integer),
                        new Claim("UserType", Convert.ToString(securityProfile.UserType), ClaimValueTypes.Integer)
                    };

            var userIdentity = new ClaimsIdentity(claims, "SuperSecureLogin");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            await HttpContext.Authentication.SignInAsync("SurveyAuthentication", userPrincipal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false,
                    AllowRefresh = false
                });

            return securityProfile;
        }

        string UserName
        {
            get
            {
                string rtnValue = string.Empty;
                try
                {
                    if (Request.Cookies["UserName"] != null)
                        rtnValue = Request.Cookies["UserName"];
                }
                catch
                {
                    rtnValue = string.Empty;
                }

                //remove if causing security issues
                if (env.IsDevelopment())
                    rtnValue = "codduff";

                return rtnValue;
            }
        }
    }
}
