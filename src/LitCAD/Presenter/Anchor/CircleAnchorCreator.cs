using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class CircleAnchorCreator : EntityAnchorCreator
    {
        public override List<EntityAnchor> NewAnchors(Entity entity)
        {
            Circle circle = entity as Circle;
            CircleAnchor circleEnd_0 = new CircleAnchor(circle, CircleAnchor.Type.EndPoint_0);
            CircleAnchor circleEnd_90 = new CircleAnchor(circle, CircleAnchor.Type.EndPoint_90);
            CircleAnchor circleEnd_180 = new CircleAnchor(circle, CircleAnchor.Type.EndPoint_180);
            CircleAnchor circleEnd_270 = new CircleAnchor(circle, CircleAnchor.Type.EndPoint_270);
            CircleAnchor circleCenter = new CircleAnchor(circle, CircleAnchor.Type.CenterPoint);

            List<EntityAnchor> lstAnchor = new List<EntityAnchor>();
            lstAnchor.Add(circleEnd_0);
            lstAnchor.Add(circleEnd_90);
            lstAnchor.Add(circleEnd_180);
            lstAnchor.Add(circleEnd_270);
            lstAnchor.Add(circleCenter);

            return lstAnchor;
        }
    }
}
