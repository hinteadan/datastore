using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HttpDataStore
{
    internal static class OperationExtensions
    {
        internal static bool IsEqualTo(this object valueA, object valueB)
        {
            return Convert.ToDecimal(valueA) == Convert.ToDecimal(valueB);
        }

        internal static bool IsHigherThan(this object valueA, object valueB)
        {
            return Convert.ToDecimal(valueA) > Convert.ToDecimal(valueB);
        }

        internal static bool IsHigherThanOrEqual(this object valueA, object valueB)
        {
            return Convert.ToDecimal(valueA) >= Convert.ToDecimal(valueB);
        }
    }
}