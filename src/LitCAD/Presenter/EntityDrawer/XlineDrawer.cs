using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class XlineDrawer : EntityDrawer
    {
        internal XlineDrawer(Presenter presenter)
            : base(presenter)
        {
        }

        internal override void Draw(Graphics graphics, Entity entity, Pen pen)
        {
            if (pen == null)
            {
                pen = this.GetPen(entity);
            }

            Xline xline = entity as Xline;
            LitMath.Vector2 basePnt = _presenter.ModelToCanvas(xline.basePoint);
            LitMath.Vector2 otherPnt = _presenter.ModelToCanvas(xline.basePoint + xline.direction);
            LitMath.Vector2 dir = (otherPnt - basePnt).normalized;

            double xk = double.MinValue;
            double yk = double.MinValue;
            if (dir.y != 0)
            {
                double k = basePnt.y / dir.y;
                xk = basePnt.x - k * dir.x;
            }
            if (dir.x != 0)
            {
                double k = basePnt.x / dir.x;
                yk = basePnt.y - k * dir.y;
            }

            if (xk > 0
                || (xk == 0 && dir.x * dir.y >= 0))
            {
                LitMath.Vector2 spnt = new LitMath.Vector2(xk, 0);
                if (dir.y < 0)
                {
                    dir = -dir;
                }

                _presenter.DrawLine(graphics, pen,
                    spnt, spnt + 10000 * dir,
                    CSYS.Canvas);
            }
            else if (yk > 0
                || (yk == 0 && dir.x * dir.y >= 0))
            {
                LitMath.Vector2 spnt = new LitMath.Vector2(0, yk);
                if (dir.x < 0)
                {
                    dir = -dir;
                }

                _presenter.DrawLine(graphics, pen,
                    spnt, spnt + 10000 * dir,
                    CSYS.Canvas);
            }
        }
    }
}
