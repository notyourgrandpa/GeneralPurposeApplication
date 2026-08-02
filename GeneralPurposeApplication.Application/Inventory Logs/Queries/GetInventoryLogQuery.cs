using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Domain.Inventory;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Inventory_Logs.Queries
{
    public class GetInventoryLogQuery : IRequest<InventoryLogDTO>
    {
        public int InventoryLogId { get; set;}
    }
}
