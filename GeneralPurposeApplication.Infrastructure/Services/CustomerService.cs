using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.Services;
using GeneralPurposeApplication.Domain.Abstractions;
using GeneralPurposeApplication.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<CustomerDTO>> SearchCustomer(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<CustomerDTO>(); // empty list if no search term

            return await _customerRepository.SearchCustomer(term);
        }
    }
}
