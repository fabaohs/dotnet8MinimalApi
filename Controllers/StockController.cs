using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Microsoft.AspNetCore.Mvc;
using minimalApi.Mappers;

namespace minimalApi.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _client;

        public StockController(ApplicationDbContext client)
        {
            _client = client;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var stocks = _client.Stocks.ToList().Select(stock => stock.ToStockDto());
                return Ok(stocks);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            try
            {
                var stock = _client.Stocks.Find(id);

                if (stock is null)
                {
                    return NotFound($"Stock with id {id} not found");
                }

                return Ok(stock.ToStockDto());
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

    }
}