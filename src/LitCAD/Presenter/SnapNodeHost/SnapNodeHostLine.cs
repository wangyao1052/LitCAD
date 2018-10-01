using System;
using System.Collections.Generic;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class SnapNodeHostLine : SnapNodeHost
    {
        public override void UpdateSnapNodes(Entity entity)
        {
            _nodes.Clear();
            Line line = entity as Line;
            if (line == null)
            {
                return;
            }

            SnapNodeEnd nodeStart = new SnapNodeEnd(line.startPoint);
            SnapNodeEnd nodeEnd = new SnapNodeEnd(line.endPoint);
            SnapNodeMiddle nodeMiddle = new SnapNodeMiddle((line.startPoint + line.endPoint) / 2);

            _nodes.Add(nodeStart);
            _nodes.Add(nodeEnd);
            _nodes.Add(nodeMiddle);
        }
    }
}
