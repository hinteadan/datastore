
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using HttpDataStore.Client;
using HttpDataStore.Model;
using HttpDataStore.StorageEngine;
using HttpDataStore.Store;

namespace HttpDataStore
{
    public class QueryDataStore
    {
        private readonly IStoreData<object> dataStore;
        private const string noValidKeys = "NoValidKeys";

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

        public KeyValuePair<Guid, Dictionary<string, object>>[] QueryMeta(NameValueCollection queryParams)
        {
            return dataStore.QueryMeta(ConvertQueryParamsToMetaPredicate(queryParams)).ToArray();
        }

        private Func<Dictionary<string, object>, bool> ConvertQueryParamsToMetaPredicate(NameValueCollection queryParams)
        {
            ChainOperation chainWith = (ChainOperation)Enum.Parse(typeof(ChainOperation), queryParams["chainWith"], true);
            switch (chainWith)
            {
                case ChainOperation.And:
                    return meta => queryParams.AllKeys.Where(k => meta.ContainsKey(k) && k != "chainWith" && !string.IsNullOrWhiteSpace(k))
                        .DefaultIfEmpty(noValidKeys)
                        .All(k => k == noValidKeys ? false : CheckQueryCondition(meta[k], QueryParameter.Parse(k, queryParams[k])));
                case ChainOperation.Or:
                    return meta => queryParams.AllKeys.Where(k => meta.ContainsKey(k) && k != "chainWith" && !string.IsNullOrWhiteSpace(k))
                        .Any(k => CheckQueryCondition(meta[k], QueryParameter.Parse(k, queryParams[k])));
                default:
                    return meta => true;
            }
        }

        private bool CheckQueryCondition(object metaValue, QueryParameter queryParameter)
        {
            bool result;

            switch (queryParameter.Operator)
            {
                case QueryParameterOperator.Equals:
                    result = metaValue.IsEqualTo(queryParameter.Value);
                    break;
                case QueryParameterOperator.HigherThan:
                    result = metaValue.IsHigherThan(queryParameter.Value);
                    break;
                case QueryParameterOperator.HigherThanOrEqual:
                    result = metaValue.IsHigherThanOrEqual(queryParameter.Value);
                    break;
                case QueryParameterOperator.LowerThan:
                    result = metaValue.IsLowerThan(queryParameter.Value);
                    break;
                case QueryParameterOperator.LowerThanOrEqual:
                    result = metaValue.IsLowerThanOrEqual(queryParameter.Value);
                    break;
                case QueryParameterOperator.Contains:
                    result = metaValue.IsContaining(queryParameter.Value);
                    break;
                case QueryParameterOperator.BeginsWith:
                    result = metaValue.IsStartingWith(queryParameter.Value);
                    break;
                case QueryParameterOperator.EndsWith:
                    result = metaValue.IsEndingWith(queryParameter.Value);
                    break;
                default:
                    throw new NotImplementedException(string.Format("Operator <{0}> is not implemented", queryParameter.Operator));
            }
            return !queryParameter.IsNegated ? result : !result;

        }
    }
}