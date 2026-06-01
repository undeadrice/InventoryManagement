using MediatR;

namespace InventoryManagement.Application.Pipeline;

public interface ICommand : IRequest
{
}

public interface ICommand<TResponse> : IRequest<TResponse>
{
}

