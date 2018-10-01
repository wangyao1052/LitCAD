using System;
using System.Drawing;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 图元节点
    /// </summary>
    internal abstract class EntityAnchor
    {
        /// <summary>
        /// 图元
        /// </summary>
        public abstract Entity entity { get; }

        /// <summary>
        /// 位置
        /// Model CSYS
        /// </summary>
        public abstract LitMath.Vector2 position { get; }

        /// <summary>
        /// 绘制
        /// </summary>
        public virtual void OnDraw(Presenter presenter, Graphics g, Color color)
        {
            double width = 10;
            double height = 10;
            LitMath.Vector2 posInCanvas = presenter.ModelToCanvas(this.position);
            posInCanvas.x -= width / 2;
            posInCanvas.y -= height / 2;
            presenter.FillRectangle(g, GDIResMgr.Instance.GetBrush(color), posInCanvas, width, height, CSYS.Canvas);
        }

        public virtual bool HitTest(Presenter presenter, LitMath.Vector2 pointInCanvas)
        {
            double width = 10;
            double height = 10;
            LitMath.Vector2 posInCanvas = presenter.ModelToCanvas(this.position);
            posInCanvas.x -= width / 2;
            posInCanvas.y -= height / 2;
            LitMath.Rectangle2 rect = new LitMath.Rectangle2(posInCanvas, width, height);

            if (MathUtils.IsPointInRectangle(pointInCanvas, rect))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
