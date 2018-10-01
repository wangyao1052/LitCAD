using System;
using System.Collections.Generic;
using System.Drawing;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 多段线锚点
    /// </summary>
    internal class PolylineAnchor : EntityAnchor
    {
        /// <summary>
        /// 多段线
        /// </summary>
        private Polyline _polyline = null;
        public Polyline polyline
        {
            get { return _polyline; }
        }

        private int _vertexIndex = -1;
        public int vertexIndex
        {
            get { return _vertexIndex; }
        }

        private int _lineIndex = -1;
        public int lineIndex
        {
            get { return _lineIndex; }
        }

        /// <summary>
        /// 图元
        /// </summary>
        public override Entity entity
        {
            get { return _polyline; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        internal enum Type
        {
            // 节点
            VertexPoint = 0,
            // 中点
            MiddlePoint = 1,
        }
        private Type _type;
        public Type type
        {
            get { return _type; }
        }

        /// <summary>
        /// 位置
        /// </summary>
        public override LitMath.Vector2 position
        {
            get
            {
                switch (_type)
                {
                    case Type.VertexPoint:
                        return _polyline.GetPointAt(_vertexIndex);

                    case Type.MiddlePoint:
                        if (_lineIndex + 1 < _polyline.NumberOfVertices)
                        {
                            return (_polyline.GetPointAt(_lineIndex) + _polyline.GetPointAt(_lineIndex + 1)) / 2;
                        }
                        else
                        {
                            return (_polyline.GetPointAt(_lineIndex) + _polyline.GetPointAt(0)) / 2;
                        }

                    default:
                        return new LitMath.Vector2(0, 0);
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PolylineAnchor(Polyline polyline, Type type, int vertexIndex = -1, int lineIndex = -1)
        {
            _polyline = polyline;
            _type = type;
            _vertexIndex = vertexIndex;
            _lineIndex = lineIndex;
        }

        public override void OnDraw(Presenter presenter, Graphics g, Color color)
        {
            switch (_type)
            {
                case Type.VertexPoint:
                    base.OnDraw(presenter, g, color);
                    return;

                case Type.MiddlePoint:
                    LitMath.Vector2 pnt1InCanvas = presenter.ModelToCanvas(_polyline.GetPointAt(_lineIndex));
                    LitMath.Vector2 pnt2InCanvas = presenter.ModelToCanvas(_polyline.GetPointAt(
                        (_lineIndex + 1 < _polyline.NumberOfVertices) ? _lineIndex + 1 : 0));
                    LitMath.Vector2 dirInCanvas = (pnt2InCanvas - pnt1InCanvas).normalized;
                    LitMath.Vector2 nInCanvas = new LitMath.Vector2(-dirInCanvas.y, dirInCanvas.x);

                    double length = 13;
                    LitMath.Vector2 anchorPosInCanvas = presenter.ModelToCanvas(this.position);
                    LitMath.Vector2 pntA = anchorPosInCanvas + dirInCanvas * length / 2;
                    LitMath.Vector2 pntB = anchorPosInCanvas - dirInCanvas * length / 2;
                    double width = 4;

                    List<LitMath.Vector2> pnts = new List<LitMath.Vector2>();
                    pnts.Add(pntA + nInCanvas * width / 2);
                    pnts.Add(pntB + nInCanvas * width / 2);
                    pnts.Add(pntB - nInCanvas * width / 2);
                    pnts.Add(pntA - nInCanvas * width / 2);

                    presenter.FillPolygon(g, GDIResMgr.Instance.GetBrush(color), pnts, CSYS.Canvas);
                    return;
            }
        }
    }
}
