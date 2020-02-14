//-----------------------------------------------------------------------
// <copyright file=”SurveyUnitOfWork.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data
{
    using System;
    using System.Data;

    public class SurveyUnitOfWork : IUnitOfWork
    {
        private readonly Action<SurveyUnitOfWork> rolledBack;
        private readonly Action<SurveyUnitOfWork> committed;
        private IDbTransaction transaction;

        public SurveyUnitOfWork(IDbTransaction transaction, Action<SurveyUnitOfWork> rolledBack, Action<SurveyUnitOfWork> committed)
        {
            this.transaction = transaction;
            this.rolledBack = rolledBack;
            this.committed = committed;
        }

        public IDbTransaction Transaction => transaction;

        public void Dispose()
        {
            if (transaction == null)
            {
                return;
            }

            transaction.Rollback();
            transaction.Dispose();
            rolledBack(this);
            transaction = null;
        }

        public void SaveChanges()
        {
            if (transaction == null)
            {
                throw new InvalidOperationException("May not call save changes twice.");
            }

            transaction.Commit();
            transaction.Dispose();
            committed(this);
            transaction = null;
        }
    }
}
