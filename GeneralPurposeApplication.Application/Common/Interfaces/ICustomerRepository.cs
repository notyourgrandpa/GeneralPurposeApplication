using GeneralPurposeApplication.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Common.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<IEnumerable<CustomerDTO>> SearchCustomer(string term);
    }
}
