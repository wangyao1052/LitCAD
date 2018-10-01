using System;

using LitCAD.DatabaseServices;

namespace LitCAD.UI
{
    internal class LineHitter : EntityHitter
    {
        internal override bool Hit(PickupBox pkbox, Entity entity)
        {
            Line line = entity as Line;
            if (line == null)
                return false;

            Bounding pkBounding = pkbox.reservedBounding;
            return LineHitter.BoundingIntersectWithLine(
                pkBounding,
                new LitMath.Line2(line.startPoint, line.endPoint));
        }

        internal static bool BoundingIntersectWithLine(Bounding bounding, LitMath.Line2 line)
        {
            Bounding lineBound = new Bounding(line.startPoint, line.endPoint);
            if (!bounding.IntersectWith(lineBound))
            {
                return false;
            }

            if (bounding.Contains(line.startPoint)
                || bounding.Contains(line.endPoint))
            {
                return true;
            }

            LitMath.Vector2 pkPnt1 = new LitMath.Vector2(bounding.left, bounding.bottom);
            LitMath.Vector2 pkPnt2 = new LitMath.Vector2(bounding.left, bounding.top);
            LitMath.Vector2 pkPnt3 = new LitMath.Vector2(bounding.right, bounding.top);
            LitMath.Vector2 pkPnt4 = new LitMath.Vector2(bounding.right, bounding.bottom);

            double d1 = LitMath.Vector2.Cross(line.startPoint - pkPnt1, line.endPoint - pkPnt1);
            double d2 = LitMath.Vector2.Cross(line.startPoint - pkPnt2, line.endPoint - pkPnt2);
            double d3 = LitMath.Vector2.Cross(line.startPoint - pkPnt3, line.endPoint - pkPnt3);
            double d4 = LitMath.Vector2.Cross(line.startPoint - pkPnt4, line.endPoint - pkPnt4);

            if (d1 * d2 <= 0 || d1 * d3 <= 0 || d1 * d4 <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
