using System;
using System.Net.Sockets;

namespace FileTransporter.Models
{
    public class ClientConnectionState
    {
        public ClientConnectionState(Socket socket)
        {
            if(socket is null)
                throw new ArgumentNullException();
            Socket = socket;
            Buffer = new byte[0];
        }

        public Guid ClientId { get; set; }
        public bool IsValidConnection { get; set; }
        public string Password { get; set; }
        public Socket Socket { get; protected set; }
        public byte[] Buffer { get; set; }
    }
}
