//-----------------------------------------------------------------------
// <copyright file=”SurveySecurityProfile.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Api.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Builds a security profile after successful login.
    /// </summary>
    public class SurveySecurityProfile
    {
        #region Fields

        SurveySecurity.User user;
        List<SurveySecurity.Attribute> attributes;
        readonly IConfiguration config;

        #endregion

        public SurveySecurityProfile(string username, IConfiguration config)
        {
            this.config = config;

            if (Environment == "DEV")
            {
                user = new SurveySecurity.User
                {
                    UserName = "codduff",
                    UserType = 1
                };

                attributes = new List<SurveySecurity.Attribute>
                {
                    new SurveySecurity.Attribute {Name = "IsAdmin" }
                    //new SurveySecurity.Attribute {Name = "CanAccess" }
                };
            }
            else
            {
                createSecurityProfile(username);
            }
        }

        #region Properties

        public string Role
        {
            get
            {
                if (attributes.FirstOrDefault(a => a.Name.Equals("IsAdmin")) != null) { return "Admin"; }
                if (attributes.FirstOrDefault(a => a.Name.Equals("CanAccess")) != null) { return "User"; }
                return "NotAuthorized";
            }
        }

        public string UserName => user.UserName;

        public int UserType => user.UserType;

        string ApplicationName => config["ApplicationName"] ?? "SurveyApi";

        string Environment => config[".SurveySecurity.Environment.Short"] ?? "DEV";

        #endregion

        void createSecurityProfile(string username)
        {
            // connect to service
            SurveySecurity.SecurityInfoWCFServiceClient securityService = new SurveySecurity.SecurityInfoWCFServiceClient();

            // get user
            user = securityService.GetUserInfoAsync(username, Environment).Result;

            // get attributes
            attributes = new List<SurveySecurity.Attribute>(
                securityService.GetAttributeInfoAsync(ApplicationName, UserName, Environment).Result);

            // close service
            securityService.CloseAsync();
        }
    }
}
