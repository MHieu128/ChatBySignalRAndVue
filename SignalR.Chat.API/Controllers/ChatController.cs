using Microsoft.AspNetCore.Mvc;

namespace SignalR.Chat.API.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ConnectedUsersService _connectedUsersService;

        public ChatController(ConnectedUsersService connectedUsersService)
        {
            _connectedUsersService = connectedUsersService;
        }

        [HttpGet("users")]
        public IActionResult GetConnectedUsers()
        {
            var connectedUsers = _connectedUsersService.GetConnectedUsers();
            return Ok(connectedUsers);
        } 
    }
}

