using System;
using System.Drawing;
using System.Windows.Forms;

namespace LitCAD
{
    internal class Origin
    {
        private Presenter _presenter = null;
        private LitMath.Vector2 _origin = new LitMath.Vector2(0, 0);
        private Bitmap _bitmap = null;
        private int _xAxisLength = 50;
        private int _yAxisLength = 50;
        private int _offsetX = 2;
        private int _offsetY = 2;

        internal Origin(Presenter presenter)
        {
            _presenter = presenter;
            _bitmap = new Bitmap(_xAxisLength + 22, _yAxisLength + 22);

            Font font = new Font("Arial", 10);
            Color xColor = Color.FromArgb(127, 0, 0);
            Color yColor = Color.FromArgb(0, 127, 0);

            Graphics graphics = Graphics.FromImage(_bitmap);
            //graphics.Clear(Color.FromArgb(33, 40, 48));
            graphics.Clear(Color.Transparent);

            // X Axis
            int xAxisStartX = _offsetX;
            int xAxisStartY = _bitmap.Height - _offsetY;
            int xAxisEndX = xAxisStartX + _xAxisLength;
            int xAxisEndY = xAxisStartY;
            graphics.DrawLine(GDIResMgr.Instance.GetPen(xColor, 0),
                xAxisStartX, xAxisStartY, xAxisEndX, xAxisEndY);
            graphics.DrawString("X", font, GDIResMgr.Instance.GetBrush(xColor),
                xAxisEndX, xAxisEndY - font.Height);

            // Y Axis
            int yAxisStartX = xAxisStartX;
            int yAxisStartY = xAxisStartY;
            int yAxisEndX = yAxisStartX;
            int yAxisEndY = yAxisStartY - _yAxisLength;
            graphics.DrawLine(GDIResMgr.Instance.GetPen(yColor, 0),
                yAxisStartX, yAxisStartY, yAxisEndX, yAxisEndY);
            graphics.DrawString("Y", font, GDIResMgr.Instance.GetBrush(yColor),
                yAxisEndX, yAxisEndY - font.Height);

            graphics.Dispose();
        }

        
        internal void OnPaint(Graphics graphics)
        {
            LitMath.Vector2 originInCanvas = _presenter.ModelToCanvas(_origin);
            graphics.DrawImage(_bitmap,
                (float)originInCanvas.x - _offsetX, (float)originInCanvas .y - _bitmap.Height + _offsetY,
                new RectangleF(0, 0, _bitmap.Width, _bitmap.Height),
                GraphicsUnit.Pixel);
        }

        //internal void OnPaint(Graphics graphics)
        //{
        //    LitMath.Vector2 originInCanvas = _presenter.ModelToCanvas(new LitMath.Vector2(0, 0));
        //    graphics.DrawLine(GDIResMgr.Instance.GetPen(Color.FromArgb(127, 0, 0), 0),
        //        (float)originInCanvas.x, (float)originInCanvas.y, 50 + (float)originInCanvas.x, (float)originInCanvas.y);
        //    graphics.DrawLine(GDIResMgr.Instance.GetPen(Color.FromArgb(0, 127, 0), 0),
        //        (float)originInCanvas.x, (float)originInCanvas.y, (float)originInCanvas.x, (float)originInCanvas.y - 50);

        //    Font font = new Font("Arial", 10);
        //    graphics.DrawString("X", font, GDIResMgr.Instance.GetBrush(Color.FromArgb(127, 0, 0)),
        //        50 + (float)originInCanvas.x,
        //        (float)originInCanvas.y - font.Height);
        //    graphics.DrawString("Y", font, GDIResMgr.Instance.GetBrush(Color.FromArgb(0, 127, 0)),
        //        (float)originInCanvas.x,
        //        (float)originInCanvas.y - 50 - font.Height);
        //}
    }
}
