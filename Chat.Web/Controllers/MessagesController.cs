
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using Chat.Web.Data;
using Chat.Web.Hubs;
using Chat.Web.Models;
using Chat.Web.ViewModels;

namespace Chat.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagesController(ApplicationDbContext context,
            IMapper mapper,
            IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Room>> Get(int id)
        {
            Message message = await _context.Messages.FindAsync(id);
            if (message == null) return NotFound();

            MessageViewModel messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
            return Ok(messageViewModel);
        }

        [HttpGet("Room/{roomName}")]
        public IActionResult GetMessages(string roomName)
        {
            Room room = _context.Rooms.FirstOrDefault(dbRoom => dbRoom.Name == roomName);
            if (room == null) return BadRequest();

            List<Message> messages = _context.Messages.Where(message => message.ToRoomId == room.Id)
                .Include(message => message.FromUser)
                .Include(message => message.ToRoom)
                .OrderByDescending(message => message.Timestamp)
                .Take(20)
                .AsEnumerable()
                .Reverse()
                .ToList();

            IEnumerable<MessageViewModel> messagesViewModel =
                _mapper.Map<IEnumerable<Message>, IEnumerable<MessageViewModel>>(messages);

            return Ok(messagesViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<Message>> Create(MessageViewModel messageViewModel)
        {
            ApplicationUser user = _context.Users.FirstOrDefault(dbUser => dbUser.UserName == User.Identity.Name);
            Room room = _context.Rooms.FirstOrDefault(dbRoom => dbRoom.Name == messageViewModel.Room);
            if (room == null) return BadRequest();

            var msg = new Message
            {
                Content = Regex.Replace(messageViewModel.Content, @"<.*?>", string.Empty),
                FromUser = user,
                ToRoom = room,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            // Broadcast the message
            MessageViewModel createdMessage = _mapper.Map<Message, MessageViewModel>(msg);
            await _hubContext.Clients.Group(room.Name).SendAsync("newMessage", createdMessage);

            return CreatedAtAction(nameof(Get), new { id = msg.Id }, createdMessage);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            Message message = await _context.Messages
                .Include(message => message.FromUser)
                .Where(message => message.Id == id && message.FromUser.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();

            if (message == null) return NotFound();

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
