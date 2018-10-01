using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Anchor
{
    /// <summary>
    /// 多段线锚点命令
    /// </summary>
    internal class PolylineAnchorCmd : AnchorCmd
    {
        /// <summary>
        /// 锚点
        /// </summary>
        private PolylineAnchor _polylineAnchor;
        protected override EntityAnchor anchor
        {
            get { return _polylineAnchor; }
        }

        /// <summary>
        /// 图元拷贝
        /// </summary>
        private Polyline _originalPolylineCopy;
        private Polyline _resultPolylineCopy;

        protected override Entity _originalEntityCopy
        {
            get { return _originalPolylineCopy; }
        }

        protected override Entity _resultEntityCopy
        {
            get { return _resultPolylineCopy; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PolylineAnchorCmd(EntityAnchor polylineAnchor)
        {
            _polylineAnchor = polylineAnchor as PolylineAnchor;
            _originalPolylineCopy = _polylineAnchor.polyline.Clone() as Polyline;
            _resultPolylineCopy = _polylineAnchor.polyline.Clone() as Polyline;
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public override void Undo()
        {
            base.Undo();

            this.OverrideBy(_originalPolylineCopy);
        }

        private void OverrideBy(Polyline refPolyline)
        {
            switch (_polylineAnchor.type)
            {
                case PolylineAnchor.Type.VertexPoint:
                    int vIndex = _polylineAnchor.vertexIndex;
                    _polylineAnchor.polyline.SetPointAt(vIndex, refPolyline.GetPointAt(vIndex));
                    break;

                case PolylineAnchor.Type.MiddlePoint:
                    int vIndex1st = _polylineAnchor.lineIndex;
                    int vIndex2nd = vIndex1st + 1;
                    if (vIndex2nd >= _polylineAnchor.polyline.NumberOfVertices)
                    {
                        vIndex2nd = 0;
                    }
                    _polylineAnchor.polyline.SetPointAt(vIndex1st, refPolyline.GetPointAt(vIndex1st));
                    _polylineAnchor.polyline.SetPointAt(vIndex2nd, refPolyline.GetPointAt(vIndex2nd));
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 重做
        /// </summary>
        public override void Redo()
        {
            base.Redo();

            this.OverrideBy(_resultPolylineCopy);
        }

        /// <summary>
        /// 完成
        /// </summary>
        public override void Finish()
        {
            this.OverrideBy(_resultPolylineCopy);

            base.Finish();
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public override void Cancel()
        {
            base.Cancel();
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        public override EventResult OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            switch (_polylineAnchor.type)
            {
                case PolylineAnchor.Type.VertexPoint:
                    int vIndex = _polylineAnchor.vertexIndex;
                    _resultPolylineCopy.SetPointAt(vIndex, _mousePosInModel);
                    break;
                    
                case PolylineAnchor.Type.MiddlePoint:
                    int vIndex1st = _polylineAnchor.lineIndex;
                    int vIndex2nd = vIndex1st + 1;
                    if (vIndex2nd >= _polylineAnchor.polyline.NumberOfVertices)
                    {
                        vIndex2nd = 0;
                    }
                    LitMath.Vector2 translate = _mousePosInModel - _polylineAnchor.position;
                    _resultPolylineCopy.SetPointAt(vIndex1st, _polylineAnchor.polyline.GetPointAt(vIndex1st) + translate);
                    _resultPolylineCopy.SetPointAt(vIndex2nd, _polylineAnchor.polyline.GetPointAt(vIndex2nd) + translate);
                    break;

                default:
                    break;
            }

            return EventResult.Handled;
        }
    }
}
