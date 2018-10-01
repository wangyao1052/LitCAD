using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Anchor
{
    internal abstract class AnchorCmd : Command
    {
        protected abstract EntityAnchor anchor { get; }
        protected abstract Entity _originalEntityCopy { get; }
        protected abstract Entity _resultEntityCopy { get; }

        /// <summary>
        /// 鼠标位置
        /// </summary>
        protected LitMath.Vector2 _mousePosInModel;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AnchorCmd()
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            //
            this.pointer.isShowAnchor = true;
            this.pointer.mode = UI.Pointer.Mode.Locate;
            _mousePosInModel = anchor.position;
        }

        public override EventResult OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mousePosInModel = this.pointer.currentSnapPoint;
                _mgr.FinishCurrentCommand();
            }
           
            return EventResult.Handled;
        }

        public override EventResult OnMouseUp(MouseEventArgs e)
        {
            return EventResult.Handled;
        }

        public override EventResult OnMouseMove(MouseEventArgs e)
        {
            _mousePosInModel = this.pointer.currentSnapPoint;
            return EventResult.Handled;
        }

        public override EventResult OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _mgr.CancelCurrentCommand();
            }

            return EventResult.Handled;
        }

        public override EventResult OnKeyUp(KeyEventArgs e)
        {
            return EventResult.Handled;
        }

        public override void OnPaint(Graphics g)
        {
            this.anchor.OnDraw(_mgr.presenter, g, Color.Red);
            this.DrawPath(g);
            _mgr.presenter.DrawEntity(g, _resultEntityCopy);
        }

        protected virtual void DrawPath(Graphics g)
        {
            _mgr.presenter.DrawLine(g, new Pen(Color.White), this.anchor.position, _mousePosInModel, CSYS.Model);
        }
    }
}
