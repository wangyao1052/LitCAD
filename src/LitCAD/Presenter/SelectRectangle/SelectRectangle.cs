using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;
using LitCAD.ApplicationServices;

namespace LitCAD
{
    /// <summary>
    /// 选择矩形
    /// Canvas坐标
    /// </summary>
    internal class SelectRectangle
    {
        private Presenter _presenter = null;

        /// <summary>
        /// 选择模式
        /// </summary>
        internal enum SelectMode
        {
            // 闭合框选,物体完全在选择矩形框内则选中物体
            Window = 1,
            // 交叉框选,物体与选择矩形框有交集则选中物体
            Cross = 2,
        }

        /// <summary>
        /// 矩形对角起点
        /// Canvas CSYS
        /// </summary>
        private LitMath.Vector2 _startPoint = new LitMath.Vector2(0, 0);
        internal LitMath.Vector2 startPoint
        {
            get { return _startPoint; }
            set { _startPoint = value; }
        }

        /// <summary>
        /// 矩形对角终点
        /// Canvas CSYS
        /// </summary>
        private LitMath.Vector2 _endPoint = new LitMath.Vector2(0, 0);
        internal LitMath.Vector2 endPoint
        {
            get { return _endPoint; }
            set { _endPoint = value; }
        }

        /// <summary>
        /// 选择模式
        /// </summary>
        internal SelectMode selectMode
        {
            get
            {
                if (_endPoint.x >= _startPoint.x)
                {
                    return SelectMode.Window;
                }
                else
                {
                    return SelectMode.Cross;
                }
            }
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        internal SelectRectangle(Presenter presenter)
        {
            _presenter = presenter;
        }

        /// <summary>
        /// 绘制
        /// </summary> 
        internal void OnPaint(Graphics graphics)
        {
            IntPtr pen;
            if (this.selectMode == SelectRectangle.SelectMode.Window)
            {
                pen = GDIResMgr.Instance.selectWindowPen;
            }
            else
            {
                pen = GDIResMgr.Instance.selectCrossPen;
            }

            XorGDI.DrawRectangle(graphics, pen, _startPoint, _endPoint);
        }

        /// <summary>
        /// 选择
        /// </summary>
        internal List<Selection> Select(Block block)
        {
            Bounding selectBound = new Bounding(
                _presenter.CanvasToModel(_startPoint),
                _presenter.CanvasToModel(_endPoint));

            List<Selection> sels = new List<Selection>();
            SelectMode selMode = this.selectMode;
            foreach (Entity entity in block)
            {
                bool selected = false;
                if (selMode == SelectMode.Cross)
                {
                    selected = EntityRSMgr.Instance.Cross(selectBound, entity);
                }
                else if (selMode == SelectMode.Window)
                {
                    selected = EntityRSMgr.Instance.Window(selectBound, entity);
                }

                if (selected)
                {
                    Selection selection = new Selection();
                    selection.objectId = entity.id;
                    sels.Add(selection);
                }
            }

            return sels;
        }

        //private bool IsLineIn(IPresenter presenter, Line line, SelectMode selMode)
        //{
        //    if (selMode == SelectMode.Window)
        //    {
        //        LitMath.Vector2 pnt1 = presenter.CanvasToModel(_startPoint);
        //        LitMath.Vector2 pnt2 = presenter.CanvasToModel(_endPoint);
        //        LitMath.Rectangle2 selRect = new LitMath.Rectangle2(pnt1, pnt2);

        //        if (MathUtils.IsPointInRectangle(line.startPoint, selRect)
        //            && MathUtils.IsPointInRectangle(line.endPoint, selRect))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else if (selMode == SelectMode.Cross)
        //    {
        //        LitMath.Vector2 pnt1 = presenter.CanvasToModel(_startPoint);
        //        LitMath.Vector2 pnt2 = presenter.CanvasToModel(_endPoint);

        //        Bounding selectBound = new Bounding(pnt1, pnt2);
        //        Bounding lineBound = line.bounding;
        //        if (selectBound.Contains(lineBound))
        //        {
        //            return true;
        //        }

        //        LitMath.Rectangle2 selRect = new LitMath.Rectangle2(pnt1, pnt2);

        //        LitMath.Line2 rectLine1 = new LitMath.Line2(selRect.leftBottom, selRect.leftTop);
        //        LitMath.Line2 rectLine2 = new LitMath.Line2(selRect.leftTop, selRect.rightTop);
        //        LitMath.Line2 rectLine3 = new LitMath.Line2(selRect.rightTop, selRect.rightBottom);
        //        LitMath.Line2 rectLine4 = new LitMath.Line2(selRect.rightBottom, selRect.leftBottom);
        //        LitMath.Line2 line2 = new LitMath.Line2(line.startPoint, line.endPoint);

        //        LitMath.Vector2 intersection = new LitMath.Vector2();
        //        if (LitMath.Line2.Intersect(rectLine1, line2, ref intersection)
        //            || LitMath.Line2.Intersect(rectLine2, line2, ref intersection)
        //            || LitMath.Line2.Intersect(rectLine3, line2, ref intersection)
        //            || LitMath.Line2.Intersect(rectLine4, line2, ref intersection))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //private bool IsCircleIn(IPresenter presenter, Circle circle, SelectMode selMode)
        //{
        //    if (selMode == SelectMode.Window)
        //    {
        //        LitMath.Vector2 pnt1 = presenter.CanvasToModel(_startPoint);
        //        LitMath.Vector2 pnt2 = presenter.CanvasToModel(_endPoint);
        //        Bounding selectBound = new Bounding(pnt1, pnt2);
        //        Bounding circleBound = circle.bounding;

        //        if (selectBound.Contains(circleBound))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else if (selMode == SelectMode.Cross)
        //    {
        //        LitMath.Vector2 pnt1 = presenter.CanvasToModel(_startPoint);
        //        LitMath.Vector2 pnt2 = presenter.CanvasToModel(_endPoint);
        //        Bounding selectBound = new Bounding(pnt1, pnt2);

        //        return MathUtils.BoundingCross(selectBound, circle);
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //private bool IsPolylineIn(IPresenter presenter, Polyline polyline, SelectMode selMode)
        //{
        //    switch (selMode)
        //    {
        //        case SelectMode.Window:
        //            {
        //                LitMath.Vector2 pnt1 = presenter.CanvasToModel(_startPoint);
        //                LitMath.Vector2 pnt2 = presenter.CanvasToModel(_endPoint);
        //                Bounding selectBound = new Bounding(pnt1, pnt2);
        //                Bounding polylineBound = polyline.bounding;

        //                if (selectBound.Contains(polylineBound))
        //                {
        //                    return true;
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            }

