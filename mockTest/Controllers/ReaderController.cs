using Microsoft.AspNetCore.Mvc;
using mockTest.DTOs;
using mockTest.Exceptions;
using mockTest.Services;

namespace mockTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReaderController : ControllerBase
    {
        private readonly IDbService _dbService;

        public ReaderController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [Route("{id}/loans")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _dbService.GetReadersLoansAsync(id);
                return Ok(result);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [Route("{id}/loans")]
        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] int id, [FromBody] CreateLoansWithBooksDto dto)
        {
            if (!dto.Books.Any())
            {
                return BadRequest("At least one book is required");
            }

            try
            {
                await _dbService.CreateLoansWithBooksAsync(id, dto);
                return Created($"api/readers/{id}/loans", dto);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}