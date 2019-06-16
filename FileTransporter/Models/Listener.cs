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
            msg = "INVALID MESSAGE";
            this.invalidMessage = Encoding.ASCII.GetBytes(msg);
            ConnectedClients = new Client[0];
            GeneratePassword = DefaultPasswordGenerator;
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
        public Func<string> GeneratePassword { get; set; }
        public string CurrentPassword { get; protected set; }

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

            CurrentPassword = GeneratePassword();

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
            if(msg.StartsWith("LOGIN ")) // Connecting to server
            {
                msg = Encoding.ASCII.GetString(state.Buffer, 6, Math.Min(300, state.Buffer.Length));
                string[] arguments = msg.Split(' ');
                if(arguments.Length != 4 || arguments[0] != "HOST" || arguments[2] != "PASS")
                {
                    state.Buffer = null;
                    state.Socket.Send(this.invalidMessage);
                }
                string host = arguments[1];
                string pass = arguments[3];
                if(pass != CurrentPassword)
                {

                }
                //OnClientConnection(this, new ClientConnectionEventArgs { });
            }
            else if(msg.StartsWith("FILE ADD ")) // Start new file transfer
            {

            }
            else if(msg.StartsWith("FILE STOP ")) // Abort file transfer
            {

            }
            else if(msg.StartsWith("FILE DATA ")) // Set data block of file
            {

            }
            else if(msg.StartsWith("LOGOUT ")) // Close connection
            {

            }
            else // Invalid message
            {
                state.Buffer = null;
                state.Socket.Send(this.invalidMessage);
            }
        }

        public string DefaultPasswordGenerator()
        {
            char[] chars = new[] { '0','1','2','3','4','5','6','7','8','9',
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N',
                'a','b','c','d','e','f','g','h','i','j','k','l','m','n'
            };
            string ret = string.Empty;
            Random rand = new Random(3);
            for(uint i = 1; i <= 8; i++)
                ret += chars[rand.Next(0, 37)];
            return ret;
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
