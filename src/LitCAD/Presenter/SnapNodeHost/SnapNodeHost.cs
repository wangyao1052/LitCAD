using System;
using System.Collections.Generic;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal abstract class SnapNodeHost : IEnumerable<SnapNode>
    {
        protected List<SnapNode> _nodes = new List<SnapNode>();

        public abstract void UpdateSnapNodes(Entity entity);

        #region IEnumerable<DBTableRecord>
        public IEnumerator<SnapNode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
