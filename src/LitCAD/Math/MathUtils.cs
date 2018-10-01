using System;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal static class MathUtils
    {
        /// <summary>
        /// 点是否在矩形内
        /// </summary>
        internal static bool IsPointInRectangle(LitMath.Vector2 point, LitMath.Rectangle2 rect)
        {
            LitMath.Vector2 rectLeftBottom = rect.leftBottom;
            LitMath.Vector2 rectRightTop = rect.rightTop;

            if (point.x >= rectLeftBottom.x
                && point.x <= rectRightTop.x
                && point.y >= rectLeftBottom.y
                && point.y <= rectRightTop.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Cross window
        /// https://yal.cc/rectangle-circle-intersection-test/
        /// </summary>
        internal static bool BoundingCross(Bounding bounding, Circle circle)
        {
            LitMath.Vector2 nearestPntOnBound = new LitMath.Vector2(
                Math.Max(bounding.left, Math.Min(circle.center.x, bounding.right)),
                Math.Max(bounding.bottom, Math.Min(circle.center.y, bounding.top)));

            if (LitMath.Vector2.Distance(nearestPntOnBound, circle.center) <= circle.radius)
            {
                double bdLeft = bounding.left;
                double bdRight = bounding.right;
                double bdTop = bounding.top;
                double bdBottom = bounding.bottom;

                return LitMath.Vector2.Distance(new LitMath.Vector2(bdLeft, bdTop), circle.center) >= circle.radius
                    || LitMath.Vector2.Distance(new LitMath.Vector2(bdLeft, bdBottom), circle.center) >= circle.radius
                    || LitMath.Vector2.Distance(new LitMath.Vector2(bdRight, bdTop), circle.center) >= circle.radius
                    || LitMath.Vector2.Distance(new LitMath.Vector2(bdRight, bdBottom), circle.center) >= circle.radius;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 值是否在范围内
        /// </summary>
        internal static bool IsValueInRange(double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 规整化弧度
        /// 返回值范围:[0, 2*PI)
        /// </summary>
        internal static double NormalizeRadianAngle(double rad)
        {
            double value = rad % (2 * LitMath.Utils.PI);
            if (value < 0)
                value += 2 * LitMath.Utils.PI;
            return value;
        }

        /// <summary>
        /// 镜像矩阵
        /// </summary>
        internal static LitMath.Matrix3 MirrorMatrix(LitMath.Line2 mirrorLine)
        {
            LitMath.Vector2 lineDir = mirrorLine.direction;
            LitMath.Matrix3 matPos1 = LitMath.Matrix3.Translate(-mirrorLine.startPoint);
            double rotAngle = LitMath.Vector2.SignedAngle(lineDir, new LitMath.Vector2(1, 0));
            LitMath.Matrix3 matRot1 = LitMath.Matrix3.Rotate(rotAngle);

            LitMath.Matrix3 mirrorMatX = new LitMath.Matrix3(
                1,  0, 0,
                0, -1, 0,
                0,  0, 1);

            LitMath.Matrix3 matRot2 = LitMath.Matrix3.Rotate(-rotAngle);
            LitMath.Matrix3 matPos2 = LitMath.Matrix3.Translate(mirrorLine.startPoint);

            return matPos2 * matRot2 * mirrorMatX * matRot1 * matPos1;
        }
    }
}
