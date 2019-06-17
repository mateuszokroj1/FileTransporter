using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FileTransporter.Models
{
    #region Delegates

    public delegate void ListenerEventHandler(object sender, ListenerEventArgs e);
    public delegate void TimeoutEventHandler(object sender, TimeoutEventArgs e);
    public delegate void ClientConnectedEventHandler(object sender, ClientConnectedEventArgs e);
    public delegate void ClientDisconnectedEventHandler(object sender, ClientDisconnectedEventArgs e);
    public delegate void TransferRequestEventHandler(object sender, TransferRequestEventArgs e);
    public delegate void ClosingEventHandler(object sender, ClosingEventArgs e);

    #endregion

    #region Types

    public class ListenerEventArgs : EventArgs
    {
        public Listener Server { get; set; }
        public DateTime UtcTime { get; set; }
    }

    public class TimeoutEventArgs : ListenerEventArgs
    {
        public uint MaxTime { get; set; }
    }

    public class ClientConnectedEventArgs : ListenerEventArgs
    {
        public Guid ClientId { get; set; }
        public IPAddress IP { get; set; }
        public string Password { get; set; }
        public string Hostname { get; set; }
    }

    public class ClientDisconnectedEventArgs : ListenerEventArgs
    {
        public Guid ClientId { get; set; }
        public IPAddress IP { get; set; }
    }

    public class TransferRequestEventArgs : ListenerEventArgs
    {
        public Client Client { get; set; }
        public string Filename { get; set; }
        public ulong Size { get; set; }
        public Guid Id { get; set; }
        public bool IsAccepted { get; protected set; } = false;
        public Stream Stream { get; set; }
        public Listener Server { get; set; }
        public bool IsCanceled { get; protected set; } = false;
        public void IsTimeout()
        {
            TimeoutEventArgs e = new TimeoutEventArgs
            {
                Server = Server,
                MaxTime =,
                UtcTime = DateTime.UtcNow
            };

            OnTimeout?.BeginInvoke(this, e, null, null);
        }
        public event TimeoutEventHandler OnTimeout;
        public void Accept(Stream writableStream)
        {
            if(writableStream is null)
                throw new ArgumentNullException();
            if(!writableStream.CanWrite)
                throw new IOException("Stream is not writable.");
            Stream = writableStream;
            IsAccepted = true;
        }
        public void Cancel() => IsCanceled = true;
    }

    public class ClosingEventArgs : ListenerEventArgs
    {
        protected bool IsCanceled { get; set; } = false;
        public void Cancel() => IsCanceled = true;
    }

    #endregion
}
