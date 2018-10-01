using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD.ApplicationServices
{
    public class Selections : IEnumerable<Selection>
    {
        private Dictionary<ObjectId, Selection> _id2Selction = new Dictionary<ObjectId, Selection>();

        internal delegate void Changed();
        internal event Changed changed;

        public int Count
        {
            get { return _id2Selction.Count; }
        }

        internal Selections()
        {
        }

        public bool IsObjectSelected(ObjectId objectId)
        {
            return _id2Selction.ContainsKey(objectId);
        }

        private void AddSelection(Selection sel)
        {
            _id2Selction[sel.objectId] = sel;
            if (changed != null)
            {
                changed.Invoke();
            }
        }

        private void AddSelections(IEnumerable<Selection> sels)
        {
            foreach (Selection sel in sels)
            {
                _id2Selction[sel.objectId] = sel;
            }
            if (changed != null)
            {
                changed.Invoke();
            }
        }

        private void RemoveSelection(Selection sel)
        {
            _id2Selction.Remove(sel.objectId);
            if (changed != null)
            {
                changed.Invoke();
            }
        }

        public bool Add(Selection sel)
        {
            if (IsSelectionCanAdd(sel))
            {
                this.AddSelection(sel);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Add(IEnumerable<Selection> sels)
        {
            List<Selection> filterSels = new List<Selection>();
            foreach (Selection sel in sels)
            {
                if (IsSelectionCanAdd(sel))
                {
                    filterSels.Add(sel);
                }
            }

            if (filterSels.Count > 0)
            {
                AddSelections(filterSels);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsSelectionCanAdd(Selection sel)
        {
            if (sel.objectId == ObjectId.Null)
            {
                return false;
            }

            if (this.IsObjectSelected(sel.objectId))
            {
                return false;
            }

            return true;
        }

        public bool Add(ObjectId objectId)
        {
            return this.Add(new Selection(objectId, new LitMath.Vector2(0, 0)));
        }

        public bool Remove(Selection selection)
        {
            if (selection.objectId == ObjectId.Null)
            {
                return false;
            }

            if (this.IsObjectSelected(selection.objectId))
            {
                this.RemoveSelection(selection);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(ObjectId objectId)
        {
            return this.Remove(new Selection(objectId, new LitMath.Vector2(0, 0)));
        }

        public void Clear()
        {
            if (_id2Selction.Count > 0)
            {
                _id2Selction.Clear();
                if (changed != null)
                {
                    changed.Invoke();
                }
            }
        }

        #region IEnumerable<Selection>
        public IEnumerator<Selection> GetEnumerator()
        {
            return _id2Selction.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
