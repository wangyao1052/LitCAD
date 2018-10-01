using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class RayDrawer : EntityDrawer
    {
        internal RayDrawer(Presenter presenter)
            : base(presenter)
        {
        }

        internal override void Draw(Graphics graphics, Entity entity, Pen pen)
        {
            if (pen == null)
            {
                pen = this.GetPen(entity);
            }

            Ray ray = entity as Ray;
            LitMath.Vector2 basePnt = _presenter.ModelToCanvas(ray.basePoint);
            LitMath.Vector2 otherPnt = _presenter.ModelToCanvas(ray.basePoint + ray.direction);
            LitMath.Vector2 dir = (otherPnt - basePnt).normalized;

            double xk = double.MinValue;
            double yk = double.MinValue;
            if (basePnt.x > 0 && basePnt.x < 10000
                && basePnt.y > 0 && basePnt.y < 10000)
            {
                xk = 1;
                yk = 1;
            }
            else
            {
                if (dir.y != 0)
                {
                    double k = -basePnt.y / dir.y;
                    if (k >= 0)
                    {
                        xk = basePnt.x + k * dir.x;
                    }
                }
                if (dir.x != 0)
                {
                    double k = -basePnt.x / dir.x;
                    if (k >= 0)
                    {
                        yk = basePnt.y + k * dir.y;
                    }
                }

            }
            
            if (xk > 0
                || (xk == 0 && dir.x * dir.y >= 0)
                || yk > 0
                || (yk == 0 && dir.x * dir.y >= 0))
            {
                _presenter.DrawLine(graphics, pen,
                    basePnt, basePnt + 10000 * dir,
                    CSYS.Canvas);
            }
        }
    }
}
