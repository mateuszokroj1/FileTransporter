using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FileTransporter.Models
{
    public class Listener : IDisposable
    {
        #region Constructor
        public Listener()
        {
            string msg = $"File Transporter Server v{version}\nWELCOME";
            this.welcomeMessage = Encoding.ASCII.GetBytes(msg);
            msg = "INVALID MESSAGE Not recognized";
            this.invalidMessage = Encoding.ASCII.GetBytes(msg);
            ConnectedClients = new Client[0];
        }
        #endregion

        #region Fields

        protected Socket socket;
        protected readonly Version version = new Version(1,0);
        protected readonly byte[] welcomeMessage;
        protected readonly byte[] invalidMessage;

        #endregion

        #region Properties

        /// <summary>
        /// IP Address and port of this server
        /// </summary>
        public IPEndPoint Address { get; protected set; }
        public bool IsClosing { get; protected set; } = false;
        public bool IsWorking { get; protected set; } = false;
        public Client[] ConnectedClients { get; protected set; }

        #endregion

        #region Events

        public event ListenerEventHandler OnStarting;
        public event ListenerEventHandler OnStarted;
        public event ClosingEventHandler OnClosing;
        public event ListenerEventHandler OnClosed;
        public event ClientConnectionEventHandler OnClientConnection;
        public event TransferRequestEventHandler OnTransferRequest;

        #endregion

        #region Methods

        public void Start(IPAddress address, int port)
        {
            if(IsClosing || IsWorking)
                throw new InvalidOperationException("Listener must be closed before executing Start method.");
            if(address is null || port == 0)
                throw new ArgumentNullException();
            // IP Check
            if(
                NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(interf=>
                    interf.OperationalStatus == OperationalStatus.Up)
                .Select(interf=>new
                {
                    uni = interf.GetIPProperties().UnicastAddresses,
                    multi = interf.GetIPProperties().MulticastAddresses
                })
                .Where(item=>
                    item.uni.Where(ip=> ip.Address == address).Count() > 0 ||
                    item.multi.Where(ip=> ip.Address == address).Count() > 0
                )
                .Count() == 0
            )
                throw new ArgumentException("This IP address isn't assigned to active interface");

            IPEndPoint endpoint = new IPEndPoint(address, port);
            this.socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            this.socket.Bind(endpoint);
            this.socket.Listen(50);

            new Thread(()=>
            {
                while(!IsClosing)
                    socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);
            });
        }

        protected void AcceptCallback(IAsyncResult result)
        {
            var listener = result.AsyncState as Socket;
            var handler = listener.EndAccept(result);

            handler.Send(this.welcomeMessage);
            ClientConnectionState state = new ClientConnectionState(handler);
            handler.BeginReceive(
                state.Buffer,
                0,
                handler.Available <= 64000 ? handler.Available : 64000,
                0,
                new AsyncCallback(ReadCallback),
                state
            );
        }

        protected void ReadCallback(IAsyncResult result)
        {
            var state = result.AsyncState as ClientConnectionState;
            var handler = state.Socket;
            int readed = handler.EndReceive(result);
            if(readed != state.Buffer.Length)
                throw new InvalidOperationException("ERROR READ Buffer length is invalid");
            DecodeMessage(state);
        }

        protected void DecodeMessage(ClientConnectionState state)
        {
            if(state.Buffer.Length < 2)
                throw new InvalidOperationException("ERROR DECODE Buffer length is invalid");
            string msg = Encoding.ASCII.GetString(state.Buffer, 0, Math.Min(10,state.Buffer.Length));

            /* Protocol reading section */
            if(msg.StartsWith("LOGIN")) // Connecting to server
            {

            }
            else if(msg.StartsWith("ADD FILE ")) // Start new file transfer
            {

            }
            else if(msg.StartsWith("STOP FILE ")) // Abort file transfer
            {

            }
            else if(msg.StartsWith("DATA ")) // Set data block of file
            {

            }
            else if(msg.StartsWith("LOGOUT ")) // Close connection
            {

            }
            else // Invalid message
            {
                state.Buffer = null;
                state.Socket.Send();
            }
        }

        public void Close()
        {
            this.IsClosing = true;
            this.socket.Disconnect(false);
            this.socket.Close(1000/* Timeout 1s */);
        }

        public void Dispose() => Close();

        #endregion
    }
}
