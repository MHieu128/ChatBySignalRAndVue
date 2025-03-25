using System.Collections.Generic;

namespace SignalR.Chat.API
{
    public class ConnectedUsersService
    {
        private readonly List<string> _connectedUsers = new List<string>();

        public void AddUser(string connectionId)
        {
            lock (_connectedUsers)
            {
                _connectedUsers.Add(connectionId);
            }
        }

        public void RemoveUser(string connectionId)
        {
            lock (_connectedUsers)
            {
                _connectedUsers.Remove(connectionId);
            }
        }

        public List<string> GetConnectedUsers()
        {
            lock (_connectedUsers)
            {
                return new List<string>(_connectedUsers);
            }
        }
    }
}

