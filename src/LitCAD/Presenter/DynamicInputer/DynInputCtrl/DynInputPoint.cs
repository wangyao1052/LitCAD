using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.Windows.Controls;

namespace LitCAD.UI
{
    internal class DynInputPoint : DynInputCtrl
    {
        /// <summary>
        /// 输入框控件
        /// </summary>
        private DynamicInputTextBox _xTextBox = null;
        private DynamicInputTextBox _yTextBox = null;

        /// <summary>
        /// 值
        /// </summary>
        private LitMath.Vector2 _value = new LitMath.Vector2(0, 0);

        /// <summary>
        /// 更新值
        /// </summary>
        protected virtual bool UpdateValue()
        {
            double x;
            if (!double.TryParse(_xTextBox.Text, out x))
            {
                return false;
            }

            double y;
            if (!double.TryParse(_yTextBox.Text, out y))
            {
                return false;
            }

            _value.x = x;
            _value.y = y;
            return true;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DynInputPoint(Presenter presenter, LitMath.Vector2 point)
            : base(presenter)
        {
            _xTextBox = new DynamicInputTextBox();
            _xTextBox.Size = new Size(60, 30);
            _xTextBox.Text = point.x.ToString();

            _yTextBox = new DynamicInputTextBox();
            _yTextBox.Size = new Size(60, 30);
            _yTextBox.Text = point.y.ToString();

            _winControls.Add(_xTextBox);
            _winControls.Add(_yTextBox);

            _xTextBox.keyEscDown += this.OnEscDown;
            _xTextBox.keySpaceDown += this.OnSpaceDown;
            _xTextBox.keyEnterDown += this.OnEnterDown;
            _xTextBox.keyTabDown += this.OnTabDown;

            _yTextBox.keyEscDown += this.OnEscDown;
            _yTextBox.keySpaceDown += this.OnSpaceDown;
            _yTextBox.keyEnterDown += this.OnEnterDown;
            _yTextBox.keyTabDown += this.OnTabDown;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        public override void UpdatePosition()
        {
            int x = (int)_position.x + 10;
            _xTextBox.Location = new Point(x, (int)_position.y + 5);
            _yTextBox.Location = new Point(x, _xTextBox.Location.Y + _xTextBox.Height + 5);
            _tipLabel.Location = new Point(x, _yTextBox.Location.Y + _yTextBox.Height + 5);
        }

        /// <summary>
        /// 开始
        /// </summary>
        public override void Start()
        {
            base.Start();

            _xTextBox.Focus();
            _xTextBox.Select(_xTextBox.Text.Length, 0);
        }

        /// <summary>
        /// 完成
        /// </summary>
        public override void Finish()
        {
            if (!this.UpdateValue())
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
                return new DynInputResult<LitMath.Vector2>(DynInputStatus.OK, _value);
            }
        }

        /// <summary>
        /// 完成
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
                return new DynInputResult<LitMath.Vector2>(DynInputStatus.Cancel, new LitMath.Vector2(0, 0));
            }
        }

        /// <summary>
        /// ESC 键响应
        /// </summary>
        private void OnEscDown(object sender)
        {
            this.Cancel();
        }

        /// <summary>
        /// Space 键响应
        /// </summary>
        private void OnSpaceDown(object sender)
        {
            this.Finish();
        }

        /// <summary>
        /// Enter 键响应
        /// </summary>
        private void OnEnterDown(object sender)
        {
            this.Finish();
        }

        /// <summary>
        /// Tab 键响应
        /// </summary>
        private void OnTabDown(object sender)
        {
            if (sender == _xTextBox)
            {
                _yTextBox.Focus();
            }
            else if (sender == _yTextBox)
            {
                _xTextBox.Focus();
            }
        }
    }
}
