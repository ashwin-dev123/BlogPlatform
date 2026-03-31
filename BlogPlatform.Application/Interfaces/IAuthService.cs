using BlogPlatform.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPlatform.Application.Interfaces
{
    public  interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);

        Task<UserDto?> RegisterAsync(CreateUserDto dto);

        Task<AuthResponseDto?> GoogleLoginAsync(string token);

    }
}
