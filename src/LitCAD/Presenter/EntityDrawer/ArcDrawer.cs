using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class ArcDrawer : EntityDrawer
    {
        internal ArcDrawer(Presenter presenter)
            : base(presenter)
        {
        }

        internal override void Draw(Graphics graphics, Entity entity, Pen pen)
        {
            if (pen == null)
            {
                pen = this.GetPen(entity);
            }

            Arc arc = entity as Arc;
            if (arc != null && arc.radius > 0)
            {
                _presenter.DrawArc(graphics, pen,
                    arc.center, arc.radius,
                    arc.startAngle * 180.0 / Math.PI, arc.endAngle * 180.0 / Math.PI,
                    CSYS.Model);
            }
        }
    }
}
