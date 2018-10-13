using System;

namespace LitCAD.DatabaseServices
{
    /// <summary>
    /// 夹点
    /// </summary>
    public class GripPoint
    {
        /// <summary>
        /// 夹点类型
        /// </summary>
        public GripPointType type
        {
            get { return _type; }
            set { _type = value; }
        }
        private GripPointType _type = GripPointType.Undefined;

        /// <summary>
        /// 位置
        /// </summary>
        public LitMath.Vector2 position
        {
            get { return _position; }
            set { _position = value; }
        }
        private LitMath.Vector2 _position = new LitMath.Vector2(0, 0);

        /// <summary>
        /// 扩展数据1
        /// </summary>
        public object xData1
        {
            get { return _xdata1; }
            set { _xdata1 = value; }
        }
        private object _xdata1 = null;

        /// <summary>
        /// 扩展数据2
        /// </summary>
        public object xData2
        {
            get { return _xdata2; }
            set { _xdata2 = value; }
        }
        private object _xdata2 = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GripPoint(GripPointType type, LitMath.Vector2 position)
        {
            _type = type;
            _position = position;
        }
    }
}
