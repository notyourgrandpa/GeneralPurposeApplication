using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Services
{
    public interface IProductService
    {
        Task UpdateStockAsync(InventoryLog inventoryLog);
    }
}
