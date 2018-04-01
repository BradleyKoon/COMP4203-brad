using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace COMP4203.Web.Controllers.Hubs
{
	[HubName("mainHub")]
	public class MainHub : Hub { }
}