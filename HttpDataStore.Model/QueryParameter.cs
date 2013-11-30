
using System;
namespace HttpDataStore.Client
{
    public enum QueryParameterOperator
    {
        Equals
    }

    public enum ChainOperation
    {
        And,
        Or
    }

    public class QueryParameter
    {
        public string Name { get; set; }
        public QueryParameterOperator Operator { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}={1}:{2}", this.Name, this.Operator, this.Value);
        }

        public static QueryParameter Parse(string name, string queryCondition)
        {
            string[] parts = queryCondition.Split(new char[] { ':' }, 2);

            if (parts.Length != 2)
            {
                throw new FormatException(string.Format("The query condition '{0}' has an invalid format. It should be '<Operator>:<Value>'"));
            }

            return new QueryParameter
            {
                Name = name,
                Operator = (QueryParameterOperator)Enum.Parse(typeof(QueryParameterOperator), parts[0], true),
                Value = ParseValue(parts[1])
            };
        }

        private static object ParseValue(string value)
        {
            int asInt;
            decimal asDecimal;
            DateTime asDateTime;
            bool asBool;

            if (bool.TryParse(value, out asBool)) return asBool;
            if (int.TryParse(value, out asInt)) return asInt;
            if (decimal.TryParse(value, out asDecimal)) return asDecimal;
            if (DateTime.TryParse(value, out asDateTime)) return asDateTime;
            return value;
        }
    }
}
