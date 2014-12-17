using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using HttpDataStore.Model;

namespace HttpDataStore.Hubs
{
    public class EntityHub : Hub
    {
        public void AnnounceEntityChange(Entity<object> entity)
        {
            Clients.Others.entityChanged(entity);
        }

        public void AnnounceEntityCreated(Entity<object> entity)
        {
            Clients.Others.entityCreated(entity);
        }
    }
}