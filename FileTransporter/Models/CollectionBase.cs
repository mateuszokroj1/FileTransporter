using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FileTransporter.Models
{
    internal abstract class CollectionBase<T> : ICollection<T>
    {
        #region Constructor
        public CollectionBase()
        {
            this.values = new List<T>();
        }
        #endregion

        #region Fields
        protected List<T> values;
        #endregion

        #region Properties
        public int Count => this.values.Count;

        public bool IsReadOnly => true;
        #endregion

        #region Methods
        public virtual void Add(T item) => throw new NotImplementedException();
        public virtual void Clear() => throw new NotImplementedException();
        public virtual bool Contains(T item) => throw new NotImplementedException();
        public void CopyTo(T[] array, int arrayIndex) => this.values.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => this.values.GetEnumerator();
        public virtual bool Remove(T item) => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => this.values.GetEnumerator();
        #endregion

        #region Events & Delegates
        public delegate void ChangedValueEventHandler(object sender, ChangedValueEventArgs e);

        public event ChangedValueEventHandler OnValueAdd;
        public event ChangedValueEventHandler OnValueRemove;
        public event ChangedValueEventHandler OnCollectionCleared;
        #endregion
    }

    public class ChangedValueEventArgs : EventArgs
    {

    }
}
