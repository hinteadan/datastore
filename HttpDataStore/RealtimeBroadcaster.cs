using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HttpDataStore.Hubs;
using HttpDataStore.Model;
using Microsoft.AspNet.SignalR;

namespace HttpDataStore
{
    public class RealtimeBroadcaster
    {
        private readonly IHubContext entityHubContext;

        public RealtimeBroadcaster()
        {
            entityHubContext = GlobalHost.ConnectionManager.GetHubContext<EntityHub>();
        }

        public void EntityChanged<T>(Entity<T> entity)
        {
            entityHubContext.Clients.All.entityChanged(entity);
        }

        public void EntityCreated<T>(Entity<T> entity)
        {
            entityHubContext.Clients.All.entityCreated(entity);
        }

        public void EntityRemoved<T>(Entity<T> entity)
        {
            entityHubContext.Clients.All.entityRemoved(entity);
        }
    }
}