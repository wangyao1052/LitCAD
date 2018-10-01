using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using db = LitCAD.DatabaseServices;

namespace LitCAD.Commands.Anchor
{
    /// <summary>
    /// 文本锚点命令
    /// </summary>
    internal class TextAnchorCmd : AnchorCmd
    {
        /// <summary>
        /// 锚点
        /// </summary>
        private TextAnchor _textAnchor;
        protected override EntityAnchor anchor
        {
            get { return _textAnchor; }
        }

        /// <summary>
        /// 图元拷贝
        /// </summary>
        private db.Text _src;
        private db.Text _result;

        protected override db.Entity _originalEntityCopy
        {
            get { return _src; }
        }

        protected override db.Entity _resultEntityCopy
        {
            get { return _result; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TextAnchorCmd(EntityAnchor textAnchor)
        {
            _textAnchor = textAnchor as TextAnchor;
            _src = _textAnchor.text.Clone() as db.Text;
            _result = _src.Clone() as db.Text;
        }

        /// <summary>
        /// 提交到数据库
        /// </summary>
        protected override void Commit()
        {
            _textAnchor.text.position = _result.position;
        }

        /// <summary>
        /// 回滚撤销
        /// </summary>
        protected override void Rollback()
        {
            _textAnchor.text.position = _src.position;
        }

        public override EventResult OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            switch (_textAnchor.type)
            {
                case TextAnchor.Type.BasePoint:
                    _result.position = _mousePosInModel;
                    break;
            }

            return EventResult.Handled;
        }
    }
}
