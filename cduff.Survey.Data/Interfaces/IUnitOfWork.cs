//-----------------------------------------------------------------------
// <copyright file=”IUnitOfWork.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data
{
    using System;

    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
    }
}