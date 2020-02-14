//-----------------------------------------------------------------------
// <copyright file=”SurveyContext.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;

    public class SurveyContext : IDisposable
    {
        private readonly IDbConnection connection;
        private readonly IConnectionFactory connectionFactory;
        private readonly ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();
        private readonly LinkedList<SurveyUnitOfWork> workList = new LinkedList<SurveyUnitOfWork>();

        public SurveyContext(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
            connection = connectionFactory.Create();
        }

        public IDbCommand CreateCommand()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            IDbCommand command = connection.CreateCommand();

            readWriteLock.EnterReadLock();
            if (workList.Count > 0)
            {
                command.Transaction = workList.First.Value.Transaction;
            }

            readWriteLock.ExitReadLock();

            return command;
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            IDbTransaction transaction = connection.BeginTransaction();
            var unitOfWork = new SurveyUnitOfWork(transaction, RemoveTransaction, RemoveTransaction);

            readWriteLock.EnterWriteLock();
            workList.AddLast(unitOfWork);
            readWriteLock.ExitWriteLock();

            return unitOfWork;
        }

        private void RemoveTransaction(SurveyUnitOfWork unitOfWork)
        {
            readWriteLock.EnterWriteLock();
            workList.Remove(unitOfWork);
            readWriteLock.ExitWriteLock();
        }

        #region Dispose

        private bool disposed;

        public void Dispose()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && connection != null)
            {
                if (disposing)
                {
                    connection.Dispose();
                }
            }

            disposed = true;
        }

        #endregion
    }
}
