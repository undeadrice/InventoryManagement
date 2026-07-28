using InventoryManagement.Application.Auth.Dtos;
using MediatR;

namespace InventoryManagement.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<TokenDto>;