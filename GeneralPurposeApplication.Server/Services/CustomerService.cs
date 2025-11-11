using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Data.Models;
using GeneralPurposeApplication.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CustomerDTO>> SearchCustomer(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<CustomerDTO>(); // empty list if no search term

            var customers = await _unitOfWork.Repository<Customer>().GetQueryable()
                .Where(c => c.Name.Contains(term))
                .OrderBy(c => c.Name)
                .Take(20) // limit results for performance
                .Select(c => new CustomerDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    ContactNumber = c.ContactNumber
                })
                .ToListAsync();

            return customers;
        }
    }
}
