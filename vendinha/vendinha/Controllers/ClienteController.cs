using Microsoft.AspNetCore.Mvc;
using vendinha.Entities;
using vendinha.Repositories;
using Microsoft.Extensions.Logging;

namespace vendinha.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteRepository _clienteRepository;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(ClienteRepository clienteRepository, ILogger<ClienteController> logger)
        {
            _clienteRepository = clienteRepository;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Clientes>> GetAll()
        {
            try
            {
                var clientes = _clienteRepository.FindAll().ToList();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cliente list");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Clientes>> GetById(int id)
        {
            try
            {
                var cliente = await _clienteRepository.FindByID(id);
                if (cliente == null)
                {
                    _logger.LogWarning("Cliente not found for id {Id}", id);
                    return NotFound();
                }
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cliente details for id {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Clientes cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _clienteRepository.Add(cliente);
                return CreatedAtAction(nameof(GetById), new { id = cliente.clientId }, cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new cliente");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Clientes cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingCliente = await _clienteRepository.FindByID(id);
                if (existingCliente == null)
                {
                    return NotFound();
                }

                cliente.clientId = id;
                await _clienteRepository.Update(cliente);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cliente with id {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var cliente = await _clienteRepository.FindByID(id);
                if (cliente == null)
                {
                    return NotFound();
                }

                await _clienteRepository.Remove(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cliente with id {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
