using System;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class PolylineRS : EntityRS
    {
        internal override bool Cross(Bounding selectBound, Entity entity)
        {
            Polyline polyline = entity as Polyline;
            if (polyline == null)
            {
                return false;
            }

            Bounding polylineBound = polyline.bounding;
            if (selectBound.Contains(polylineBound))
            {
                return true;
            }

            LitMath.Rectangle2 selRect = new LitMath.Rectangle2(
                new LitMath.Vector2(selectBound.left, selectBound.bottom),
                new LitMath.Vector2(selectBound.right, selectBound.top));

            LitMath.Line2 rectLine1 = new LitMath.Line2(selRect.leftBottom, selRect.leftTop);
            LitMath.Line2 rectLine2 = new LitMath.Line2(selRect.leftTop, selRect.rightTop);
            LitMath.Line2 rectLine3 = new LitMath.Line2(selRect.rightTop, selRect.rightBottom);
            LitMath.Line2 rectLine4 = new LitMath.Line2(selRect.rightBottom, selRect.leftBottom);

            for (int i = 1; i < polyline.NumberOfVertices; ++i)
            {
                LitMath.Vector2 spnt = polyline.GetPointAt(i - 1);
                LitMath.Vector2 epnt = polyline.GetPointAt(i);
                LitMath.Line2 line2 = new LitMath.Line2(spnt, epnt);
                LitMath.Vector2 intersection = new LitMath.Vector2();
                if (LitMath.Line2.Intersect(rectLine1, line2, ref intersection)
                    || LitMath.Line2.Intersect(rectLine2, line2, ref intersection)
                    || LitMath.Line2.Intersect(rectLine3, line2, ref intersection)
                    || LitMath.Line2.Intersect(rectLine4, line2, ref intersection))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
