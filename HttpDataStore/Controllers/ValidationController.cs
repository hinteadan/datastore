using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HttpDataStore.Infrastructure;
using HttpDataStore.Model;
using HttpDataStore.StorageEngine;

namespace HttpDataStore.Controllers
{
    public class ValidationController : BaseController
    {
        public HttpResponseMessage Put(Entity<object> data, string storeName, string clientId)
        {
            ValidatableEntity<object> validation = null;
            try
            {
                validation = ValidatableEntity<object>.FromEntity(data, clientId, storeName);
                ValidationStore.Save(validation.ToEntity());
            }
            catch (InvalidOperationException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, ex);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, validation.ValidationToken);
        }

        public HttpResponseMessage Post(string clientId, string validationToken)
        {
            var validation = ValidationStore.Query(m => m["ClientId"].ToString() == clientId && m["ValidationToken"].ToString() == validationToken).Single() as dynamic;
            var validEntity = validation.Data.Payload;
            Store.On(validation.Data.StoreName).Save(validEntity);
            ValidationStore.Delete(validation.Id);

            return Request.CreateResponse(HttpStatusCode.OK, validEntity as Entity<object>);
        }

        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
