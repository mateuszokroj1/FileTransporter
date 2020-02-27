using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FileTransporter.Models
{
    internal abstract class CollectionBase<T> : ICollection<T> where T : IEquatable<T>
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

        public T this[int index] => this.values[index];
        #endregion

        #region Methods
        public virtual void Add(T item) => throw new NotImplementedException();
        public virtual void Clear() => throw new NotImplementedException();
        public bool Contains(T item) =>
            this.values.Where(value => value.Equals(item)).Count() == 1;
        public void CopyTo(T[] array, int arrayIndex) =>
            this.values.CopyTo(array, arrayIndex);
        public virtual bool Remove(T item) => throw new NotImplementedException();
        public IEnumerator<T> GetEnumerator() => this.values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.values.GetEnumerator();
        #endregion

        #region Events
        public event ChangedValueEventHandler<T> OnAdded;
        public event ChangedValueEventHandler<T> OnRemoved;
        #endregion
    }

    public delegate void ChangedValueEventHandler<Tvalue>(object sender, ChangedValueEventArgs<Tvalue> e) where Tvalue : IEquatable<Tvalue>;
    public class ChangedValueEventArgs<T> : EventArgs
    {
        public IEnumerable<T> ChangedValues { get; set; }
    }
}
