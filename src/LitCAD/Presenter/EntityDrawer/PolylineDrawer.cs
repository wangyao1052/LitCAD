using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class PolylineDrawer : EntityDrawer
    {
        internal PolylineDrawer(Presenter presenter)
            : base(presenter)
        {
        }

        internal override void Draw(Graphics graphics, Entity entity, Pen pen)
        {
            if (pen == null)
            {
                pen = this.GetPen(entity);
            }

            Polyline polyline = entity as Polyline;
            int numOfVertices = polyline.NumberOfVertices;
            for (int i = 0; i < numOfVertices - 1; ++i)
            {
                _presenter.DrawLine(graphics, pen,
                    polyline.GetPointAt(i), polyline.GetPointAt(i + 1),
                    CSYS.Model);
            }

            if (polyline.closed
                && numOfVertices > 2)
            {
                _presenter.DrawLine(graphics, pen,
                    polyline.GetPointAt(numOfVertices - 1), polyline.GetPointAt(0),
                    CSYS.Model);
            }
        }
    }
}
