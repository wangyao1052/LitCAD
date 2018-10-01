using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Draw
{
    /// <summary>
    /// 绘制圆弧命令
    /// </summary>
    internal class ArcCmd : DrawCmd
    {
        /// <summary>
        /// 绘制的圆弧
        /// </summary>
        private Arc _arc = null;

        /// <summary>
        /// 新增的图元
        /// </summary>
        protected override IEnumerable<Entity> newEntities
        {
            get { return new Arc[1] { _arc }; }
        }

        /// <summary>
        /// 步骤
        /// </summary>
        private enum Step
        {
            Step1_SpecifyCenter = 1,
            Step2_SpecityStartPoint = 2,
            Step3_SpecifyEndPoint = 3,
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
            switch (_step)
            {
                case Step.Step1_SpecifyCenter:
                    if (e.Button == MouseButtons.Left)
                    {
                        _arc = new Arc();
                        _arc.center = this.pointer.currentSnapPoint;
                        _arc.radius = 0;
                        _arc.layerId = this.document.currentLayerId;
                        _arc.color = this.document.currentColor;

                        _step = Step.Step2_SpecityStartPoint;
                    }
                    break;

                case Step.Step2_SpecityStartPoint:
                    if (e.Button == MouseButtons.Left)
                    {
                        _arc.radius = (_arc.center - this.pointer.currentSnapPoint).length;
                        _arc.layerId = this.document.currentLayerId;
                        _arc.color = this.document.currentColor;

                        double startAngle = LitMath.Vector2.SignedAngleInRadian(
                            new LitMath.Vector2(1, 0),
                            this.pointer.currentSnapPoint - _arc.center);
                        startAngle = MathUtils.NormalizeRadianAngle(startAngle);
                        _arc.startAngle = startAngle;
                        _arc.endAngle = startAngle;

                        _step = Step.Step3_SpecifyEndPoint;
                    }
                    break;

                case Step.Step3_SpecifyEndPoint:
                    if (e.Button == MouseButtons.Left)
                    {
                        double endAngle = LitMath.Vector2.SignedAngleInRadian(
                            new LitMath.Vector2(1, 0),
                            this.pointer.currentSnapPoint - _arc.center);
                        endAngle = MathUtils.NormalizeRadianAngle(endAngle);
                        _arc.endAngle = endAngle;

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

            switch (_step)
            {
                case Step.Step1_SpecifyCenter:
                    break;

                case Step.Step2_SpecityStartPoint:
                    break;

                case Step.Step3_SpecifyEndPoint:
                    double endAngle = LitMath.Vector2.SignedAngleInRadian(
                            new LitMath.Vector2(1, 0),
                            this.pointer.currentSnapPoint - _arc.center);
                    endAngle = MathUtils.NormalizeRadianAngle(endAngle);
                    _arc.endAngle = endAngle;
                    break;
            }

            return EventResult.Handled;
        }

        public override void OnPaint(Graphics g)
        {
            if (_arc != null)
            {
                Presenter presenter = _mgr.presenter as Presenter;
                presenter.DrawEntity(g, _arc);
            }

            switch (_step)
            {
                case Step.Step1_SpecifyCenter:
                    break;

                case Step.Step2_SpecityStartPoint:
                case Step.Step3_SpecifyEndPoint:
                    _mgr.presenter.DrawLine(g,
                        GDIResMgr.Instance.GetPen(Color.White, 0),
                        _arc.center,
                        this.pointer.currentSnapPoint,
                        CSYS.Model);
                    break;
            }
        }
    }
}
