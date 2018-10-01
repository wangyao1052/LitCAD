using System;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Modify.Offset
{
    internal class OffsetArc : _OffsetOperation
    {
        private Arc _arc = null;
        private Arc _result = null;

        public override Entity result
        {
            get { return _result; }
        }

        public OffsetArc(Entity entity)
            : base()
        {
            _arc = entity as Arc;
            if (_arc != null)
            {
                _result = _arc.Clone() as Arc;
            }
        }

        public override bool Do(double value, LitMath.Vector2 refPoint)
        {
            if (_arc == null
                || _result == null)
            {
                return false;
            }

            double dis = (_arc.center - refPoint).length;
            if (dis > _arc.radius)
            {
                _result.radius = _arc.radius + Math.Abs(value);
            }
            else
            {
                if (_arc.radius <= Math.Abs(value))
                {
                    _result.radius = _arc.radius;
                    return false;
                }
                else
                {
                    _result.radius = _arc.radius - Math.Abs(value);
                }
            }

            return true;
        }
    }
}
