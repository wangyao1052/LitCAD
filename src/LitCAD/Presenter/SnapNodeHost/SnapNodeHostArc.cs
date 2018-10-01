using System;
using System.Collections.Generic;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class SnapNodeHostArc : SnapNodeHost
    {
        public override void UpdateSnapNodes(Entity entity)
        {
            _nodes.Clear();
            Arc arc = entity as Arc;
            if (arc == null)
            {
                return;
            }

            SnapNodeCenter nodeCenter = new SnapNodeCenter(arc.center);
            SnapNodeEnd nodeStart = new SnapNodeEnd(arc.startPoint);
            SnapNodeEnd nodeEnd = new SnapNodeEnd(arc.endPoint);
            SnapNodeMiddle nodeMiddle = new SnapNodeMiddle(
                DBUtils.ArcUtils.ArcMiddlePoint(arc));


            _nodes.Add(nodeCenter);
            _nodes.Add(nodeStart);
            _nodes.Add(nodeEnd);
            _nodes.Add(nodeMiddle);
        }
    }
}
