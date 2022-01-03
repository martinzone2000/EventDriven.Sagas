﻿using Microsoft.AspNetCore.Mvc;
using SagaConfigService.DTO;
using SagaConfigService.Repositories;

namespace SagaConfigService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SagaConfigController : ControllerBase
    {
        private readonly ISagaConfigRepository _configRepository;

        public SagaConfigController(ISagaConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }

        // GET api/sagaconfig/d89ffb1e-7481-4111-a4dd-ac5123217293
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _configRepository.GetSagaConfigurationAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST api/sagaconfig
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SagaConfiguration value)
        {
            try
            {
                var result = await _configRepository.AddSagaConfigurationAsync(value);
                return CreatedAtAction(nameof(Get), new { id = value.Id }, result);
            }
            catch (ConcurrencyException e)
            {
                Console.WriteLine(e);
                return Conflict();
            }
        }

        // PUT api/sagaconfig
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] SagaConfiguration value)
        {
            try
            {
                var result = await _configRepository.UpdateSagaConfigurationAsync(value);
                return Ok(result);
            }
            catch (ConcurrencyException e)
            {
                Console.WriteLine(e);
                return Conflict();
            }
        }

        // DELETE api/<sagaconfig/d89ffb1e-7481-4111-a4dd-ac5123217293
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result =await _configRepository.RemoveSagaConfigurationAsync(id);
            return Ok(result);
        }
    }
}
