//-----------------------------------------------------------------------
// <copyright file=”Extension.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    public static class Extension
    {
        public static bool HasColumn(this IDataRecord record, string columnName)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (record.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasProperty<T>(this T record, string propName) where T : class
        {
            var props = record.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.Name.Equals(propName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static object ToValidRange(this DateTime date)
        {
            if (date > Convert.ToDateTime("1753-01-01") && date <= Convert.ToDateTime("9999-12-31"))
            { return date; }

            return DBNull.Value;
        }

        public static object ToDBNull<T>(this T? value) where T : struct
        {
            return (object)value ?? DBNull.Value;
        }

        public static object ToDBNull(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? DBNull.Value : (object)value;
        }

        public static object ToDBNull(this object value)
        {
            return value ?? DBNull.Value;
        }
    }
}
