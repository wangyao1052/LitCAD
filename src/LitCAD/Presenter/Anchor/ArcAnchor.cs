using System;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 圆弧锚点
    /// </summary>
    internal class ArcAnchor : EntityAnchor
    {
        /// <summary>
        /// 圆弧
        /// </summary>
        private Arc _arc = null;
        public Arc arc
        {
            get { return _arc; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        internal enum Type
        {
            // 起点
            Start = 1,
            // 终点
            End = 2,
            // 中点
            Middle = 3,
            // 圆心
            Center = 4,
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
            get { return _arc; }
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
                    case Type.Start:
                        return _arc.startPoint;
                    case Type.End:
                        return _arc.endPoint;
                    case Type.Middle:
                        return DBUtils.ArcUtils.ArcMiddlePoint(_arc);
                    case Type.Center:
                        return _arc.center;
                    default:
                        return new LitMath.Vector2(0, 0);
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ArcAnchor(Arc arc, Type type)
        {
            _arc = arc;
            _type = type;
        }
    }
}
