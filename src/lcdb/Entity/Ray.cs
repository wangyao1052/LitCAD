using System;
using System.Collections.Generic;

namespace LitCAD.DatabaseServices
{
    /// <summary>
    /// 射线
    /// </summary>
    public class Ray : Entity
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
                double x = 0;
                if (_direction.x == 0)
                {
                    x = _basePoint.x;
                }
                else
                {
                    if (_direction.x > 0)
                    {
                        x = double.MaxValue;
                    }
                    else
                    {
                        x = double.MinValue;
                    }
                }

                double y = 0;
                if (_direction.y == 0)
                {
                    y = _basePoint.y;
                }
                else
                {
                    if (_direction.y > 0)
                    {
                        y = double.MaxValue;
                    }
                    else
                    {
                        y = double.MinValue;
                    }
                }

                return new Bounding(_basePoint, new LitMath.Vector2(x, y));
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Ray()
        {
        }

        public Ray(LitMath.Vector2 basePoint, LitMath.Vector2 direction)
        {
            _basePoint = basePoint;
            _direction = direction.normalized;
        }

        /// <summary>
        /// 绘制函数
        /// </summary>
        public override void Draw(IGraphicsDraw gd)
        {
            gd.DrawRay(_basePoint, _direction);
        }

        /// <summary>
        /// 克隆函数
        /// </summary>
        public override object Clone()
        {
            Ray ray = base.Clone() as Ray;
            ray._basePoint = _basePoint;
            ray._direction = _direction;
            return ray;
        }

        protected override DBObject CreateInstance()
        {
            return new Ray();
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
            List<ObjectSnapPoint> snapPnts = new List<ObjectSnapPoint>();
            snapPnts.Add(new ObjectSnapPoint(ObjectSnapMode.End, _basePoint));

            return snapPnts;
        }
    }
}
