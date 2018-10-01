using System;
using System.Collections.Generic;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class SnapNodeHostText : SnapNodeHost
    {
        public override void UpdateSnapNodes(Entity entity)
        {
            _nodes.Clear();
            Text text = entity as Text;
            if (text == null)
            {
                return;
            }

            SnapNodeEnd node = new SnapNodeEnd(text.position);
            _nodes.Add(node);
        }
    }
}
