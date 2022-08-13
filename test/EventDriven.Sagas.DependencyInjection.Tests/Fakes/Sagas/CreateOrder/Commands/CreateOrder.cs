using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Domain;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder.Commands;

public record CreateOrder : SagaCommand<OrderState, OrderState>;
