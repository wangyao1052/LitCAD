using System;
using System.Collections.Generic;

namespace LitCAD.DatabaseServices
{
    /// <summary>
    /// 构造线
    /// </summary>
    public class Xline : Entity
    {
        /// <summary>
        /// 类名
        /// </summary>
        public override string className
        {
            get { return "Xline"; }
        }

        /// <summary>
        /// 基点
        /// </summary>
        private LitMath.Vector2 _basePoint = new LitMath.Vector2(0, 0);
        public LitMath.Vector2 basePoint
        {
            get { return _basePoint; }
            set { _basePoint = value; }
        }

        /// <summary>
        /// 方向
        /// </summary>
        private LitMath.Vector2 _direction = new LitMath.Vector2(1, 0);
        public LitMath.Vector2 direction
        {
            get { return _direction; }
            set { _direction = value.normalized; }
        }

        /// <summary>
        /// 外围边框
        /// </summary>
        public override Bounding bounding
        {
            get
            {
                return new Bounding(
                    new LitMath.Vector2(double.MinValue, double.MinValue),
                    new LitMath.Vector2(double.MaxValue, double.MaxValue));
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Xline()
        {
        }

        public Xline(LitMath.Vector2 basePoint, LitMath.Vector2 direction)
        {
            _basePoint = basePoint;
            _direction = direction;
        }

        /// <summary>
        /// 绘制函数
        /// </summary>
        public override void Draw(IGraphicsDraw gd)
        {
            gd.DrawXLine(_basePoint, _direction);
        }

        /// <summary>
        /// 克隆函数
        /// </summary>
        public override object Clone()
        {
            Xline xline = base.Clone() as Xline;
            xline._basePoint = _basePoint;
            xline._direction = _direction;
            return xline;
        }

        protected override DBObject CreateInstance()
        {
            return new Xline();
        }

        /// <summary>
        /// 平移
        /// </summary>
        public override void Translate(LitMath.Vector2 translation)
        {
            _basePoint += translation;
        }

        /// <summary>
        /// Transform
        /// </summary>
        public override void TransformBy(LitMath.Matrix3 transform)
        {
            LitMath.Vector2 refPnt = _basePoint + _direction;
            _basePoint = transform * _basePoint;
            refPnt = transform * refPnt;
            _direction = (refPnt - _basePoint).normalized;
        }

        /// <summary>
        /// 对象捕捉点
        /// </summary>
        public override List<ObjectSnapPoint> GetSnapPoints()
        {
            return null;
        }

        /// <summary>
        /// 获取夹点
        /// </summary>
        public override List<GripPoint> GetGripPoints()
        {
            List<GripPoint> gripPnts = new List<GripPoint>();
            gripPnts.Add(new GripPoint(GripPointType.End, _basePoint));
            gripPnts.Add(new GripPoint(GripPointType.End, _basePoint + 10 * _direction));
            gripPnts.Add(new GripPoint(GripPointType.End, _basePoint - 10 * _direction));

            return gripPnts;
        }

        /// <summary>
        /// 设置夹点
        /// </summary>
        public override void SetGripPointAt(int index, GripPoint gripPoint, LitMath.Vector2 newPosition)
        {
            if (index == 0)
            {
                _basePoint = newPosition;
            }
            else if (index == 1 || index == 2)
            {
                LitMath.Vector2 dir = (newPosition - _basePoint).normalized;
                if (!dir.Equals(new LitMath.Vector2(0, 0)))
                {
                    _direction = dir;
                }
            }
        }

        /// <summary>
        /// 写XML
        /// </summary>
        public override void XmlOut(Filer.XmlFiler filer)
        {
            base.XmlOut(filer);

            filer.Write("basePoint", _basePoint);
            filer.Write("direction", _direction);
        }

        /// <summary>
        /// 读XML
        /// </summary>
        public override void XmlIn(Filer.XmlFiler filer)
        {
            base.XmlIn(filer);

            filer.Read("basePoint", out _basePoint);
            filer.Read("direction", out _direction);
        }
    }
}
