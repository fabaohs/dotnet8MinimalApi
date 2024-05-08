using System;
using System.Collections.Generic;
using System.Linq;
using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimalApi.Dtos.Stock;
using minimalApi.Mappers;
using minimalApi.Models;
using minimalApi.Models.Responses;

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
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var stocks = await _client.Stocks.Include(stock => stock.Comments).ToListAsync();
                var stocksDto = stocks.Select(stock => stock.ToStockDto());

                var response = new Response<IEnumerable<StockDto>>
                {
                    Data = stocksDto,
                    Success = true,
                    Message = "Stocks retrieved successfully"
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                var response = new Response<IEnumerable<StockDto>>
                {
                    Data = null,
                    Success = false,
                    Message = e.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var stock = await _client.Stocks.Include(stock => stock.Comments).FirstOrDefaultAsync(stock => stock.Id == id);

                if (stock == null)
                {
                    var notFoundResponse = new Response<StockDto>
                    {
                        Data = null,
                        Success = false,
                        Message = $"Stock with id {id} not found"
                    };
                    return NotFound(notFoundResponse);
                }

                var response = new Response<StockDto>
                {
                    Data = stock.ToStockDto(),
                    Success = true,
                    Message = "Stock retrieved successfully"
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                var response = new Response<StockDto>
                {
                    Data = null,
                    Success = false,
                    Message = e.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockDto stockData)
        {
            try
            {
                var stock = new Stock
                {
                    Symbol = stockData.Symbol,
                    CompanyName = stockData.CompanyName,
                    Purchase = stockData.Purchase,
                    LastDiv = stockData.LastDiv,
                    Industry = stockData.Industry,
                    MarkCap = stockData.MarkCap
                };

                await _client.Stocks.AddAsync(stock);
                await _client.SaveChangesAsync();

                var createdResponse = new Response<StockDto>
                {
                    Data = stock.ToStockDto(),
                    Success = true,
                    Message = "Stock created successfully"
                };
                return CreatedAtAction(nameof(GetById), new { id = stock.Id }, createdResponse);
            }
            catch (Exception e)
            {
                var response = new Response<StockDto>
                {
                    Data = null,
                    Success = false,
                    Message = e.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStockDto stock)
        {
            try
            {
                var stockToUpdate = await _client.Stocks.FindAsync(id);

                if (stockToUpdate == null)
                {
                    var notFoundResponse = new Response<StockDto>
                    {
                        Data = null,
                        Success = false,
                        Message = $"Stock with id {id} not found"
                    };
                    return NotFound(notFoundResponse);
                }

                stockToUpdate.Symbol = stock.Symbol;
                stockToUpdate.CompanyName = stock.CompanyName;
                stockToUpdate.Purchase = stock.Purchase;
                stockToUpdate.LastDiv = stock.LastDiv;
                stockToUpdate.Industry = stock.Industry;
                stockToUpdate.MarkCap = stock.MarkCap;

                _client.Stocks.Update(stockToUpdate);
                await _client.SaveChangesAsync();

                var updatedResponse = new Response<StockDto>
                {
                    Data = stockToUpdate.ToStockDto(),
                    Success = true,
                    Message = "Stock updated successfully"
                };
                return Ok(updatedResponse);
            }
            catch (Exception e)
            {
                var response = new Response<StockDto>
                {
                    Data = null,
                    Success = false,
                    Message = e.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                var stock = await _client.Stocks.FindAsync(id);

                if (stock == null)
                {
                    var notFoundResponse = new Response<StockDto>
                    {
                        Data = null,
                        Success = false,
                        Message = $"Stock with id {id} not found"
                    };
                    return NotFound(notFoundResponse);
                }

                _client.Stocks.Remove(stock);
                await _client.SaveChangesAsync();

                var deletedResponse = new Response<StockDto>
                {
                    Data = stock.ToStockDto(),
                    Success = true,
                    Message = "Stock deleted successfully"
                };
                return Ok(deletedResponse);

            }
            catch (Exception e)
            {
                var response = new Response<StockDto>
                {
                    Data = null,
                    Success = false,
                    Message = e.Message
                };

                return StatusCode(500, response);
            }
        }

    }
}
