using System;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 文本锚点
    /// </summary>
    internal class TextAnchor : EntityAnchor
    {
        /// <summary>
        /// 图元: 文本
        /// </summary>
        private Text _text = null;
        public Text text
        {
            get { return _text; }
        }

        /// <summary>
        /// 图元
        /// </summary>
        public override Entity entity
        {
            get { return _text; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        internal enum Type
        {
            // 基点
            BasePoint = 1,
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
                        return _text.position;

                    default:
                        return _text.position;
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        internal TextAnchor(Text text, Type type)
        {
            _text = text;
            _type = type;
        }
    }
}
