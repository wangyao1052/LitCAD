using System;
using System.Collections.Generic;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class SnapNodeHostCircle : SnapNodeHost
    {
        public override void UpdateSnapNodes(Entity entity)
        {
            _nodes.Clear();
            Circle circle = entity as Circle;
            if (circle == null)
            {
                return;
            }

            SnapNodeCenter nodeCenter = new SnapNodeCenter(circle.center);

            _nodes.Add(nodeCenter);
        }
    }
}
