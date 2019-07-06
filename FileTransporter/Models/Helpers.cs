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

    public class ClientInfo
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
        public IPAddress IP { get; set; }
        public string Hostname { get; set; }

        public List<FileInfo> TransferingFiles { get; set; }
    }

    public class FileInfo
    {
        public Guid Id { get; set; }
        public string LocalPath { get; set; }
        public string Filename => Path.GetFileName(LocalPath);
        public uint Size { get; set; }
        public Stream Stream { get; set; }
        /// <summary>
        /// From 0 to 1.
        /// </summary>
        public float Progress { get; set; }
    }

    

    public class ReadonlyStream : Stream
    {
        protected ReadonlyStream() {}
        public ReadonlyStream(byte[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();
            this.buffer = buffer;
            this.position = -1;
        }

        protected byte[] buffer;

        public override bool CanRead => Position < this.buffer.LongLength - 1;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => this.buffer.LongLength;

        protected long position;
        public override long Position
        {
            get => this.position;
            set
            {
                if (value >= this.buffer.LongLength)
                    throw new EndOfStreamException();
                this.position = value;
            }
        }

        
        public override int Read(byte[] buffer, int offset, int count)
        {

        }
        public override void Flush() => throw new InvalidOperationException();
        public override long Seek(long offset, SeekOrigin origin) => throw new InvalidOperationException();
        public override void SetLength(long value) => throw new InvalidOperationException();
        public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException();
    }
}
