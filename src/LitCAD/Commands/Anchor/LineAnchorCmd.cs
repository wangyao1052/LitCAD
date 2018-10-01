using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Anchor
{
    /// <summary>
    /// 线段锚点命令
    /// </summary>
    internal class LineAnchorCmd : AnchorCmd
    {
        /// <summary>
        /// 锚点
        /// </summary>
        private LineAnchor _lineAnchor;
        protected override EntityAnchor anchor
        {
            get { return _lineAnchor; }
        }

        /// <summary>
        /// 图元拷贝
        /// </summary>
        private Line _originalLineCopy;
        private Line _resultLineCopy;

        protected override Entity _originalEntityCopy
        {
            get { return _originalLineCopy; }
        }

        protected override Entity _resultEntityCopy
        {
            get { return _resultLineCopy; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LineAnchorCmd(EntityAnchor lineAnchor)
        {
            _lineAnchor = lineAnchor as LineAnchor;
            _originalLineCopy = _lineAnchor.line.Clone() as Line;
            _resultLineCopy = _originalLineCopy.Clone() as Line;
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public override void Undo()
        {
            base.Undo();

            _lineAnchor.line.startPoint = _originalLineCopy.startPoint;
            _lineAnchor.line.endPoint = _originalLineCopy.endPoint;
        }

        /// <summary>
        /// 重做
        /// </summary>
        public override void Redo()
        {
            base.Redo();

            _lineAnchor.line.startPoint = _resultLineCopy.startPoint;
            _lineAnchor.line.endPoint = _resultLineCopy.endPoint;
        }

        /// <summary>
        /// 完成
        /// </summary>
        public override void Finish()
        {
            _lineAnchor.line.startPoint = _resultLineCopy.startPoint;
            _lineAnchor.line.endPoint = _resultLineCopy.endPoint;

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

            switch (_lineAnchor.type)
            {
                case LineAnchor.Type.StartPoint:
                    _resultLineCopy.startPoint = _mousePosInModel;
                    break;

                case LineAnchor.Type.EndPoint:
                    _resultLineCopy.endPoint = _mousePosInModel;
                    break;

                case LineAnchor.Type.MiddlePoint:
                    LitMath.Vector2 translate = _mousePosInModel - _lineAnchor.position;
                    _resultLineCopy.startPoint = _originalLineCopy.startPoint + translate;
                    _resultLineCopy.endPoint = _originalLineCopy.endPoint + translate;
                    break;
            }

            return EventResult.Handled;
        }
    }
}
