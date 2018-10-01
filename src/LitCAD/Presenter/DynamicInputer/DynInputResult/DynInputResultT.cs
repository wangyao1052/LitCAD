using System;

namespace LitCAD.UI
{
    /// <summary>
    /// 动态输入结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DynInputResult<T> : DynInputResult
    {
        /// <summary>
        /// 值
        /// </summary>
        protected T _value;
        public T value
        {
            get { return _value; }
        }

        public DynInputResult(DynInputStatus status, T value)
            : base(status)
        {
            _value = value;
        }
    }
}
