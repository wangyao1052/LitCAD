using System;

using LitCAD.DatabaseServices;

namespace LitCAD.DBUtils
{
    internal class ArcUtils
    {
        public static LitMath.Vector2 ArcMiddlePoint(Arc arc)
        {
            double angle = 0;
            if (arc.endAngle >= arc.startAngle)
            {
                angle = (arc.startAngle + arc.endAngle) / 2;
            }
            else
            {
                angle = (arc.startAngle + arc.endAngle + LitMath.Utils.PI * 2) / 2;
            }
            return arc.center + LitMath.Vector2.RotateInRadian(
                 new LitMath.Vector2(arc.radius, 0), angle);
        }
    }
}
