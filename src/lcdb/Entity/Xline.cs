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
    }
}
