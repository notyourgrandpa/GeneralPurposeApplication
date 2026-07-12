using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.Services;
using GeneralPurposeApplication.Domain.Abstractions;
using GeneralPurposeApplication.Domain.Inventory;
using GeneralPurposeApplication.Domain.Products;
using GeneralPurposeApplication.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Infrastructure.Services
{
    public class SalesTransactionService : ISalesTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryLogService _inventoryLogService;
        public SalesTransactionService(IUnitOfWork unitOfWork, IInventoryLogService inventoryLogService)
        {
            _unitOfWork = unitOfWork;
            _inventoryLogService = inventoryLogService;
        }
    }
}
