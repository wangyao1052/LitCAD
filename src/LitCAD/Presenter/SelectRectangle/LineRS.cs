using System;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class LineRS : EntityRS
    {
        internal override bool Cross(Bounding selectBound, Entity entity)
        {
            Line line = entity as Line;
            if (line == null)
            {
                return false;
            }

            Bounding lineBound = line.bounding;
            if (selectBound.Contains(lineBound))
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
            LitMath.Line2 line2 = new LitMath.Line2(line.startPoint, line.endPoint);

            LitMath.Vector2 intersection = new LitMath.Vector2();
            if (LitMath.Line2.Intersect(rectLine1, line2, ref intersection)
                || LitMath.Line2.Intersect(rectLine2, line2, ref intersection)
                || LitMath.Line2.Intersect(rectLine3, line2, ref intersection)
                || LitMath.Line2.Intersect(rectLine4, line2, ref intersection))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //internal override bool Window(Bounding selectBound, Entity entity)
        //{
        //    Line line = entity as Line;
        //    if (line == null)
        //    {
        //        return false;
        //    }

        //    LitMath.Rectangle2 selRect = new LitMath.Rectangle2(
        //        new LitMath.Vector2(selectBound.left, selectBound.bottom),
        //        new LitMath.Vector2(selectBound.right, selectBound.top));

        //    if (MathUtils.IsPointInRectangle(line.startPoint, selRect)
        //        && MathUtils.IsPointInRectangle(line.endPoint, selRect))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}
