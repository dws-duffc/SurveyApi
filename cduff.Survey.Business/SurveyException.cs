//-----------------------------------------------------------------------
// <copyright file=”SurveyException.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Business
{
    using System;

    public class FailedOperationException : Exception
    {
        public object OperationValue { get; private set; }
        public FailedOperationException() { }
        public FailedOperationException(string message) : base(message) { }
        public FailedOperationException(string message, object value) : base(message)
        {
            OperationValue = value;
        }
        public FailedOperationException(string message, Exception inner) : base(message, inner) { }
    }
}
