using System;

namespace LitCAD.DatabaseServices
{
    /// <summary>
    /// 对象捕捉点
    /// </summary>
    public class ObjectSnapPoint
    {
        /// <summary>
        /// 类型
        /// </summary>
        public ObjectSnapMode type
        {
            get { return _type; }
            set { _type = value; }
        }
        private ObjectSnapMode _type = ObjectSnapMode.Undefined;

        /// <summary>
        /// 点坐标
        /// </summary>
        private LitMath.Vector2 _position = new LitMath.Vector2(0, 0);
        public LitMath.Vector2 position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ObjectSnapPoint(ObjectSnapMode type, LitMath.Vector2 pos)
        {
            _type = type;
            _position = pos;
        }
    }
}
