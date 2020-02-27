using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FileTransporter.Models
{
    internal class ClientsCollection : CollectionBase<Client>
    {
        internal new void Add(Client item)
        {
            if (item == null || item.Id == null || item.Hostname == null || item.Address == null)
                throw new ArgumentNullException();

            this.values.Add(item);
        }

        [Obsolete]
        internal new void Clear() => throw new NotImplementedException();

        internal new bool Remove(Client item)
        {

        }
    }
}
