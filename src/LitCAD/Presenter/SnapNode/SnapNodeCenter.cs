using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 中心点
    /// </summary>
    internal class SnapNodeCenter : SnapNode
    {
        public SnapNodeCenter(LitMath.Vector2 position)
            : base(position)
        {
        }

        internal override void OnDraw(Presenter presenter, Graphics g)
        {
            LitMath.Vector2 posInCanvas = presenter.ModelToCanvas(_position);
            presenter.DrawCircle(g, this.pen,
                posInCanvas, _threshold, CSYS.Canvas);
        }
    }
}
