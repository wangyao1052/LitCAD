using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 中点
    /// </summary>
    internal class SnapNodeMiddle : SnapNode
    {
        public SnapNodeMiddle(LitMath.Vector2 position)
            : base(position)
        {
        }

        internal override void OnDraw(Presenter presenter, Graphics g)
        {
            LitMath.Vector2 posInCanvas = presenter.ModelToCanvas(_position);
            LitMath.Vector2 offset = new LitMath.Vector2(0, -_threshold * 1.2);
            LitMath.Vector2 point1 = posInCanvas + offset;
            offset = LitMath.Vector2.Rotate(offset, 120);
            LitMath.Vector2 point2 = posInCanvas + offset;
            offset = LitMath.Vector2.Rotate(offset, 120);
            LitMath.Vector2 point3 = posInCanvas + offset;

            Pen pen = this.pen;
            presenter.DrawLine(g, this.pen, point1, point2, CSYS.Canvas);
            presenter.DrawLine(g, this.pen, point2, point3, CSYS.Canvas);
            presenter.DrawLine(g, this.pen, point3, point1, CSYS.Canvas);
        }
    }
}
