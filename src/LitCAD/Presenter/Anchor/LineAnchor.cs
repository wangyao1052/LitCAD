using System;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 线段锚点
    /// </summary>
    internal class LineAnchor : EntityAnchor
    {
        /// <summary>
        /// 图元: 线段
        /// </summary>
        private Line _line = null;
        public Line line
        {
            get { return _line; }
        }

        /// <summary>
        /// 图元
        /// </summary>
        public override Entity entity
        {
            get { return _line; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        internal enum Type
        {
            // 起点
            StartPoint = 1,
            // 终点
            EndPoint = 2,
            // 中点
            MiddlePoint = 3,
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
                if (_type == Type.StartPoint)
                {
                    return _line.startPoint;
                }
                else if (_type == Type.EndPoint)
                {
                    return _line.endPoint;
                }
                else if (_type == Type.MiddlePoint)
                {
                    return (_line.startPoint + _line.endPoint) / 2;
                }
                else
                {
                    return new LitMath.Vector2(0, 0);
                }
            }
        }

        internal LineAnchor(Line line, Type type)
        {
            _line = line;
            _type = type;
        }
    }
}
