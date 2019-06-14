using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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

    public class Client
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
        public IPAddress IP { get; set; }
        public string Hostname { get; set; }

        public IEnumerable<FileInfo> TransferingFiles { get; set; }
    }

    public class FileInfo
    {
        public Guid Id { get; set; }
        public string LocalPath { get; set; }
        public string Filename => Path.GetFileName(LocalPath);
        public uint Size { get; set; }
    }
}
