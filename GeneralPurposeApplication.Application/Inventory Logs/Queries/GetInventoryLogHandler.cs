using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Domain.Inventory;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Inventory_Logs.Queries
{
    public class GetInventoryLogHandler : IRequestHandler<GetInventoryLogQuery, InventoryLog>
    {
        private readonly IApplicationDbContext _context;

        public GetInventoryLogHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryLog> Handle(GetInventoryLogQuery request, CancellationToken cancellationToken)
        {
            var inventoryLog = await _context.InventoryLogs.FindAsync(request.InventoryLogId, cancellationToken);
            if (inventoryLog == null)
            {
                throw new KeyNotFoundException($"Inventory Log {request.InventoryLogId} not found.");
            }

            return inventoryLog;
        }
    }
}
