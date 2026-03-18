using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.Queries.Customers;
using GeneralPurposeApplication.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController: ControllerBase
    {
        private readonly SearchCustomersHandler _searchCustomerHandler;
        public CustomersController(SearchCustomersHandler searchCustomersHandler) 
        { 
            _searchCustomerHandler = searchCustomersHandler;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> Search(string term)
        {
            var customers = await _searchCustomerHandler.Handle(new SearchCustomersQuery(term));

            return Ok(customers);
        }
    }
}
