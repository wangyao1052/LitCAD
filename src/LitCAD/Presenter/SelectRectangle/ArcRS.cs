using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class ArcRS : EntityRS
    {
        internal override bool Cross(Bounding selectBound, Entity entity)
        {
            Arc arc = entity as Arc;
            if (arc == null)
            {
                return false;
            }

            Bounding arcBounding = arc.bounding;
            if (selectBound.Contains(arcBounding))
            {
                return true;
            }

            if (!selectBound.IntersectWith(arcBounding))
                return false;

            Circle circle = new Circle(arc.center, arc.radius);
            LitMath.Vector2 nearestPntOnBound = new LitMath.Vector2(
                Math.Max(selectBound.left, Math.Min(circle.center.x, selectBound.right)),
                Math.Max(selectBound.bottom, Math.Min(circle.center.y, selectBound.top)));

            if (LitMath.Vector2.Distance(nearestPntOnBound, circle.center) <= circle.radius)
            {
                double bdLeft = selectBound.left;
                double bdRight = selectBound.right;
                double bdTop = selectBound.top;
                double bdBottom = selectBound.bottom;

                List<LitMath.Vector2> pnts = new List<LitMath.Vector2>();
                pnts.Add(new LitMath.Vector2(bdLeft, bdTop));
                pnts.Add(new LitMath.Vector2(bdLeft, bdBottom));
                pnts.Add(new LitMath.Vector2(bdRight, bdTop));
                pnts.Add(new LitMath.Vector2(bdRight, bdBottom));
                LitMath.Vector2 xp = new LitMath.Vector2(1, 0);
                foreach (LitMath.Vector2 pnt in pnts)
                {
                    if (LitMath.Vector2.Distance(pnt, circle.center) >= circle.radius)
                    {
                        LitMath.Vector2 v = pnt - circle.center;
                        double rad = LitMath.Vector2.AngleInRadian(xp, v);
                        if (LitMath.Vector2.Cross(xp, v) < 0)
                            rad = Math.PI * 2 - rad;

                        if (AngleInRange(rad, arc.startAngle, arc.endAngle))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        private bool AngleInRange(double angle, double startAngle, double endAngle)
        {
            if (endAngle >= startAngle)
            {
                return angle >= startAngle 
                    && angle <= endAngle;
            }
            else
            {
                return angle >= startAngle
                    || angle <= endAngle;
            }
        }
    }
}
