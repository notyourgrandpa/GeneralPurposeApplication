using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server
{
    public class HealthCheckHub: Hub
    {
        public async Task ClientUpdate(string message) =>
            await Clients.All.SendAsync("ClientUpdate", message);
    }
}
