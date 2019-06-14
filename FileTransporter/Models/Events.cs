using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FileTransporter.Models
{
    #region Delegates

    public delegate void ListenerEventHandler(object sender, ListenerEventArgs e);
    public delegate void TimeoutEventHandler(object sender, TimeoutEventArgs e);
    public delegate void ClientConnectionEventHandler(object sender, ClientConnectionEventArgs e);
    public delegate void TransferRequestEventHandler(object sender, TransferRequestEventArgs e);
    public delegate void ClosingEventHandler(object sender, ClosingEventArgs e);

    #endregion

    #region Types

    public class ListenerEventArgs : EventArgs
    {
        public Listener Server { get; set; }
    }

    public class TimeoutEventArgs : ListenerEventArgs
    {
        public uint MaxTime { get; set; }
    }

    public class ClientConnectionEventArgs : ListenerEventArgs
    {
        public Guid ClientId { get; set; }
        public IPAddress IP { get; set; }
        public string Password { get; set; }
        public string Hostname { get; set; }
        protected bool IsAccepted { get; set; } = false;
        protected bool IsCanceled { get; set; } = false;
        protected bool IsTimeout { get; set; } = false;
        public event TimeoutEventHandler OnTimeout;
        public void IsValid() => IsAccepted = true;
        public void IsInvalid() => IsCanceled = true;
    }

    public class TransferRequestEventArgs : ListenerEventArgs
    {
        public Client Client { get; set; }
        public string Filename { get; set; }
        public uint Size { get; set; }
        public Guid Id { get; set; }
        protected bool IsAccepted { get; set; } = false;
        protected Stream Stream { get; set; }
        protected bool IsCanceled { get; set; } = false;
        protected bool IsTimeout { get; set; } = false;
        public event TimeoutEventHandler OnTimeout;
        public void Accept(Stream writableStream)
        {
            if(writableStream is null)
                throw new ArgumentNullException();
            if(!writableStream.CanWrite)
                throw new IOException("Stream is not writable.");
            if(writableStream.Length < Size)
                throw new IOException("Stream is too small.");
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
