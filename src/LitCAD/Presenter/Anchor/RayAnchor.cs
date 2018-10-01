using System;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 射线锚点
    /// </summary>
    internal class RayAnchor : EntityAnchor
    {
        /// <summary>
        /// 图元: 射线
        /// </summary>
        private Ray _ray = null;
        public Ray ray
        {
            get { return _ray; }
        }

        /// <summary>
        /// 图元
        /// </summary>
        public override Entity entity
        {
            get { return _ray; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        internal enum Type
        {
            // 基点
            BasePoint = 1,
            // 方向点
            DirectionPoint = 2,
        }
        private Type _type;
        public Type type
        {
            get { return _type; }
        }

        /// <summary>
        /// 位置
        /// </summary>
        public override LitMath.Vector2 position
        {
            get
            {
                switch (_type)
                {
                    case Type.BasePoint:
                        return _ray.basePoint;

                    case Type.DirectionPoint:
                        return _ray.basePoint + 10 * _ray.direction;

                    default:
                        return _ray.basePoint;
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        internal RayAnchor(Ray ray, Type type)
        {
            _ray = ray;
            _type = type;
        }
    }
}
