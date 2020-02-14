//-----------------------------------------------------------------------
// <copyright file=”ConnectionFactory.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data
{
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    public class ConnectionFactory : IConnectionFactory
    {
        private const string ConnectionName = "DefaultConnection:ConnectionString";
        private readonly string connectionString;
        private readonly SqlClientFactory provider;

        public ConnectionFactory(IConfiguration config)
        {
            connectionString = config[ConnectionName];
            provider = SqlClientFactory.Instance;
        }

        public IDbConnection Create()
        {
            IDbConnection connection = provider.CreateConnection();
            if (connection == null)
            {
                throw new KeyNotFoundException(
                    $"Failed to create a connection using the connection string named '{nameof(connectionString)}' in Data/web.config.");
            }

            connection.ConnectionString = connectionString;
            return connection;
        }
    }
}
