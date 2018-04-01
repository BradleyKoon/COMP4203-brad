using System;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace COMP4203.Web.Controllers.Extend
{
	public abstract class ApiControllerWithHub<THub> : ApiController
		where THub : IHub
	{
		Lazy<IHubContext> _hub = new Lazy<IHubContext>(
			() => GlobalHost.ConnectionManager.GetHubContext<THub>()
		);

		protected IHubContext Hub => _hub.Value;
	}
}