//-----------------------------------------------------------------------
// <copyright file=”SurveyBusinessConfig.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Extensions.DependencyInjection
{
    using cduff.Survey.Data;
    using cduff.Survey.Business;

    public static class SurveyBusinessConfig
    {
        public static void AddSurveyConfiguration(this IServiceCollection services)
        {
            services.AddScoped<IConnectionFactory, ConnectionFactory>();
            services.AddScoped<SurveyContext>();
            services.AddTransient<AgentManager>();
            services.AddTransient<AnswerManager>();
            services.AddTransient<AssignmentManager>();
            services.AddTransient<RepManager>();
            services.AddTransient<AttemptLogManager>();
            services.AddTransient<ContactManager>();
            services.AddTransient<PeriodManager>();
            services.AddTransient<QuestionManager>();
            services.AddTransient<ResponseManager>();
        }
    }
}
