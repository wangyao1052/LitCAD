using System;

using LitCAD.DatabaseServices;

namespace LitCAD.UI
{
    internal class XlineHitter : EntityHitter
    {
        internal override bool Hit(PickupBox pkbox, Entity entity)
        {
            Xline xline = entity as Xline;
            if (xline == null)
                return false;

            Bounding bounding = pkbox.reservedBounding;
            return BoundingIntersectWithXline(bounding, xline);
        }

        internal static bool BoundingIntersectWithXline(Bounding bounding, Xline xline)
        {
            LitMath.Vector2 pkPnt1 = new LitMath.Vector2(bounding.left, bounding.bottom);
            LitMath.Vector2 pkPnt2 = new LitMath.Vector2(bounding.left, bounding.top);
            LitMath.Vector2 pkPnt3 = new LitMath.Vector2(bounding.right, bounding.top);
            LitMath.Vector2 pkPnt4 = new LitMath.Vector2(bounding.right, bounding.bottom);

            double d1 = LitMath.Vector2.Cross(pkPnt1 - xline.basePoint, xline.direction);
            double d2 = LitMath.Vector2.Cross(pkPnt2 - xline.basePoint, xline.direction);
            double d3 = LitMath.Vector2.Cross(pkPnt3 - xline.basePoint, xline.direction);
            double d4 = LitMath.Vector2.Cross(pkPnt4 - xline.basePoint, xline.direction);

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
