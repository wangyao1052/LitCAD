using System;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 圆锚点
    /// </summary>
    internal class CircleAnchor : EntityAnchor
    {
        /// <summary>
        /// 图元:圆
        /// </summary>
        private Circle _circle = null;
        public Circle circle
        {
            get { return _circle; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        internal enum Type
        {
            // 端点: 0度位置
            EndPoint_0 = 1,
            // 端点: 90度位置
            EndPoint_90 = 2,
            // 端点: 180度位置
            EndPoint_180 = 3,
            // 端点: 270度位置
            EndPoint_270 = 4,
            // 中点
            CenterPoint = 5,
        }
        private Type _type;
        public Type type
        {
            get { return _type; }
        }

        /// <summary>
        /// 图元
        /// </summary>
        public override Entity entity
        {
            get { return _circle; }
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
                    case Type.EndPoint_0:
                        return _circle.center + new LitMath.Vector2(_circle.radius, 0);
                    case Type.EndPoint_90:
                        return _circle.center + new LitMath.Vector2(0, _circle.radius);
                    case Type.EndPoint_180:
                        return _circle.center - new LitMath.Vector2(_circle.radius, 0);
                    case Type.EndPoint_270:
                        return _circle.center - new LitMath.Vector2(0, _circle.radius);
                    case Type.CenterPoint:
                        return _circle.center;
                    default:
                        return new LitMath.Vector2(0, 0);
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CircleAnchor(Circle circle, Type type)
        {
            _circle = circle;
            _type = type;
        }
    }
}
