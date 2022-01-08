﻿using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Commands.SagaCommands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.Commands.Handlers;

public class SetOrderStateCommandHandler :
    ICommandHandler<Order, SetOrderStatePending>
{
    private readonly IOrderRepository _repository;
    private readonly ICommandResultProcessor<Order> _commandResultProcessor;
    private readonly ILogger<SetOrderStateCommandHandler> _logger;

    public SetOrderStateCommandHandler(
        IOrderRepository repository,
        ICommandResultProcessor<Order> commandResultProcessor,
        ILogger<SetOrderStateCommandHandler> logger)
    {
        _repository = repository;
        _commandResultProcessor = commandResultProcessor;
        _logger = logger;
    }

    public async Task<CommandResult<Order>> Handle(SetOrderStatePending command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(SetOrderStatePending));
    
        var order = await _repository.GetOrderAsync(command.EntityId);
        if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound);

        try
        {
            var updatedOrder = await _repository.UpdateOrderStateAsync(order, OrderState.Pending);
            if (updatedOrder == null)
                return new CommandResult<Order>(CommandOutcome.NotFound);
            await _commandResultProcessor.ProcessCommandResultAsync(updatedOrder, false);
            return new CommandResult<Order>(CommandOutcome.Accepted, updatedOrder);
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return new CommandResult<Order>(CommandOutcome.Conflict);
        }
    }
}