using System;
using System.Collections.Generic;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class SnapNodeHostPolyline : SnapNodeHost
    {
        public override void UpdateSnapNodes(Entity entity)
        {
            _nodes.Clear();
            Polyline polyline = entity as Polyline;
            if (polyline == null)
            {
                return;
            }

            for (int i = 0; i < polyline.NumberOfVertices; ++i)
            {
                SnapNodeEnd nodeEnd = new SnapNodeEnd(polyline.GetPointAt(i));
                _nodes.Add(nodeEnd);

                if (i != polyline.NumberOfVertices - 1)
                {
                    SnapNodeMiddle nodeMiddle = new SnapNodeMiddle(
                        (polyline.GetPointAt(i) + polyline.GetPointAt(i + 1)) / 2);
                    _nodes.Add(nodeMiddle);
                }
            }
        }
    }
}
