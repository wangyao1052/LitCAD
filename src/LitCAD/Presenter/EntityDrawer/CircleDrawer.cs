using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class CircleDrawer : EntityDrawer
    {
        internal CircleDrawer(Presenter presenter)
            : base(presenter)
        {
        }

        internal override void Draw(Graphics graphics, Entity entity, Pen pen)
        {
            if (pen == null)
            {
                pen = this.GetPen(entity);
            }

            Circle circle = entity as Circle;
            _presenter.DrawCircle(graphics, pen,
                circle.center, circle.radius,
                CSYS.Model);
        }
    }
}
