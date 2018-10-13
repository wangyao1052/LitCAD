using System;
using System.Windows.Forms;
using System.Drawing;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands
{
    /// <summary>
    /// 夹点移动命令
    /// </summary>
    internal class GripPointMoveCmd : Command
    {
        /// <summary>
        /// 夹点
        /// </summary>
        protected GripPoint _gripPoint = null;
        protected int _index = -1;

        protected LitMath.Vector2 _originalGripPos;
        protected LitMath.Vector2 _resultGripPos;

        /// <summary>
        /// 图元
        /// </summary>
        protected Entity _entity = null;
        protected Entity _entityCopy = null;

        /// <summary>
        /// 鼠标位置(世界坐标系)
        /// </summary>
        protected LitMath.Vector2 _mousePosInWorld;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GripPointMoveCmd(Entity entity, int index, GripPoint gripPoint)
        {
            _entity = entity;
            _entityCopy = entity.Clone() as Entity;
            _index = index;
            _gripPoint = gripPoint;

            _originalGripPos = _gripPoint.position;
            _resultGripPos = _gripPoint.position;
            _mousePosInWorld = _gripPoint.position;
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
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public override void Undo()
        {
            base.Undo();
            _entity.SetGripPointAt(_index, _gripPoint, _originalGripPos);
        }

        /// <summary>
        /// 重做
        /// </summary>
        public override void Redo()
        {
            base.Redo();
            _entity.SetGripPointAt(_index, _gripPoint, _resultGripPos);
        }

        /// <summary>
        /// 完成
        /// </summary>
        public override void Finish()
        {
            _resultGripPos = _mousePosInWorld;
            _entity.SetGripPointAt(_index, _gripPoint, _resultGripPos);
            pointer.UpdateGripPoints();
            base.Finish();
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public override void Cancel()
        {
            base.Cancel();
        }

        public override EventResult OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mousePosInWorld = this.pointer.currentSnapPoint;
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
            _mousePosInWorld = this.pointer.currentSnapPoint;
            _entityCopy.SetGripPointAt(_index, _gripPoint, _mousePosInWorld);
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
            //this.anchor.OnDraw(_mgr.presenter, g, Color.Red);
            this.DrawPath(g);
            _mgr.presenter.DrawEntity(g, _entityCopy);
        }

        protected virtual void DrawPath(Graphics g)
        {
            _mgr.presenter.DrawLine(g, new Pen(Color.White), _originalGripPos, _mousePosInWorld, CSYS.Model);
        }
    }
}
