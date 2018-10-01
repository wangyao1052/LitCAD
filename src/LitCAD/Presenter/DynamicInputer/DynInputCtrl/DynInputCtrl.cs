using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LitCAD.UI
{
    /// <summary>
    /// 动态输入控件
    /// </summary>
    internal abstract class DynInputCtrl
    {
        protected Presenter _presenter = null;
        protected List<Control> _winControls = new List<Control>();
        protected Label _tipLabel = null;

        /// <summary>
        /// 独占
        /// </summary>
        private bool _exclusive = false;
        public bool exclusive
        {
            get { return _exclusive; }
            set { _exclusive = value; }
        }

        /// <summary>
        /// 位置
        /// </summary>
        protected LitMath.Vector2 _position = new LitMath.Vector2(0, 0);
        public virtual LitMath.Vector2 position
        {
            get { return _position; }
            set
            {
                _position = value;
                this.UpdatePosition();
            }
        }
        public abstract void UpdatePosition();

        /// <summary>
        /// 提示消息
        /// </summary>
        public string Message
        {
            set { _tipLabel.Text = value; }
        }

        /// <summary>
        /// 事件
        /// </summary>
        public delegate void Handler(DynInputCtrl sender, DynInputResult result);
        public event Handler finish;
        public event Handler cancel;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DynInputCtrl(Presenter presenter)
        {
            _presenter = presenter;

            _tipLabel = new Label();
            _tipLabel.BackColor = Color.FromArgb(153, 153, 153);
            _tipLabel.ForeColor = Color.Black;
            _tipLabel.TextAlign = ContentAlignment.MiddleLeft;
            _tipLabel.Padding = new Padding(3, 3, 3, 3);
            _tipLabel.AutoSize = true;
            _winControls.Add(_tipLabel);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Initialize()
        {
            foreach (Control ctrl in _winControls)
            {
                _presenter.canvas.AddChild(ctrl);
            }
        }

        /// <summary>
        /// 结束
        /// </summary>
        protected virtual void Terminate()
        {
            foreach (Control ctrl in _winControls)
            {
                _presenter.canvas.RemoveChild(ctrl);
            }
        }

        /// <summary>
        /// 开始
        /// </summary>
        public virtual void Start()
        {
            this.Initialize();
        }

        /// <summary>
        /// 完成
        /// </summary>
        public virtual void Finish()
        {
            this.Terminate();

            if (finish != null)
            {
                finish.Invoke(this, this.finishResult);
            }
        }

        protected abstract DynInputResult finishResult
        {
            get;
        }

        /// <summary>
        /// 取消
        /// </summary>
        public virtual void Cancel()
        {
            this.Terminate();

            if (cancel != null)
            {
                cancel.Invoke(this, this.cancelResult);
            }
        }

        protected abstract DynInputResult cancelResult
        {
            get;
        }
    }
}
