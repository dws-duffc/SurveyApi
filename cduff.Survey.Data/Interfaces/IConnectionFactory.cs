//-----------------------------------------------------------------------
// <copyright file=”IConnectionFactory.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data
{
    using System.Data;

    public interface IConnectionFactory
    {
        IDbConnection Create();
    }
}