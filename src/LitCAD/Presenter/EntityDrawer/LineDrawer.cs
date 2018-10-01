using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class LineDrawer : EntityDrawer
    {
        internal LineDrawer(Presenter presenter)
            : base(presenter)
        {
        }

        internal override void Draw(Graphics graphics, Entity entity, Pen pen)
        {
            if (pen == null)
            {
                pen = this.GetPen(entity);
            }

            Line line = entity as Line;
            _presenter.DrawLine(graphics, pen,
                line.startPoint, line.endPoint,
                CSYS.Model);
        }
    }
}
