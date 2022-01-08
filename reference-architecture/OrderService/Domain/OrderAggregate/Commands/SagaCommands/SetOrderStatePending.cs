﻿using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands.SagaCommands;

public record SetOrderStatePending(Guid EntityId = default) : SagaCommand<OrderState, OrderState>(EntityId);