using GeneralPurposeApplication.Server.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> SearchCustomer(string term);
    }
}
