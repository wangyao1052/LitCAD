using System;
using System.Collections.Generic;

namespace LitCAD.Utils
{
    internal class HashSet<T> : IEnumerable<T>
    {
        private Dictionary<T, object> _dict = new Dictionary<T, object>();

        public int Count
        {
            get { return _dict.Count; }
        }

        public bool Add(T item)
        {
            if (_dict.ContainsKey(item))
            {
                return false;
            }
            else
            {
                _dict.Add(item, null);
                return true;
            }
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool Contains(T item)
        {
            return _dict.ContainsKey(item);
        }

        public bool Remove(T item)
        {
            return _dict.Remove(item);
        }

        #region IEnumerable<T>
        public IEnumerator<T> GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
