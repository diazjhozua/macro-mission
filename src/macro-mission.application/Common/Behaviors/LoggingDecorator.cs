using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;
using Microsoft.Extensions.Logging;

namespace MacroMission.Application.Common.Behaviors;

public static class LoggingDecorator
{
    public sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string name = typeof(TCommand).Name;
            logger.LogInformation("Processing command {Name}", name);

            Result<TResponse> result = await inner.Handle(command, cancellationToken);

            if (result.IsFailure)
                logger.LogWarning("Command {Name} failed: [{Code}] {Description}",
                    name, result.Error.Code, result.Error.Description);

            return result;
        }
    }

    public sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> inner,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string name = typeof(TCommand).Name;
            logger.LogInformation("Processing command {Name}", name);

            Result result = await inner.Handle(command, cancellationToken);

            if (result.IsFailure)
                logger.LogWarning("Command {Name} failed: [{Code}] {Description}",
                    name, result.Error.Code, result.Error.Description);

            return result;
        }
    }

    public sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> inner,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            string name = typeof(TQuery).Name;
            logger.LogInformation("Processing query {Name}", name);

            Result<TResponse> result = await inner.Handle(query, cancellationToken);

            if (result.IsFailure)
                logger.LogWarning("Query {Name} failed: [{Code}] {Description}",
                    name, result.Error.Code, result.Error.Description);

            return result;
        }
    }
}
