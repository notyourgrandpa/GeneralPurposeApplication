using GeneralPurposeApplication.Server.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Path = System.IO.Path;
using GeneralPurposeApplication.Infrastructure.Persistence;
using GeneralPurposeApplication.Infrastructure.Identity;
using GeneralPurposeApplication.Application.Services;

namespace GeneralPurposeApplication.Server.Controllers
{
    //[Authorize(Roles = "Administrator")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ISeedService _seedService;

        public SeedController(ISeedService seedService)
        {
            _seedService = seedService;
        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            var seedResult = await _seedService.Import();

            return Ok(seedResult);
        }

        [HttpGet]
        public async Task<ActionResult> CreateDefaultUsers()
        {
            await _seedService.CreateDefaultUser();
            return Ok();
        }
    }
}
