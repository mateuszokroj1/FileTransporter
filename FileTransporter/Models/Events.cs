using System;
using System.Net.Sockets;

namespace FileTransporter.Models
{
    #region Delegates

    public delegate void ServerEventHandler(object sender, ServerEventArgs e);

    #endregion

    #region Types

    public class ServerEventArgs : EventArgs
    {
        public Listener Server { get; set; }
    }

    #endregion
}
