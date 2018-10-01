using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;
using LitCAD.UI;

namespace LitCAD.UI
{
    internal class PolylineHitter : EntityHitter
    {
        internal override bool Hit(PickupBox pkbox, Entity entity)
        {
            Polyline polyline = entity as Polyline;
            if (polyline == null)
                return false;

            Bounding pkBounding = pkbox.reservedBounding;
            for (int i = 1; i < polyline.NumberOfVertices; ++i)
            {
                LitMath.Line2 line = new LitMath.Line2(
                    polyline.GetPointAt(i - 1),
                    polyline.GetPointAt(i));

                if (LineHitter.BoundingIntersectWithLine(pkBounding, line))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
