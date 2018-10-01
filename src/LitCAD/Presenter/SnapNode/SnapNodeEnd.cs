using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 端点
    /// </summary>
    internal class SnapNodeEnd : SnapNode
    {
        public SnapNodeEnd(LitMath.Vector2 position)
            : base(position)
        {
        }

        internal override void OnDraw(Presenter presenter, Graphics g)
        {
            LitMath.Vector2 posInCanvas = presenter.ModelToCanvas(_position);

            presenter.DrawRectangle(g, this.pen,
                posInCanvas.x - _threshold, posInCanvas.y - _threshold,
                _threshold * 2, _threshold * 2,
                CSYS.Canvas);
        }
    }
}
