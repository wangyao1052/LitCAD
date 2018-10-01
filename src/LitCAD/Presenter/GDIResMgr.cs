using System;
using System.Collections.Generic;
using System.Drawing;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// GDI资源管理类
    /// </summary>
    internal class GDIResMgr
    {
        private Pen _pen = null;
        private Pen _entitySelectedPen = null;
        private SolidBrush _brush = null;

        /// <summary>
        /// 选择矩形画笔
        /// </summary>
        private IntPtr _selectWindowPen = IntPtr.Zero;
        private IntPtr _selectCrossPen = IntPtr.Zero;

        internal IntPtr selectWindowPen
        {
            get
            {
                if (_selectWindowPen == IntPtr.Zero)
                {
                    _selectWindowPen = XorGDI.CreatePen(
                        XorGDI.PenStyles.PS_SOLID, 1, XorGDI.RGB(0, 0, 255));
                }
                return _selectWindowPen;
            }
        }

        internal IntPtr selectCrossPen
        {
            get
            {
                if (_selectCrossPen == IntPtr.Zero)
                {
                    _selectCrossPen = XorGDI.CreatePen(
                        XorGDI.PenStyles.PS_DOT, 1, XorGDI.RGB(0, 255, 0));
                }
                return _selectCrossPen;
            }
        }

        /// <summary>
        /// 单例
        /// </summary>
        private static GDIResMgr _instance = null;
        public static GDIResMgr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GDIResMgr();
                }
                return _instance;
            }
        }

        private GDIResMgr()
        {
            _pen = new Pen(Color.White, 1);
            _entitySelectedPen = new Pen(Color.White, 1);
            _entitySelectedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            _entitySelectedPen.DashPattern = new float[] { 3, 3 };
            _brush = new SolidBrush(Color.White);
        }

        public Pen GetPen(Color color, double width)
        {
            _pen.Color = color;
            _pen.Width = (float)width;
            return _pen;
        }

        public Pen GetEntitySelectedPen(Entity entity)
        {
            _entitySelectedPen.Color = entity.colorValue;
            return _entitySelectedPen;
        }

        public SolidBrush GetBrush(Color color)
        {
            _brush.Color = color;
            return _brush;
        }
    }
}
