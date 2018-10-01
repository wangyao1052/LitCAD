using System;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class XlineRS : EntityRS
    {
        internal override bool Cross(Bounding selectBound, Entity entity)
        {
            Xline xline = entity as Xline;
            if (xline == null)
            {
                return false;
            }

            return LitCAD.UI.XlineHitter.BoundingIntersectWithXline(selectBound, xline);
        }
    }
}
