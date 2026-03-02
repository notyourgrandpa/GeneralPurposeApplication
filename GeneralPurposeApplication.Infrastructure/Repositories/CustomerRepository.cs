using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IApplicationDbContext _context;
        public CustomerRepository(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CustomerDTO>> SearchCustomer(string term)
        {
            var customers = await _context.Customers
                .Where(c => c.Name.Contains(term))
                .OrderBy(c => c.Name)
                .Take(20)
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
