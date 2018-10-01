using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using db = LitCAD.DatabaseServices;

namespace LitCAD.Commands.Text
{
    /// <summary>
    /// 文本编辑命令
    /// </summary>
    internal class EditCmd : Command
    {
        /// <summary>
        /// 文本
        /// </summary>
        private db.Text _text = null;
        private db.Text _original = null;
        private db.Text _result = null;

        internal db.Text text
        {
            set
            {
                _text = value;
                _original = _text.Clone() as db.Text;
                _result = _text.Clone() as db.Text;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _mgr.presenter.selections.Clear();
            this.pointer.isShowAnchor = false;
        }

        /// <summary>
        /// 结束
        /// </summary>
        public override void Terminate()
        {
            _mgr.presenter.selections.Clear();

            base.Terminate();
        }

        /// <summary>
        /// 提交到数据库
        /// </summary>
        protected override void Commit()
        {
            _text.text = _result.text;
            _text.font = _result.font;
            _text.height = _result.height;
            _text.alignment = _result.alignment;
        }

        /// <summary>
        /// 回滚撤销
        /// </summary>
        protected override void Rollback()
        {
            _text.text = _original.text;
            _text.font = _original.font;
            _text.height = _original.height;
            _text.alignment = _original.alignment;
        }
    }
}
