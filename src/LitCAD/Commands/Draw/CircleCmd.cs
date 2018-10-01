using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Draw
{
    internal class CircleCmd : DrawCmd
    {
        private Circle _circle = null;

        /// <summary>
        /// 新增的图元
        /// </summary>
        protected override IEnumerable<Entity> newEntities
        {
            get { return new Circle[1] { _circle }; }
        }

        /// <summary>
        /// 步骤
        /// </summary>
        private enum Step
        {
            Step1_SpecifyCenter = 1,
            Step2_SpecityRadius = 2,
        }
        private Step _step = Step.Step1_SpecifyCenter;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            //
            _step = Step.Step1_SpecifyCenter;
            this.pointer.mode = UI.Pointer.Mode.Locate;
        }

        public override EventResult OnMouseDown(MouseEventArgs e)
        {
            if (_step == Step.Step1_SpecifyCenter)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _circle = new Circle();
                    _circle.center = this.pointer.currentSnapPoint;
                    _circle.radius = 0;
                    _circle.layerId = this.document.currentLayerId;
                    _circle.color = this.document.currentColor;

                    _step = Step.Step2_SpecityRadius;
                }
            }
            else if (_step == Step.Step2_SpecityRadius)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _circle.radius = (_circle.center - this.pointer.currentSnapPoint).length;
                    _circle.layerId = this.document.currentLayerId;
                    _circle.color = this.document.currentColor;

                    _mgr.FinishCurrentCommand();
                }
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

            if (_circle != null)
            {
                _circle.radius = (_circle.center - this.pointer.currentSnapPoint).length;
            }

            return EventResult.Handled;
        }

        public override void OnPaint(Graphics g)
        {
            if (_circle != null)
            {
                Presenter presenter = _mgr.presenter as Presenter;
                presenter.DrawEntity(g, _circle);
            }
        }
    }
}
