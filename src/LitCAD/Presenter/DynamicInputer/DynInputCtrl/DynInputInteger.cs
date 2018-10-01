using System;

namespace LitCAD.UI
{
    internal class DynInputInteger : DynInputTextBoxOne<int>
    {
        /// <summary>
        /// 更新值
        /// </summary>
        protected override bool UpdateValue()
        {
            return int.TryParse(_textBox.Text, out _value);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DynInputInteger(Presenter presenter, int value)
            : base(presenter, value.ToString())
        {
        }
    }
}
