using System;

namespace LitCAD.UI
{
    internal class DynInputDouble : DynInputTextBoxOne<double>
    {
        /// <summary>
        /// 更新值
        /// </summary>
        protected override bool UpdateValue()
        {
            return double.TryParse(_textBox.Text, out _value);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DynInputDouble(Presenter presenter, double value)
            : base(presenter, value.ToString())
        {
        }
    }
}
