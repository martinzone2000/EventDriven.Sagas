﻿using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Sagas.CreateOrder;

public class CreateOrderSaga : Saga,
    ICommandResultProcessor<Order>
{
    private readonly ISagaCommandDispatcher _commandDispatcher;
    private readonly ICommandResultEvaluator<OrderState, OrderState> _orderStateEvaluator;

    public CreateOrderSaga(
        ISagaCommandDispatcher commandDispatcher,
        ICommandResultEvaluator<OrderState, OrderState> orderStateEvaluator)
    {
        _commandDispatcher = commandDispatcher;
        _orderStateEvaluator = orderStateEvaluator;
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
        => await ProcessCommandResultAsync(Steps[CurrentStep], compensating);

    private async Task ProcessCommandResultAsync(SagaStep step, bool compensating)
    {
        var commandSuccessful = await _orderStateEvaluator.EvaluateStepResultAsync(
            Steps[CurrentStep], compensating, CancellationToken);
        StateInfo = _orderStateEvaluator.SagaStateInfo;
        await TransitionSagaStateAsync(commandSuccessful);
    }
}