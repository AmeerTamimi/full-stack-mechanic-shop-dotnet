using GOATY.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GOATY.Infrastructure.RealTime
{
    internal class SignalRWorkOrderNotifier(IHubContext<WorkOrderHub> hub) : IWorkOrderNotifier
    {
        public async Task NotifyWorkOrdersChangedAsync(CancellationToken ct = default)
        {
            await hub.Clients.All.SendAsync("WorkOrderChanged", ct);
        }
    }
}
