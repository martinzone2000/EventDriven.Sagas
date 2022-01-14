using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Factories;

/// <summary>
/// Factory for creating saga instances.
/// </summary>
/// <typeparam name="TSaga">Saga type.</typeparam>
public class SagaFactory<TSaga> : ISagaFactory<TSaga>
    where TSaga : Saga, ISagaCommandResultHandler
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="sagaCommandDispatcher">Saga command dispatcher.</param>
    /// <param name="sagaCommandResultEvaluator">Saga command result evaluator.</param>
    /// <param name="commandResultDispatchers">Command result dispatchers</param>
    public SagaFactory(
        ISagaCommandDispatcher sagaCommandDispatcher,
        ISagaCommandResultEvaluator sagaCommandResultEvaluator,
        IEnumerable<ISagaCommandResultDispatcher> commandResultDispatchers)
    {
        SagaCommandDispatcher = sagaCommandDispatcher;
        SagaCommandResultEvaluator = sagaCommandResultEvaluator;
        SagaCommandResultDispatchers = commandResultDispatchers;
    }

    /// <summary>
    /// Saga command dispatcher.
    /// </summary>
    public virtual ISagaCommandDispatcher SagaCommandDispatcher { get; }

    /// <summary>
    /// Command result evaluator.
    /// </summary>
    public virtual ISagaCommandResultEvaluator SagaCommandResultEvaluator { get; }

    /// <summary>
    /// Command result dispatchers.
    /// </summary>
    protected IEnumerable<ISagaCommandResultDispatcher> SagaCommandResultDispatchers { get; set; }

    /// <inheritdoc />
    public virtual TSaga CreateSaga()
    {
        var saga = (TSaga?)Activator.CreateInstance(
            typeof(TSaga), SagaCommandDispatcher, SagaCommandResultEvaluator);
        if (saga == null)
            throw new Exception($"Unable to create instance of {typeof(TSaga).Name}");
        foreach (var commandResultDispatcher in SagaCommandResultDispatchers
            .Where(d => d.SagaType == null || d.SagaType == typeof(TSaga)))
            commandResultDispatcher.SagaCommandResultHandler = saga;
        return saga;
    }
}