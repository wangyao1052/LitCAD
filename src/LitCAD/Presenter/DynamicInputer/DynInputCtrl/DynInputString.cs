using System;

namespace LitCAD.UI
{
    /// <summary>
    /// 命令输入控件
    /// </summary>
    internal class DynInputString : DynInputTextBoxOne<string>
    {
        /// <summary>
        /// 更新值
        /// </summary>
        protected override bool UpdateValue()
        {
            _value = _textBox.Text;
            return true;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DynInputString(Presenter presenter, string value)
            : base(presenter, value)
        {
        }
    }
}