        //        case SelectMode.Cross:
        //            {
        //                LitMath.Vector2 pnt1 = presenter.CanvasToModel(_startPoint);
        //                LitMath.Vector2 pnt2 = presenter.CanvasToModel(_endPoint);
        //                Bounding selectBound = new Bounding(pnt1, pnt2);
        //                Bounding polylineBound = polyline.bounding;

        //                if (selectBound.Contains(polylineBound))
        //                {
        //                    return true;
        //                }

        //                LitMath.Rectangle2 selRect = new LitMath.Rectangle2(pnt1, pnt2);
        //                LitMath.Line2 rectLine1 = new LitMath.Line2(selRect.leftBottom, selRect.leftTop);
        //                LitMath.Line2 rectLine2 = new LitMath.Line2(selRect.leftTop, selRect.rightTop);
        //                LitMath.Line2 rectLine3 = new LitMath.Line2(selRect.rightTop, selRect.rightBottom);
        //                LitMath.Line2 rectLine4 = new LitMath.Line2(selRect.rightBottom, selRect.leftBottom);

        //                for (int i = 1; i < polyline.NumberOfVertices; ++i)
        //                {
        //                    LitMath.Vector2 spnt = polyline.GetPointAt(i - 1);
        //                    LitMath.Vector2 epnt = polyline.GetPointAt(i);
        //                    LitMath.Line2 line2 = new LitMath.Line2(spnt, epnt);
        //                    LitMath.Vector2 intersection = new LitMath.Vector2();
        //                    if (LitMath.Line2.Intersect(rectLine1, line2, ref intersection)
        //                        || LitMath.Line2.Intersect(rectLine2, line2, ref intersection)
        //                        || LitMath.Line2.Intersect(rectLine3, line2, ref intersection)
        //                        || LitMath.Line2.Intersect(rectLine4, line2, ref intersection))
        //                    {
        //                        return true;
        //                    }
        //                }

        //                return false;
        //            }

        //        default:
        //            return false;
        //    }
        //}
    }
}
