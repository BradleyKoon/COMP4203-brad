using COMP4203.Web.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COMP4203.Web.Controllers.Hubs
{
    public class OutputHub : Hub
    {
        internal static void PrintToOutputPane(string tag, string message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<OutputHub>();
            context.Clients.All.broadcastOutputMessage(JsonConvert.SerializeObject(new OutputMessage
            {
                Id = Guid.NewGuid(),
                Tag = tag,
                Message = message
            }));
        }
    }
}