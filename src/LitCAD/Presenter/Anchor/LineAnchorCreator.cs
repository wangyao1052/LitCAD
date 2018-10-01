using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class LineAnchorCreator : EntityAnchorCreator
    {
        public override List<EntityAnchor> NewAnchors(Entity entity)
        {
            Line line = entity as Line;

            LineAnchor lineStartAnchor = new LineAnchor(line, LineAnchor.Type.StartPoint);
            LineAnchor lineEndAnchor = new LineAnchor(line, LineAnchor.Type.EndPoint);
            LineAnchor lineMiddleAnchor = new LineAnchor(line, LineAnchor.Type.MiddlePoint);

            List<EntityAnchor> lstAnchor = new List<EntityAnchor>();
            lstAnchor.Add(lineStartAnchor);
            lstAnchor.Add(lineEndAnchor);
            lstAnchor.Add(lineMiddleAnchor);

            return lstAnchor;
        }
    }
}
