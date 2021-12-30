﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions;

namespace EventDriven.Sagas.Tests.Fakes;

public class FakeSaga : Saga,
    ICommandResultProcessor<Order>,
    ICommandResultProcessor<Customer>,
    ICommandResultProcessor<Inventory>
{
    private readonly ISagaCommandDispatcher _commandDispatcher;

    public FakeSaga(Dictionary<int, SagaStep> steps, ISagaCommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        Steps = steps;
    }

    public override async Task StartSagaAsync(CancellationToken cancellationToken = default)
    {
        // Set state and current step
        State = SagaState.Executing;
        CurrentStep = 1;

        // Dispatch current step command
        await ExecuteCurrentActionAsync();
    }

    protected override async Task ExecuteCurrentActionAsync()
    {
        var action = Steps[CurrentStep].Action;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        await _commandDispatcher.DispatchAsync(action.Command, false);
    }

    protected override async Task ExecuteCurrentCompensatingActionAsync()
    {
        var action = Steps[CurrentStep].CompensatingAction;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        await _commandDispatcher.DispatchAsync(action.Command, true);
    }

    public async Task ProcessCommandResultAsync(Order commandResult, bool compensating)
    {
        await ProcessCommandResultLocalAsync(commandResult, compensating);    
    }

    public async Task ProcessCommandResultAsync(Customer commandResult, bool compensating)
    {
        await ProcessCommandResultLocalAsync(commandResult, compensating);
    }

    public async Task ProcessCommandResultAsync(Inventory commandResult, bool compensating)
    {
        await ProcessCommandResultLocalAsync(commandResult, compensating);
    }

    private async Task ProcessCommandResultLocalAsync<TEntity>(TEntity commandResult, bool compensating)
    {
        // Get result
        string? result = null;
        if (commandResult is Order order) result = order.State;
        else if (commandResult is Customer customer) result = customer.Credit;
        else if (commandResult is Inventory inventory) result = inventory.Stock;

        // Evaluate result
        var action = compensating ? Steps[CurrentStep].CompensatingAction : Steps[CurrentStep].Action;
        var commandSuccessful = string.Compare(result,
            ((FakeCommand)action.Command).ExpectedResult, StringComparison.OrdinalIgnoreCase) == 0;

        // Check timeout
        action.Completed = DateTime.UtcNow;
        action.Duration = action.Completed - action.Started;
        var commandTimedOut = commandSuccessful && action.Timeout != null && action.Duration > action.Timeout;
        if (commandTimedOut) commandSuccessful = false;

        // Transition action state
        action.State = commandSuccessful ? ActionState.Succeeded : ActionState.Failed;
        if (!commandSuccessful && result != null)
        {
            action.StateInfo = !commandTimedOut
                ? $"Unexpected result: '{result}'."
                : $"Duration of '{action.Duration!.Value:c}' exceeded timeout of '{action.Timeout!.Value:c}'";
            var commandName = action.Command.Name ?? "No name";
            StateInfo = $"Step {CurrentStep} command '{commandName}' failed. {action.StateInfo}";
        }

        // Transition saga state
        await TransitionSagaStateAsync(commandSuccessful);
    }
}