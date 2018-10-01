using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Draw
{
    internal class PolylineCmd : DrawCmd
    {
        private Polyline _polyline = null;
        private Line _line = null;

        /// <summary>
        /// 新增的图元
        /// </summary>
        protected override IEnumerable<Entity> newEntities
        {
            get{ return new Polyline[1] { _polyline }; }
        }

        /// <summary>
        /// 步骤
        /// </summary>
        private enum Step
        {
            Step1_SpecifyStartPoint = 1,
            Step2_SpecifyOtherPoint = 2,
        }
        private Step _step = Step.Step1_SpecifyStartPoint;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            //
            _step = Step.Step1_SpecifyStartPoint;
            this.pointer.mode = UI.Pointer.Mode.Locate;
        }

        public override EventResult OnMouseDown(MouseEventArgs e)
        {
            switch (_step)
            {
                case Step.Step1_SpecifyStartPoint:
                    if (e.Button == MouseButtons.Left)
                    {
                        _polyline = new Polyline();
                        _polyline.AddVertexAt(_polyline.NumberOfVertices, this.pointer.currentSnapPoint);

                        _line = new Line();
                        _line.startPoint = _line.endPoint = this.pointer.currentSnapPoint;

                        _step = Step.Step2_SpecifyOtherPoint;
                    }
                    break;

                case Step.Step2_SpecifyOtherPoint:
                    if (e.Button == MouseButtons.Left)
                    {
                        _polyline.AddVertexAt(_polyline.NumberOfVertices, this.pointer.currentSnapPoint);
                        _line.startPoint = this.pointer.currentSnapPoint;
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

            switch (_step)
            {
                case Step.Step1_SpecifyStartPoint:
                    break;

                case Step.Step2_SpecifyOtherPoint:
                    if (_line != null)
                    {
                        _line.endPoint = this.pointer.currentSnapPoint;
                    }
                    break;
            }
            
            return EventResult.Handled;
        }

        public override EventResult OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (_polyline.NumberOfVertices > 1)
                {
                    _polyline.layerId = this.document.currentLayerId;
                    _polyline.color = this.document.currentColor;
                    _mgr.FinishCurrentCommand();
                }
                else
                {
                    _mgr.CancelCurrentCommand();
                }
            }
            return EventResult.Handled;
        }

        public override EventResult OnKeyUp(KeyEventArgs e)
        {
            return EventResult.Handled;
        }

        public override void OnPaint(Graphics g)
        {
            if (_polyline != null)
            {
                _mgr.presenter.DrawEntity(g, _polyline);
            }
            if (_line != null)
            {
                _mgr.presenter.DrawEntity(g, _line);
            }
        }
    }
}
