﻿using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands.SagaCommands;

public record ReserveInventoryStock(Guid EntityId = default) : SagaCommand(EntityId);