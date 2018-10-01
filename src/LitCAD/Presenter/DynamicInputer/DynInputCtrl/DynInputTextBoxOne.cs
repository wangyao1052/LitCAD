using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.Windows.Controls;

namespace LitCAD.UI
{
    internal abstract class DynInputTextBoxOne<T> : DynInputCtrl
    {
        protected DynamicInputTextBox _textBox = null;
        protected T _value;

        public string text
        {
            get { return _textBox.Text; }
            set { _textBox.Text = value; }
        }

        public Size size
        {
            get { return _textBox.Size; }
            set { _textBox.Size = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DynInputTextBoxOne(Presenter presenter, string text)
            : base(presenter)
        {
            _textBox = new DynamicInputTextBox();
            _textBox.Size = new Size(60, 30);
            _textBox.Text = text;
            _textBox.Hide();

            _winControls.Add(_textBox);
            _winControls.Add(_tipLabel);

            _textBox.keyEscDown += this.OnEscDown;
            _textBox.keySpaceDown += this.OnSpaceDown;
            _textBox.keyEnterDown += this.OnEnterDown;
            _textBox.keyTabDown += this.OnTabDown;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        public override void UpdatePosition()
        {
            _textBox.Location = new Point((int)_position.x + 10, (int)_position.y + 10);
            _tipLabel.Location = new Point(_textBox.Location.X, _textBox.Location.Y + _textBox.Size.Height + 10);
        }

        /// <summary>
        /// 更新值
        /// </summary>
        protected abstract bool UpdateValue();

        /// <summary>
        /// 开始
        /// </summary>
        public override void Start()
        {
            base.Start();

            _textBox.Show();
            _textBox.Focus();
            _textBox.Select(_textBox.Text.Length, 0);
        }

        /// <summary>
        /// 完成
        /// </summary>
        public override void Finish()
        {
            if (!UpdateValue())
            {
                return;
            }

            base.Finish();
        }

        /// <summary>
        /// 完成结果
        /// </summary>
        protected override DynInputResult finishResult
        {
            get
            {
                return new DynInputResult<T>(DynInputStatus.OK, _value);
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        public override void Cancel()
        {
            base.Cancel();
        }

        /// <summary>
        /// 取消结果
        /// </summary>
        protected override DynInputResult cancelResult
        {
            get
            {
                return new DynInputResult<T>(DynInputStatus.Cancel, _value);
            }
        }

        /// <summary>
        /// ESC 键响应
        /// </summary>
        protected virtual void OnEscDown(object sender)
        {
            this.Cancel();
        }

        /// <summary>
        /// Space 键响应
        /// </summary>
        protected virtual void OnSpaceDown(object sender)
        {
            this.Finish();
        }

        /// <summary>
        /// Enter 键响应
        /// </summary>
        protected virtual void OnEnterDown(object sender)
        {
            this.Finish();
        }

        /// <summary>
        /// Tab 键响应
        /// </summary>
        protected virtual void OnTabDown(object sender)
        {
        }
    }
}
