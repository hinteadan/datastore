
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using HttpDataStore.Client;
using HttpDataStore.Model;
using HttpDataStore.StorageEngine;
namespace HttpDataStore
{
    public class QueryDataStore
    {
        private readonly IStoreData<object> dataStore;

        public QueryDataStore(IStoreData<object> dataStore)
        {
            if (dataStore == null)
            {
                throw new ArgumentNullException("dataStore");
            }

            this.dataStore = dataStore;
        }

        public Entity<object>[] Query(NameValueCollection queryParams)
        {
            return dataStore.Query(ConvertQueryParamsToMetaPredicate(queryParams)).ToArray();
        }

        private Func<Dictionary<string, object>, bool> ConvertQueryParamsToMetaPredicate(NameValueCollection queryParams)
        {
            ChainOperation chainWith = (ChainOperation)Enum.Parse(typeof(ChainOperation), queryParams["chainWith"], true);
            switch (chainWith)
            {
                case ChainOperation.And:
                    return meta => queryParams.AllKeys.Where(k => k != "chainWith")
                        .All(k => CheckQueryCondition(meta[k], QueryParameter.Parse(k, queryParams[k])));
                case ChainOperation.Or:
                    return meta => queryParams.AllKeys.Where(k => k != "chainWith")
                        .Any(k => CheckQueryCondition(meta[k], QueryParameter.Parse(k, queryParams[k])));
                default:
                    return meta => true;
            }
        }

        private bool CheckQueryCondition(object metaValue, QueryParameter queryParameter)
        {
            if (metaValue == null)
            {
                return true;
            }

            switch (queryParameter.Operator)
            {
                case QueryParameterOperator.Equals:
                    return metaValue == queryParameter.Value;
                default:
                    throw new NotImplementedException(string.Format("Operator <{0}> is not implemented", queryParameter.Operator));
            }

        }
    }
}