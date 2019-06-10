using System;

namespace FileTransporter.Models
{
    #region Delegates

    public delegate void ServerEventHandler(object sender, ServerEventArgs e);

    #endregion

    #region Types

    public class ServerEventArgs : EventArgs
    {
        public Server Server { get; set; }
    }

    #endregion
}
