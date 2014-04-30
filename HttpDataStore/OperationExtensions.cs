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
            return Operate(valueA, valueB,
                (a, b) => a == b,
                (a, b) => a == b,
                (a, b) => a == b
                );
        }

        internal static bool IsHigherThan(this object valueA, object valueB)
        {
            return Operate(valueA, valueB,
                (a, b) => a > b,
                (a, b) => a > b,
                (a, b) => string.CompareOrdinal(a, b) > 0
                );
        }

        internal static bool IsHigherThanOrEqual(this object valueA, object valueB)
        {
            return Operate(valueA, valueB,
                (a, b) => a > b,
                (a, b) => a > b,
                (a, b) => string.CompareOrdinal(a, b) >= 0
                );
        }

        internal static bool IsLowerThan(this object valueA, object valueB)
        {
            return Operate(valueA, valueB,
                (a, b) => a < b,
                (a, b) => a < b,
                (a, b) => string.CompareOrdinal(a, b) < 0
                );
        }

        internal static bool IsLowerThanOrEqual(this object valueA, object valueB)
        {
            return Operate(valueA, valueB,
                (a, b) => a < b,
                (a, b) => a < b,
                (a, b) => string.CompareOrdinal(a, b) <= 0
                );
        }

        internal static bool IsContaining(this object valueA, object valueB)
        {
            return Operate(valueA, valueB,
                (a, b) => a.ToString().Contains(b.ToString()),
                (a, b) => { throw new InvalidOperationException("This operation cannot be performed on DateTimes"); },
                (a, b) => a.Contains(b)
                );
        }

        internal static bool IsStartingWith(this object valueA, object valueB)
        {
            return Operate(valueA, valueB,
                (a, b) => a.ToString().StartsWith(b.ToString()),
                (a, b) => { throw new InvalidOperationException("This operation cannot be performed on DateTimes"); },
                (a, b) => a.StartsWith(b)
                );
        }

        internal static bool IsEndingWith(this object valueA, object valueB)
        {
            return Operate(valueA, valueB,
                (a, b) => a.ToString().EndsWith(b.ToString()),
                (a, b) => { throw new InvalidOperationException("This operation cannot be performed on DateTimes"); },
                (a, b) => a.EndsWith(b)
                );
        }

        private static bool IsNumber(object value)
        {
            return value is sbyte
                || value is byte
                || value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong
                || value is float
                || value is double
                || value is decimal;
        }

        private static bool Operate(object a, object b,
            Func<decimal, decimal, bool> num,
            Func<DateTime, DateTime, bool> dtime,
            Func<string, string, bool> str)
        {
            if (IsNumber(a)) return num((decimal)a, (decimal)b);
            if (a is DateTime) return dtime((DateTime)a, (DateTime)b);
            if (a is string) return str((string)a, (string)b);
            throw new NotSupportedException("Only these types are supported: numeric, DateTime and string");
        }
    }
}