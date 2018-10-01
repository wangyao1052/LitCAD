using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class RayAnchorCreator : EntityAnchorCreator
    {
        public override List<EntityAnchor> NewAnchors(Entity entity)
        {
            List<EntityAnchor> lstAnchor = new List<EntityAnchor>();
            Ray ray = entity as Ray;
            if (ray == null)
            {
                return lstAnchor;
            }

            RayAnchor rayBasePnt = new RayAnchor(ray, RayAnchor.Type.BasePoint);
            RayAnchor rayDirPnt = new RayAnchor(ray, RayAnchor.Type.DirectionPoint);

            lstAnchor.Add(rayBasePnt);
            lstAnchor.Add(rayDirPnt);

            return lstAnchor;
        }
    }
}
