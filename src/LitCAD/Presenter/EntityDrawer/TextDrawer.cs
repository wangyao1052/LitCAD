using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class TextDrawer : EntityDrawer
    {
        internal TextDrawer(Presenter presenter)
            : base(presenter)
        {
        }

        internal override void Draw(Graphics graphics, Entity entity, Pen pen)
        {
            Text text = entity as Text;
            if (text == null)
            {
                return;
            }

            Brush brush = null;
            if (pen == null)
            {
                pen = this.GetPen(entity);
                brush = new SolidBrush(pen.Color);
            }
            else
            {
                brush = new HatchBrush(HatchStyle.DiagonalCross,
                  Color.FromArgb(33, 40, 48), pen.Color);
            }

            string fontFamily = text.font == "" ? "Arial" : text.font;

            LitMath.Vector2 size = _presenter.DrawString(
                graphics, brush, text.text, fontFamily, text.height, text.alignment,
                text.position, CSYS.Model);
            text.UpdateBounding(size.x, size.y);
        }
    }
}
