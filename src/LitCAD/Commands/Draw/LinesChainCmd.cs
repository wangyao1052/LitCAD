using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;
using LitCAD.UI;

namespace LitCAD.Commands.Draw
{
    internal class LinesChainCmd : DrawCmd
    {
        private List<Line> _lines = new List<Line>();
        private Line _currLine = null;

        /// <summary>
        /// 新增的图元
        /// </summary>
        protected override IEnumerable<Entity> newEntities
        {
            get { return _lines.ToArray(); }
        }

        /// <summary>
        /// 步骤
        /// </summary>
        private enum Step
        {
            Step1_SpecifyStartPoint = 1,
            Step2_SpecifyEndPoint = 2,
        }
        private Step _step = Step.Step1_SpecifyStartPoint;

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

            //
            _step = Step.Step1_SpecifyStartPoint;
            this.pointer.mode = UI.Pointer.Mode.Locate;

            _pointInput = new DynInputPoint(this.presenter, new LitMath.Vector2(0, 0));
            _pointInput.Message = "指定第一个点: ";
            this.dynamicInputer.StartInput(_pointInput);
            _pointInput.finish += this.OnPointInputReturn;
            _pointInput.cancel += this.OnPointInputReturn;
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
            if (_step == Step.Step1_SpecifyStartPoint)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _currLine = new Line();
                    _currLine.startPoint = this.pointer.currentSnapPoint;
                    _currLine.endPoint = this.pointer.currentSnapPoint;
                    _currLine.layerId = this.document.currentLayerId;
                    _currLine.color = this.document.currentColor;

                    _pointInput.Message = "指定下一点: ";
                    _step = Step.Step2_SpecifyEndPoint;
                }
            }
            else if (_step == Step.Step2_SpecifyEndPoint)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _currLine.endPoint = this.pointer.currentSnapPoint;
                    _currLine.layerId = this.document.currentLayerId;
                    _currLine.color = this.document.currentColor;
                    _lines.Add(_currLine);

                    _currLine = new Line();
                    _currLine.startPoint = this.pointer.currentSnapPoint;
                    _currLine.endPoint = this.pointer.currentSnapPoint;
                    _currLine.layerId = this.document.currentLayerId;
                    _currLine.color = this.document.currentColor;
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

            if (_currLine != null)
            {
                _currLine.endPoint = this.pointer.currentSnapPoint;
            }

            return EventResult.Handled;
        }

        public override EventResult OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (_lines.Count > 0)
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
            foreach (Line line in _lines)
            {
                Presenter presenter = _mgr.presenter as Presenter;
                presenter.DrawEntity(g, line);
            }

            if (_currLine != null)
            {
                Presenter presenter = _mgr.presenter as Presenter;
                presenter.DrawEntity(g, _currLine);
            }
        }

        private void OnPointInputReturn(DynInputCtrl sender, DynInputResult retult)
        {
            DynInputResult<LitMath.Vector2> xyRet = retult as DynInputResult<LitMath.Vector2>;
            if (xyRet == null 
                || xyRet.status == DynInputStatus.Cancel)
            {
                if (_lines.Count > 0)
                {
                    _mgr.FinishCurrentCommand();
                }
                else
                {
                    _mgr.CancelCurrentCommand();
                }

                return;
            }

            _pointInput.Message = "指定下一点: ";
            this.dynamicInputer.StartInput(_pointInput);

            switch (_step)
            {
                case Step.Step1_SpecifyStartPoint:
                    {
                        _currLine = new Line();
                        _currLine.startPoint = xyRet.value;
                        _currLine.endPoint = xyRet.value;
                        _currLine.layerId = this.document.currentLayerId;
                        _currLine.color = this.document.currentColor;

                        _step = Step.Step2_SpecifyEndPoint;
                    }
                    break;

                case Step.Step2_SpecifyEndPoint:
                    {
                        _currLine.endPoint = xyRet.value;
                        _currLine.layerId = this.document.currentLayerId;
                        _currLine.color = this.document.currentColor;
                        _lines.Add(_currLine);

                        _currLine = new Line();
                        _currLine.startPoint = xyRet.value;
                        _currLine.endPoint = xyRet.value;
                        _currLine.layerId = this.document.currentLayerId;
                        _currLine.color = this.document.currentColor;
                    }
                    break;
            }
        }
    }
}
