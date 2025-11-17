using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Services
{
    public interface ISeedController
    {
        Task Import();
        Task CreateDefaultUser();
    }
}
