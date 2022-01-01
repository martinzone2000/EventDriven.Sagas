﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Repositories;

namespace EventDriven.Sagas.Tests.Fakes;

public class FakeSagaConfigRepository : ISagaConfigRepository
{
    public Task<SagaConfiguration> GetSagaConfigurationAsync(Guid id)
    {
        var steps = new Dictionary<int, SagaStep>
            {
                {   1,
                    new SagaStep
                    {
                        Sequence = 1,
                        Action = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "SetStatePending",
                                Result = "Pending",
                                ExpectedResult = "Pending"
                            }
                        },
                        CompensatingAction = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "SetStateInitial",
                                Result = "Initial",
                                ExpectedResult = "Initial"
                            }
                        }
                    }
                },
                {   2,
                    new SagaStep
                    {
                        Sequence = 2,
                        Action = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "ReserveCredit",
                                Result = "Reserved",
                                ExpectedResult = "Reserved"
                            }
                        },
                        CompensatingAction = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "ReleaseCredit",
                                Result = "Available",
                                ExpectedResult = "Available"
                            }
                        }
                    }
                },
                {   3,
                    new SagaStep
                    {
                        Sequence = 3,
                        Action = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "ReserveInventory",
                                Result = "Reserved",
                                ExpectedResult = "Reserved"
                            }
                        },
                        CompensatingAction = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "ReleaseInventory",
                                Result = "Available",
                                ExpectedResult = "Available"
                            }
                        }
                    }
                },
                {   4,
                    new SagaStep
                    {
                        Sequence = 4,
                        Action = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "SetStateCreated",
                                Result = "Created",
                                ExpectedResult = "Created"
                            }
                        },
                        CompensatingAction = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "SetStateInitial",
                                Result = "Initial",
                                ExpectedResult = "Initial"
                            }
                        }
                    }
                },
            };
        var config = new SagaConfiguration();
        config.Steps = steps;
        return Task.FromResult(config);
    }

    public Task<SagaConfiguration> AddSagaConfigurationAsync(SagaConfiguration entity)
    {
        throw new NotImplementedException();
    }

    public Task<SagaConfiguration> UpdateSagaConfigurationAsync(SagaConfiguration entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> RemoveSagaConfigurationAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}