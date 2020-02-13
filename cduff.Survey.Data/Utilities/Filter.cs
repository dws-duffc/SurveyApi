//-----------------------------------------------------------------------
// <copyright file=”Filter.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data.Utilities
{
    public class Filter
    {
        public Operation Operation { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public string OpChar { get; set; }
    }
}
