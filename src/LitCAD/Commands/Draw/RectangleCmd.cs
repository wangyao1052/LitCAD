using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Draw
{
    internal class RectangleCmd : DrawCmd
    {
        private Polyline _rectangle = null;

        /// <summary>
        /// 新增的图元
        /// </summary>
        protected override IEnumerable<Entity> newEntities
        {
            get { return new Polyline[1] { _rectangle }; }
        }

        // 起点+终点
        private LitMath.Vector2 _point1st = new LitMath.Vector2(0, 0);
        private LitMath.Vector2 _point2nd = new LitMath.Vector2(0, 0);

        private void UpdateRectangle()
        {
            if (_rectangle == null)
            {
                _rectangle = new Polyline();
                _rectangle.closed = true;
                for (int i = 0; i < 4; ++i)
                {
                    _rectangle.AddVertexAt(0, new LitMath.Vector2(0, 0));
                }
            }

            _rectangle.SetPointAt(0, _point1st);
            _rectangle.SetPointAt(1, new LitMath.Vector2(_point2nd.x, _point1st.y));
            _rectangle.SetPointAt(2, _point2nd);
            _rectangle.SetPointAt(3, new LitMath.Vector2(_point1st.x, _point2nd.y));
            _rectangle.layerId = this.document.currentLayerId;
            _rectangle.color = this.document.currentColor;
        }

        /// <summary>
        /// 步骤
        /// </summary>
        private enum Step
        {
            Step1_SpecifyPoint1st = 1,
            Step2_SpecifyPoint2nd = 2,
        }
        private Step _step = Step.Step1_SpecifyPoint1st;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            //
            _step = Step.Step1_SpecifyPoint1st;
            this.pointer.mode = UI.Pointer.Mode.Locate;
        }

        public override EventResult OnMouseDown(MouseEventArgs e)
        {
            switch (_step)
            {
                case Step.Step1_SpecifyPoint1st:
                    if (e.Button == MouseButtons.Left)
                    {
                        _point1st = this.pointer.currentSnapPoint;
                        _step = Step.Step2_SpecifyPoint2nd;
                    }
                    break;

                case Step.Step2_SpecifyPoint2nd:
                    if (e.Button == MouseButtons.Left)
                    {
                        _point2nd = this.pointer.currentSnapPoint;
                        this.UpdateRectangle();

                        _mgr.FinishCurrentCommand();
                    }
                    break;
            }

            return EventResult.Handled;
        }

        public override EventResult OnMouseUp(MouseEventArgs e)
        {
            return EventResult.Handled;
        }

        public override EventResult OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                return EventResult.Handled;
            }

            if (_step == Step.Step2_SpecifyPoint2nd)
            {
                _point2nd = this.pointer.currentSnapPoint;
                this.UpdateRectangle();
            }

            return EventResult.Handled;
        }

        public override void OnPaint(Graphics g)
        {
            if (_rectangle != null)
            {
                this.presenter.DrawEntity(g, _rectangle);
            }
        }
    }
}
