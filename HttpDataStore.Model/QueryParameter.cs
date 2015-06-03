
using System;
using System.Globalization;
namespace HttpDataStore.Client
{
    public enum QueryParameterOperator
    {
        Equals,
        LowerThan,
        LowerThanOrEqual,
        HigherThan,
        HigherThanOrEqual,
        Contains,
        BeginsWith,
        EndsWith
    }

    public enum ChainOperation
    {
        None,
        And,
        Or
    }

    public class QueryParameter
    {
        public string Name { get; set; }
        public QueryParameterOperator Operator { get; set; }
        public bool IsNegated { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}={3}{1}:{2}", this.Name, this.Operator, this.Value, this.IsNegated ? "!" : string.Empty);
        }

        public static QueryParameter Parse(string name, string queryCondition)
        {
            string[] parts = queryCondition.Split(new char[] { ':' }, 2);

            if (parts.Length != 2)
            {
                throw new FormatException(string.Format("The query condition '{0}' has an invalid format. It should be '<Operator>:<Value>'"));
            }

            string op = parts[0];
            bool isNegated = false;

            if (parts[0][0] == '!')
            {
                isNegated = true;
                op = parts[0].Substring(1);
            }

            return new QueryParameter
            {
                Name = name,
                Operator = (QueryParameterOperator)Enum.Parse(typeof(QueryParameterOperator), op, true),
                IsNegated = isNegated,
                Value = ParseValue(parts[1])
            };
        }

        private static object ParseValue(string value)
        {
            int asInt;
            decimal asDecimal;
            DateTime asDateTime;
            bool asBool;

            if (value == "null") return null;
            if (bool.TryParse(value, out asBool)) return asBool;
            if (int.TryParse(value, out asInt)) return asInt;
            if (decimal.TryParse(value, out asDecimal)) return asDecimal;
            if (DateTime.TryParse(value, out asDateTime)) return asDateTime;
            return value;
        }
    }
}
