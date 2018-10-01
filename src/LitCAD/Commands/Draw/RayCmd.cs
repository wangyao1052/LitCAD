using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;
using LitCAD.UI;

namespace LitCAD.Commands.Draw
{
    internal class RayCmd : DrawCmd
    {
        private List<Ray> _xlines = new List<Ray>();
        private Ray _currXline = null;

        /// <summary>
        /// 新增的图元
        /// </summary>
        protected override IEnumerable<Entity> newEntities
        {
            get { return _xlines.ToArray(); }
        }

        /// <summary>
        /// 步骤
        /// </summary>
        private enum Step
        {
            Step1_SpecifyBasePoint = 1,
            Step2_SpecifyOtherPoint = 2,
        }
        private Step _step = Step.Step1_SpecifyBasePoint;

        private void GotoStep(Step step)
        {
            switch (step)
            {
                case Step.Step1_SpecifyBasePoint:
                    {
                        this.pointer.mode = Pointer.Mode.Locate;
                        _pointInput.Message = "指定点: ";
                        this.dynamicInputer.StartInput(_pointInput);
                    }
                    break;

                case Step.Step2_SpecifyOtherPoint:
                    {
                        this.pointer.mode = Pointer.Mode.Locate;
                        _pointInput.Message = "指定通过点: ";
                        this.dynamicInputer.StartInput(_pointInput);
                    }
                    break;
            }

            _step = step;
        }

        /// <summary>
        /// 点动态输入控件
        /// </summary>
        private DynInputPoint _pointInput = null;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _pointInput = new DynInputPoint(this.presenter, new LitMath.Vector2(0, 0));
            _pointInput.finish += this.OnPointInputReturn;
            _pointInput.cancel += this.OnPointInputReturn;

            this.GotoStep(Step.Step1_SpecifyBasePoint);
        }

        /// <summary>
        /// 结束
        /// </summary>
        public override void Terminate()
        {
            _pointInput.finish -= this.OnPointInputReturn;
            _pointInput.cancel -= this.OnPointInputReturn;

            base.Terminate();
        }

        public override EventResult OnMouseDown(MouseEventArgs e)
        {
            switch (_step)
            {
                case Step.Step1_SpecifyBasePoint:
                    if (e.Button == MouseButtons.Left)
                    {
                        _currXline = new Ray();
                        _currXline.basePoint = this.pointer.currentSnapPoint;
                        _currXline.layerId = this.document.currentLayerId;
                        _currXline.color = this.document.currentColor;

                        this.GotoStep(Step.Step2_SpecifyOtherPoint);
                    }
                    break;

                case Step.Step2_SpecifyOtherPoint:
                    if (e.Button == MouseButtons.Left)
                    {
                        LitMath.Vector2 dir = (this.pointer.currentSnapPoint
                            - _currXline.basePoint).normalized;
                        if (dir.x != 0 || dir.y != 0)
                        {
                            _currXline.direction = dir;
                            _currXline.layerId = this.document.currentLayerId;
                            _currXline.color = this.document.currentColor;
                            _xlines.Add(_currXline);

                            _currXline = _currXline.Clone() as Ray;
                        }
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

            if (_currXline != null
                && _step == Step.Step2_SpecifyOtherPoint)
            {
                LitMath.Vector2 dir = (this.pointer.currentSnapPoint
                            - _currXline.basePoint).normalized;
                if (dir.x != 0 || dir.y != 0)
                {
                    _currXline.direction = dir;
                }
            }

            return EventResult.Handled;
        }

        public override EventResult OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (_xlines.Count > 0)
                {
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
            foreach (Ray xline in _xlines)
            {
                Presenter presenter = _mgr.presenter as Presenter;
                presenter.DrawEntity(g, xline);
            }

            if (_currXline != null
                && _step == Step.Step2_SpecifyOtherPoint)
            {
                Presenter presenter = _mgr.presenter as Presenter;
                presenter.DrawEntity(g, _currXline);
            }
        }

        private void OnPointInputReturn(DynInputCtrl sender, DynInputResult retult)
        {
            DynInputResult<LitMath.Vector2> xyRet = retult as DynInputResult<LitMath.Vector2>;
            if (xyRet != null
                && xyRet.status == DynInputStatus.OK)
            {
                switch (_step)
                {
                    case Step.Step1_SpecifyBasePoint:
                        {
                            _currXline = new Ray();
                            _currXline.basePoint = xyRet.value;
                            _currXline.layerId = this.document.currentLayerId;
                            _currXline.color = this.document.currentColor;

                            this.GotoStep(Step.Step2_SpecifyOtherPoint);
                        }
                        break;

                    case Step.Step2_SpecifyOtherPoint:
                        {
                            LitMath.Vector2 dir = (xyRet.value
                                - _currXline.basePoint).normalized;
                            if (dir.x != 0 || dir.y != 0)
                            {
                                _currXline.direction = dir;
                                _currXline.layerId = this.document.currentLayerId;
                                _currXline.color = this.document.currentColor;
                                _xlines.Add(_currXline);

                                _currXline = _currXline.Clone() as Ray;
                            }

                            this.GotoStep(Step.Step2_SpecifyOtherPoint);
                        }
                        break;
                }
            }
            else
            {
                if (_xlines.Count > 0)
                {
                    _mgr.FinishCurrentCommand();
                }
                else
                {
                    _mgr.CancelCurrentCommand();
                }
            }
        }
    }
}
