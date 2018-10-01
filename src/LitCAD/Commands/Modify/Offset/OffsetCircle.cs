using System;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Modify.Offset
{
    internal class OffsetCircle : _OffsetOperation
    {
        private Circle _circle = null;
        private Circle _result = null;

        public override Entity result
        {
            get { return _result; }
        }

        public OffsetCircle(Entity entity)
            : base()
        {
            _circle = entity as Circle;
            if (_circle != null)
            {
                _result = _circle.Clone() as Circle;
            }
        }

        public override bool Do(double value, LitMath.Vector2 refPoint)
        {
            if (_circle == null
                || _result == null)
            {
                return false;
            }

            double dis = (_circle.center - refPoint).length;
            if (dis > _circle.radius)
            {
                _result.radius = _circle.radius + Math.Abs(value);
            }
            else
            {
                if (_circle.radius <= Math.Abs(value))
                {
                    _result.radius = _circle.radius;
                    return false;
                }
                else
                {
                    _result.radius = _circle.radius - Math.Abs(value);
                }
            }

            return true;
        }
    }
}
