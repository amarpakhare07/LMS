using LMS.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repository.Interfaces
{
    public interface IRegisterUserRepository
    {
        public Task<RegisterDto> RegisterUserAsync(RegisterDto registerDto);
    }
}
