using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimalApi.Dtos.Comment;
using minimalApi.Mappers;
using minimalApi.Models.Responses;

namespace minimalApi.Controllers
{
    [ApiController]
    [Route("api/comment")]
    public class CommentController : ControllerBase
    {

        private ILogger<CommentController> _logger;
        private readonly ApplicationDbContext _client;

        public CommentController(ILogger<CommentController> logger, ApplicationDbContext client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                IEnumerable<Comment> comments = await _client.Comments.ToListAsync();

                IEnumerable<CommentDto> commentsDto = comments.Select(comment => comment.ToCommentDto());

                var response = new Response<IEnumerable<CommentDto>>
                {
                    Data = commentsDto,
                    Success = true,
                    Message = "Comments retrieved successfully"
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                var response = new Response<IEnumerable<string>>
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
                Comment comment = await _client.Comments.FindAsync(id);

                if (comment == null)
                {
                    return NotFound();
                }

                CommentDto commentDto = comment.ToCommentDto();

                var response = new Response<CommentDto>
                {
                    Data = commentDto,
                    Success = true,
                    Message = "Comment retrieved successfully"
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                var response = new Response<string>
                {
                    Data = null,
                    Success = false,
                    Message = e.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpPost("{stockId}")]
        public async Task<IActionResult> Create(int stockId, [FromBody] CreateCommentDto createCommentDto)
        {
            try
            {
                Stock stock = await _client.Stocks.FirstOrDefaultAsync(stock => stock.Id == stockId);

                if (stock == null)
                {
                    var notFoundedStockResponse = new Response<Comment>
                    {
                        Data = null,
                        Success = false,
                        Message = "Stock not found"
                    };
                    return BadRequest(notFoundedStockResponse);
                }

                var newComment = createCommentDto.FromCreateDtoToComment(stockId);

                await _client.Comments.AddAsync(newComment);
                await _client.SaveChangesAsync();

                var response = new Response<Comment>
                {
                    Data = newComment,
                    Success = true,
                    Message = "Comment created successfully"
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                var response = new Response<string>
                {
                    Data = null,
                    Success = false,
                    Message = e.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCommentDto updateComment)
        {
            try
            {

                Comment comment = await _client.Comments.Include(comment => comment.Stock).FirstOrDefaultAsync(comment => comment.Id == id);

                if (comment == null)
                {
                    var notFoundedCommentResponse = new Response<Comment>
                    {
                        Data = null,
                        Success = false,
                        Message = "Comment not found"
                    };
                    return NotFound(notFoundedCommentResponse);
                }

                comment.Title = updateComment.Title;
                comment.Content = updateComment.Content;

                _client.Comments.Update(comment);
                await _client.SaveChangesAsync();

                var response = new Response<Comment>
                {
                    Data = comment,
                    Success = true,
                    Message = "Comment updated successfully"
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                var response = new Response<string>
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
                Comment comment = await _client.Comments.FindAsync(id);

                if (comment == null)
                {
                    var notFoundedCommentResponse = new Response<Comment>
                    {
                        Data = null,
                        Success = false,
                        Message = "Comment not found"
                    };
                    return NotFound(notFoundedCommentResponse);
                }

                _client.Comments.Remove(comment);
                await _client.SaveChangesAsync();

                var response = new Response<Comment>
                {
                    Data = comment,
                    Success = true,
                    Message = "Comment deleted successfully"
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                var response = new Response<string>
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