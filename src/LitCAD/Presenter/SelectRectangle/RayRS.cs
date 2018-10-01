using System;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class RayRS : EntityRS
    {
        internal override bool Cross(Bounding selectBound, Entity entity)
        {
            Ray ray = entity as Ray;
            if (ray == null)
            {
                return false;
            }

            return LitCAD.UI.RayHitter.BoundingIntersectWithRay(selectBound, ray);
        }
    }
}
