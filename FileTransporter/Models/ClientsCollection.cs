using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FileTransporter.Models
{
    internal class ClientsCollection : ICollection<Client>
    {
        #region Constructor
            public ClientsCollection()
            {
                this.clients = new List<Client>();
            }
        #endregion

        #region Fields
        protected List<Client> clients;
        #endregion

        #region Properties
        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();
        #endregion
        public void Add(Client item) => throw new NotImplementedException();
        public void Clear() => throw new NotImplementedException();
        public bool Contains(Client item) => throw new NotImplementedException();
        public void CopyTo(Client[] array, int arrayIndex) => throw new NotImplementedException();
        public IEnumerator<Client> GetEnumerator() => throw new NotImplementedException();
        public bool Remove(Client item) => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        public delegate 
    }
}
