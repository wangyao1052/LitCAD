using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Anchor
{
    /// <summary>
    /// 圆锚点命令
    /// </summary>
    internal class CircleAnchorCmd : AnchorCmd
    {
        /// <summary>
        /// 锚点
        /// </summary>
        private CircleAnchor _circleAnchor;
        protected override EntityAnchor anchor
        {
            get { return _circleAnchor; }
        }

        /// <summary>
        /// 图元拷贝
        /// </summary>
        private Circle _originalCircleCopy;
        private Circle _resultCircleCopy;

        protected override Entity _originalEntityCopy
        {
            get { return _originalCircleCopy; }
        }

        protected override Entity _resultEntityCopy
        {
            get { return _resultCircleCopy; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CircleAnchorCmd(EntityAnchor circleAnchor)
        {
            _circleAnchor = circleAnchor as CircleAnchor;
            _originalCircleCopy = _circleAnchor.circle.Clone() as Circle;
            _resultCircleCopy = _originalCircleCopy.Clone() as Circle;
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public override void Undo()
        {
            base.Undo();

            _circleAnchor.circle.center = _originalCircleCopy.center;
            _circleAnchor.circle.radius = _originalCircleCopy.radius;
        }

        /// <summary>
        /// 重做
        /// </summary>
        public override void Redo()
        {
            base.Redo();

            _circleAnchor.circle.center = _resultCircleCopy.center;
            _circleAnchor.circle.radius = _resultCircleCopy.radius;
        }

        /// <summary>
        /// 完成
        /// </summary>
        public override void Finish()
        {
            _circleAnchor.circle.center = _resultCircleCopy.center;
            _circleAnchor.circle.radius = _resultCircleCopy.radius;

            base.Finish();
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public override void Cancel()
        {
            base.Cancel();
        }

        public override EventResult OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            switch (_circleAnchor.type)
            {
                case CircleAnchor.Type.EndPoint_0:
                case CircleAnchor.Type.EndPoint_90:
                case CircleAnchor.Type.EndPoint_180:
                case CircleAnchor.Type.EndPoint_270:
                    _resultCircleCopy.radius = (_mousePosInModel - _resultCircleCopy.center).length;
                    break;

                case CircleAnchor.Type.CenterPoint:
                    _resultCircleCopy.center = _mousePosInModel;
                    break;
            }

            return EventResult.Handled;
        }

        protected override void DrawPath(Graphics g)
        {
            if (_circleAnchor.type == CircleAnchor.Type.CenterPoint)
            {
                base.DrawPath(g);
            }
        }
    }
}
