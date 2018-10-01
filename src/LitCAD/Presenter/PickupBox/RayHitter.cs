using System;

using LitCAD.DatabaseServices;

namespace LitCAD.UI
{
    internal class RayHitter : EntityHitter
    {
        internal override bool Hit(PickupBox pkbox, Entity entity)
        {
            Ray ray = entity as Ray;
            if (ray == null)
                return false;

            Bounding bounding = pkbox.reservedBounding;
            return BoundingIntersectWithRay(bounding, ray);
        }

        internal static bool BoundingIntersectWithRay(Bounding bounding, Ray ray)
        {
            if (!ray.bounding.IntersectWith(bounding))
            {
                return false;
            }

            LitMath.Vector2 pkPnt1 = new LitMath.Vector2(bounding.left, bounding.bottom);
            LitMath.Vector2 pkPnt2 = new LitMath.Vector2(bounding.left, bounding.top);
            LitMath.Vector2 pkPnt3 = new LitMath.Vector2(bounding.right, bounding.top);
            LitMath.Vector2 pkPnt4 = new LitMath.Vector2(bounding.right, bounding.bottom);

            double d1 = LitMath.Vector2.Cross(pkPnt1 - ray.basePoint, ray.direction);
            double d2 = LitMath.Vector2.Cross(pkPnt2 - ray.basePoint, ray.direction);
            double d3 = LitMath.Vector2.Cross(pkPnt3 - ray.basePoint, ray.direction);
            double d4 = LitMath.Vector2.Cross(pkPnt4 - ray.basePoint, ray.direction);

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
