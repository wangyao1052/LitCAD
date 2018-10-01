using System;

namespace LitCAD.DatabaseServices
{
    internal class MathUtils
    {
        internal static double NormalizeRadianAngle(double angleInRadian)
        {
            double value = angleInRadian % (2 * Math.PI);
            if (value < 0)
                value += 2 * Math.PI;
            return value;
        }
    }
}
