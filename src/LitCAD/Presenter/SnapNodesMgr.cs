using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 捕捉节点管理器
    /// </summary>
    internal class SnapNodesMgr
    {
        private Presenter _presenter = null;

        private ObjectSnapPoint _currObjectSnapPoint = null;
        internal ObjectSnapPoint currentObjectSnapPoint
        {
            get { return _currObjectSnapPoint; }
        }

        public SnapNodesMgr(Presenter presenter)
        {
            _presenter = presenter;
        }

        public LitMath.Vector2 Snap(double x, double y)
        {
            return this.Snap(new LitMath.Vector2(x, y));
        }

        internal LitMath.Vector2 Snap(LitMath.Vector2 posInCanvas)
        {
            LitMath.Vector2 posInModel = _presenter.CanvasToModel(posInCanvas);

            foreach (Entity entity in _presenter.currentBlock)
            {
                List<ObjectSnapPoint> snapPnts = entity.GetSnapPoints();
                if (snapPnts == null || snapPnts.Count == 0)
                {
                    continue;
                }
                foreach (ObjectSnapPoint snapPnt in snapPnts)
                {
                    double dis = (snapPnt.position - posInModel).length;
                    double disInCanvas = _presenter.ModelToCanvas(dis);
                    if (disInCanvas <= _threshold)
                    {
                        _currObjectSnapPoint = snapPnt;
                        return snapPnt.position;
                    }
                }
            }

            _currObjectSnapPoint = null;
            return posInModel;
        }

        public void Clear()
        {
            _currObjectSnapPoint = null;
        }

        private double _threshold = 8;
        public void OnPaint(IGraphicsDraw canvasDraw)
        {
            if (_currObjectSnapPoint != null)
            {
                CanvasDraw gd = canvasDraw as CanvasDraw;
                Pen pen = GDIResMgr.Instance.GetPen(Color.White, 2);
                gd.pen = pen;
                LitMath.Vector2 posInCanvas = _presenter.ModelToCanvas(_currObjectSnapPoint.position);
                
                switch (_currObjectSnapPoint.type)
                {
                    case ObjectSnapMode.End:
                        {
                            gd.DrawRectangle(new LitMath.Vector2(posInCanvas.x - _threshold, posInCanvas.y - _threshold),
                                _threshold * 2, _threshold * 2);
                        }
                        break;

                    case ObjectSnapMode.Mid:
                        {
                            LitMath.Vector2 offset = new LitMath.Vector2(0, -_threshold * 1.2);
                            LitMath.Vector2 point1 = posInCanvas + offset;
                            offset = LitMath.Vector2.Rotate(offset, 120);
                            LitMath.Vector2 point2 = posInCanvas + offset;
                            offset = LitMath.Vector2.Rotate(offset, 120);
                            LitMath.Vector2 point3 = posInCanvas + offset;

                            gd.DrawLine(point1, point2);
                            gd.DrawLine(point2, point3);
                            gd.DrawLine(point3, point1);
                        }
                        break;

                    case ObjectSnapMode.Center:
                        {
                            gd.DrawCircle(posInCanvas, _threshold);
                        }
                        break;

                    default:
                        {
                            gd.DrawRectangle(new LitMath.Vector2(posInCanvas.x - _threshold, posInCanvas.y - _threshold),
                                _threshold * 2, _threshold * 2);
                        }
                        break;
                }
            }
        }
    }
}
