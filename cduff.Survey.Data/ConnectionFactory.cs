//-----------------------------------------------------------------------
// <copyright file=”ConnectionFactory.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;

    public class ConnectionFactory : IConnectionFactory
    {
        const string CONNECTION_NAME = "DefaultConnection:ConnectionString";
        readonly string connectionString;
        readonly SqlClientFactory provider;

        public ConnectionFactory(IConfiguration config)
        {
            connectionString = config[CONNECTION_NAME];
            provider = SqlClientFactory.Instance;
        }

        public IDbConnection Create()
        {
            IDbConnection connection = provider.CreateConnection();
            if (connection == null)
            {
                throw new KeyNotFoundException(string.Format("Failed to create a connection using the connection string named '{0}' in Data/web.config.", nameof(connectionString)));
            }

            connection.ConnectionString = connectionString;
            return connection;
        }
    }
}
