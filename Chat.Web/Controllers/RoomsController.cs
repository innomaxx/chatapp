
using System.Collections.Generic;
using System.Linq;
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
    public class RoomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public RoomsController(ApplicationDbContext context,
            IMapper mapper,
            IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomViewModel>>> Get()
        {
            List<Room> rooms = await _context.Rooms.ToListAsync();

            IEnumerable<RoomViewModel> roomsViewModel =
                _mapper.Map<IEnumerable<Room>, IEnumerable<RoomViewModel>>(rooms);

            return Ok(roomsViewModel);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Room>> Get(int id)
        {
            Room room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            RoomViewModel roomViewModel = _mapper.Map<Room, RoomViewModel>(room);
            return Ok(roomViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<Room>> Create(RoomViewModel roomViewModel)
        {
            if (_context.Rooms.Any(r => r.Name == roomViewModel.Name))
                return BadRequest("Invalid room name or room already exists");

            ApplicationUser user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            
            var room = new Room
            {
                Name = roomViewModel.Name,
                Admin = user
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("addChatRoom", new { id = room.Id, name = room.Name });

            return CreatedAtAction(nameof(Get), new { id = room.Id }, new { id = room.Id, name = room.Name });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, RoomViewModel roomViewModel)
        {
            if (_context.Rooms.Any(r => r.Name == roomViewModel.Name))
                return BadRequest("Invalid room name or room already exists");

            Room room = await _context.Rooms
                .Include(r => r.Admin)
                .Where(room => room.Id == id && room.Admin.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();

            if (room == null) return NotFound();

            room.Name = roomViewModel.Name;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("updateChatRoom", new { id = room.Id, room.Name});

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            Room room = await _context.Rooms
                .Include(room => room.Admin)
                .Where(room => room.Id == id && room.Admin.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();

            if (room == null) return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("removeChatRoom", room.Id);
            await _hubContext.Clients.Group(room.Name).SendAsync("onRoomDeleted", string.Format("Room {0} has been deleted.\nYou are moved to the first available room!", room.Name));

            return NoContent();
        }
    }
}
