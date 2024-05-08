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

    }
}