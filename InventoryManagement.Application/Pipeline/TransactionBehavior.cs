using MediatR;

namespace InventoryManagement.Application.Pipeline
{
    public class TransactionBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is not ICommand<TResponse> && request is not ICommand)
            {
                return await next();
            }

            await unitOfWork.StartTransaction();

            try
            {
                var response = await next();
                await unitOfWork.CommitAsync();
                return response;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
