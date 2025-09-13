using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.DTOs;
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
        private ApplicationDbContext _context;
        public CustomersController(ApplicationDbContext context) 
        { 
            _context = context;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Ok(new List<CustomerDTO>()); // empty list if no search term

            var customers = await _context.Customers
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

            return Ok(customers);
        }
    }
}
