using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace HttpDataStore.Hubs
{
    public class EntityHub : Hub
    {
        public void Ping()
        {
            Clients.Caller.pong(DateTime.Now);
        }
    }
}