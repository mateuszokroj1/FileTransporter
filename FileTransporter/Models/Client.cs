using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FileTransporter.Models
{
    public class Client : IDisposable
    {
        #region Constructor
        public Client()
        {

        }
        #endregion

        #region Fields
        protected Socket socket;
        protected readonly Version version = new Version(1,0);
        protected byte[] buffer;
        #endregion

        #region Properties
        public Guid Id { get; protected set; }
        public IPEndPoint Address { get; protected set; }
        public bool IsConnected { get; protected set; }
        public bool IsWorking { get; protected set; }
        public FileInfo[] FileTransfers { get; protected set; }
        #endregion

        #region Methods
        public Guid Connect(IPEndPoint localAddress, IPEndPoint remoteAddress)
        {
            socket = new Socket(remoteAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localAddress);
            socket.Connect(remoteAddress);
            if(socket.RemoteEndPoint != remoteAddress)
                throw new Exception();
            while(socket.Available == 0);
            int available = socket.Available;
            if(socket.Receive(buffer) != available)
        }

        public void Disconnect()
        {

        }
        public void Dispose()
        {
            if(IsConnected) Disconnect();
        }
        #endregion
    }
}
