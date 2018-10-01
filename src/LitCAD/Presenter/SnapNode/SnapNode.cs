using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 捕捉节点
    /// </summary>
    internal abstract class SnapNode
    {
        /// <summary>
        /// 位置
        /// In Model CSYS
        /// </summary>
        protected LitMath.Vector2 _position = new LitMath.Vector2(0, 0);
        public LitMath.Vector2 position
        {
            get { return _position; }
        }

        /// <summary>
        /// 捕捉阀值
        /// In Canvas CSYS
        /// </summary>
        protected double _threshold = 8;
        internal double threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }

        public SnapNode(LitMath.Vector2 pos)
        {
            _position = pos;
        }

        /// <summary>
        /// 绘制
        /// </summary>
        internal virtual void OnDraw(Presenter presenter, Graphics g)
        {
        }

        protected virtual Pen pen
        {
            get
            {
                return GDIResMgr.Instance.GetPen(Color.White, 2);
            }
        }
    }
}
