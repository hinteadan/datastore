using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using HttpDataStore.Model;
using Metrics;

namespace HttpDataStore.Hubs
{
    public class EntityHub : Hub
    {
        private static readonly Counter counterEntityChanged = Metric.Counter("Entity Changed Notification", Unit.Events, new MetricTags("entity", "change", "update", "updated"));
        private static readonly Counter counterEntityCreated = Metric.Counter("Entity Created Notification", Unit.Events, new MetricTags("entity", "create", "created", "add", "added"));
        private static readonly Counter counterEntityRemoved = Metric.Counter("Entity Removed Notification", Unit.Events, new MetricTags("entity", "remove", "delete", "deleted", "removed"));

        public void AnnounceEntityChange(Entity<object> entity)
        {
            Clients.Others.entityChanged(entity);
            counterEntityChanged.Increment(entity.Id.ToString());
        }

        public void AnnounceEntityCreated(Entity<object> entity)
        {
            Clients.Others.entityCreated(entity);
            counterEntityCreated.Increment(entity.Id.ToString());
        }

        public void AnnounceEntityRemoved(Entity<object> entity)
        {
            Clients.Others.entityRemoved(entity);
            counterEntityRemoved.Increment(entity.Id.ToString());
        }
    }
}