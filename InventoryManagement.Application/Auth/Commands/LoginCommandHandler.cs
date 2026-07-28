using InventoryManagement.Application.Auth.Dtos;
using InventoryManagement.Application.Auth.Services;
using MediatR;

namespace InventoryManagement.Application.Auth.Commands;

public class LoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, TokenDto>
{
    public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var token = await authService.Login(request.Email, request.Password);
        return new TokenDto(token, DateTime.Now.AddHours(1));
    }
}