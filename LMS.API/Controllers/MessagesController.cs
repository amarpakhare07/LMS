using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {

        private readonly IMessageRepository _repository;

        public MessagesController(IMessageRepository repository)
        {
            _repository = repository;
        }


        [HttpPost("send")]
        [Authorize(Roles = "Instructor,Student")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var message = new Message
            {
                SenderID = Convert.ToInt32(senderId),
                ReceiverID = Convert.ToInt32(dto.ReceiverId),
                Content = dto.Content
            };

            await _repository.SendMessageAsync(message);
            return Ok("Message sent.");
        }




        [HttpGet("conversation/{otherUserId}")]
        [Authorize(Roles = "Instructor,Student")]
        public async Task<IActionResult> GetConversation(string otherUserId)
        {
            // Check if the NameIdentifier claim exists and get its value
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Handle the case where the user ID is not found
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in claims.");
            }

            // Now, you can safely use 'userId'
            var messages = await _repository.GetMessagesAsync(userId, otherUserId);

            // It's also a good practice to handle the case where messages are null
            if (messages == null)
            {
                return NotFound("Conversation not found.");
            }

            var response = messages.Select(m => new MessageDto
            {
                SenderName = m.Sender.Name,
                Content = m.Content,
                SentAt = m.CreatedAt,
                IsRead = m.IsRead
            }).ToList();

            return Ok(response);
        }

    }
}
