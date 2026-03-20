using Asisya.Products.Application.Common;
using Asisya.Products.Application.DTOs;

namespace Asisya.Products.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
}
