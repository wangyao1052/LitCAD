using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class XlineAnchorCreator : EntityAnchorCreator
    {
        public override List<EntityAnchor> NewAnchors(Entity entity)
        {
            List<EntityAnchor> lstAnchor = new List<EntityAnchor>();
            Xline xline = entity as Xline;
            if (xline == null)
            {
                return lstAnchor;
            }

            XlineAnchor xlineBasePnt = new XlineAnchor(xline, XlineAnchor.Type.BasePoint);
            XlineAnchor xlineDirPnt = new XlineAnchor(xline, XlineAnchor.Type.DirectionPoint);
            XlineAnchor xlineDirPntMinus = new XlineAnchor(xline, XlineAnchor.Type.DirectionPointMinus);

            lstAnchor.Add(xlineBasePnt);
            lstAnchor.Add(xlineDirPnt);
            lstAnchor.Add(xlineDirPntMinus);

            return lstAnchor;
        }
    }
}
