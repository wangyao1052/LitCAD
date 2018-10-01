using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class ArcAnchorCreator : EntityAnchorCreator
    {
        public override List<EntityAnchor> NewAnchors(Entity entity)
        {
            Arc arc = entity as Arc;
            ArcAnchor arcStart = new ArcAnchor(arc, ArcAnchor.Type.Start);
            ArcAnchor arcEnd = new ArcAnchor(arc, ArcAnchor.Type.End);
            ArcAnchor arcMiddle = new ArcAnchor(arc, ArcAnchor.Type.Middle);
            ArcAnchor arcCenter = new ArcAnchor(arc, ArcAnchor.Type.Center);

            List<EntityAnchor> lstAnchor = new List<EntityAnchor>();
            lstAnchor.Add(arcStart);
            lstAnchor.Add(arcEnd);
            lstAnchor.Add(arcMiddle);
            lstAnchor.Add(arcCenter);

            return lstAnchor;
        }
    }
}
