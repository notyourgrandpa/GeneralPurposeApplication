using GeneralPurposeApplication.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Services
{
    public interface ISeedService
    {
        Task<SeedResultDTO> Import();
        Task CreateDefaultUser();
    }
}
