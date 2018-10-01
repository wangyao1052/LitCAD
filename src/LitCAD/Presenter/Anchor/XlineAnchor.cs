using System;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 构造线锚点
    /// </summary>
    internal class XlineAnchor : EntityAnchor
    {
        /// <summary>
        /// 图元: 构造线
        /// </summary>
        private  Xline _xline = null;
        public Xline xline
        {
            get { return _xline; }
        }

        /// <summary>
        /// 图元
        /// </summary>
        public override Entity entity
        {
            get { return _xline; }
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
            // 反向方向点
            DirectionPointMinus = 3,
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
                        return _xline.basePoint;

                    case Type.DirectionPoint:
                        return _xline.basePoint + 10 * _xline.direction;

                    case Type.DirectionPointMinus:
                        return _xline.basePoint - 10 * _xline.direction;

                    default:
                        return _xline.basePoint;
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        internal XlineAnchor(Xline xline, Type type)
        {
            _xline = xline;
            _type = type;
        }
    }
}
