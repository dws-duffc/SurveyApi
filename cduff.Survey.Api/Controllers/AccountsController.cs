namespace cduff.Survey.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.Extensions.Hosting;
    using Security;
    using Business;
    using Model;

    /// <summary>
    /// Logs in a user, creates an encrypted user cookie, and redirects
    /// the user to the appropriate page, given a username cookie.
    /// </summary>
    [AllowAnonymous]
    public class AccountsController : Controller
    {
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration config;
        private readonly RepManager repManager;

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

            return Redirect(securityProfile == null ? "~/Home/Unauthorized/" : returnUrl);
        }

        private async Task<SurveySecurityProfile> LogIn()
        {
            var securityProfile = new SurveySecurityProfile(UserName, config);

            if (securityProfile.Role == "NotAuthorized")
            {
                return null;
            }

            string issuer = Environment.GetEnvironmentVariable("Domain");

            Rep rep = repManager.Find(x => x.Username == securityProfile.UserName).SingleOrDefault();

            var claims = new List<Claim> {
                        new Claim("Username", securityProfile.UserName, ClaimValueTypes.String, issuer),
                        new Claim(ClaimTypes.Role, securityProfile.Role, ClaimValueTypes.String, issuer),
                        new Claim("RepId", Convert.ToString(rep?.RepId ?? 0), ClaimValueTypes.Integer),
                        new Claim("UserType", Convert.ToString(securityProfile.UserType), ClaimValueTypes.Integer)
                    };

            var claimsIdentity = new ClaimsIdentity(claims, "SuperSecureLogin");

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false,
                    AllowRefresh = false
                });

            return securityProfile;
        }

        private string UserName
        {
            get
            {
                string rtnValue = string.Empty;
                try
                {
                    if (Request.Cookies["UserName"] != null)
                    {
                        rtnValue = Request.Cookies["UserName"];
                    }
                }
                catch
                {
                    rtnValue = string.Empty;
                }

                //remove if causing security issues
                if (env.IsDevelopment())
                {
                    rtnValue = "cduff";
                }

                return rtnValue;
            }
        }
    }
}
