using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Commands;

/// <summary>
/// Saga command handler.
/// </summary>
public interface ISagaCommandHandler
{
}

/// <summary>
/// Saga command handler.
/// </summary>
/// <typeparam name="TSagaCommand">Saga command type.</typeparam>
public interface ISagaCommandHandler<in TSagaCommand> : ISagaCommandHandler
    where TSagaCommand : class, ISagaCommand
{
    /// <summary>
    /// Handles a command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The command result.</returns>
    Task<CommandResult> HandleCommandAsync(TSagaCommand command);
}

/// <summary>
/// Saga command handler.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TSagaCommand">Saga command type.</typeparam>
public interface ISagaCommandHandler<TEntity, in TSagaCommand> : ISagaCommandHandler
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    /// <summary>
    /// Handles a command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The command result.</returns>
    Task<CommandResult<TEntity>> HandleCommandAsync(TSagaCommand command);
}
