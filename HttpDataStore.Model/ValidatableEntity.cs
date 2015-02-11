using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpDataStore.Model
{
    public class ValidatableEntity<T>
    {
        private ValidatableEntity()
        {

        }

        public Entity<T> Payload { get; set; }
        public string ClientId { get; set; }
        public string StoreName { get; set; }
        public string ValidationToken { get; set; }

        public Entity<object> ToEntity()
        {
            return new Entity<object>(this, new Dictionary<string, object>
            {
                { "ClientId", this.ClientId},
                { "StoreName", this.StoreName},
                { "ValidationToken", this.ValidationToken}
            });
        }

        public static ValidatableEntity<T> FromEntity(Entity<T> entity, string clientId, string storeName)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "The entity payload cannot be null");
            }

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentException("The client id cannot be null or empty", "clientId");
            }

            return new ValidatableEntity<T>
            {
                Payload = entity,
                ClientId = clientId,
                StoreName = storeName,
                ValidationToken = Guid.NewGuid().ToString()
            };
        }
    }
}
