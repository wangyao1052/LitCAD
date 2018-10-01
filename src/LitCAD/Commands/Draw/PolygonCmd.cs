using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;
using LitCAD.UI;

namespace LitCAD.Commands.Draw
{
    internal class PolygonCmd : DrawCmd
    {
        private Polyline _polygon = null;

        /// <summary>
        /// 新增的图元
        /// </summary>
        protected override IEnumerable<Entity> newEntities
        {
            get { return new Polyline[1] { _polygon }; }
        }

        /// <summary>
        /// 多边形边数,>=3
        /// </summary>
        private uint _sides = 5;

        /// <summary>
        /// 定位点
        /// </summary>
        private LitMath.Vector2 _center = new LitMath.Vector2(0, 0);
        private LitMath.Vector2 _point = new LitMath.Vector2(0, 0);

        /// <summary>
        /// 选项
        /// 内接于圆,外切于圆
        /// </summary>
        private enum Option
        {
            InscribedInCircle = 0,
            CircumscribedAboutCircle = 1,
        }
        private Option _option = Option.CircumscribedAboutCircle;

        /// <summary>
        /// 刷新正多边形
        /// </summary>
        private void UpdatePolygon()
        {
            if (_polygon == null)
            {
                _polygon = new Polyline();
                _polygon.closed = true;
                for (int i = 0; i < _sides; ++i)
                {
                    _polygon.AddVertexAt(i, new LitMath.Vector2(0, 0));
                }
            }

            switch (_option)
            {
                case Option.InscribedInCircle:
                    {
                        LitMath.Vector2 sPnt = _point - _center;
                        _polygon.SetPointAt(0, _center + sPnt);
                        for (int i = 1; i < _sides; ++i)
                        {
                            LitMath.Vector2 vPnt = LitMath.Vector2.Rotate(sPnt, 360.0 / _sides * i);
                            _polygon.SetPointAt(i, _center + vPnt);
                        }
                    }
                    break;

                case Option.CircumscribedAboutCircle:
                    {
                        double radius = (_point - _center).length;
                        double angle = (LitMath.Utils.PI * 2) / _sides;
                        double l = radius / Math.Cos(angle / 2.0);

                        LitMath.Vector2 sPnt = (_point - _center).normalized * l;
                        sPnt = LitMath.Vector2.RotateInRadian(sPnt, angle / 2);
                        _polygon.SetPointAt(0, _center + sPnt);

                        for (int i = 1; i < _sides; ++i)
                        {
                            LitMath.Vector2 vPnt = LitMath.Vector2.RotateInRadian(sPnt, angle * i);
                            _polygon.SetPointAt(i, _center + vPnt);
                        }
                    }
                    break;
            }
            
            _polygon.layerId = this.document.currentLayerId;
            _polygon.color = this.document.currentColor;
        }

        /// <summary>
        /// 步骤
        /// </summary>
        private enum Step
        {
            // 边数
            Step1_SpecifySidesCount = 1,
            // 内接于圆还是外切于圆
            Step2_IORC = 2,
            Step3_SpecifyPointCenter = 3,
            Step4_SpecifyPointOther = 4,
        }
        private Step _step = Step.Step1_SpecifySidesCount;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            //
            _step = Step.Step1_SpecifySidesCount;
            this.pointer.mode = UI.Pointer.Mode.Locate;

            //
            DynInputInteger sidesCntInput = new DynInputInteger(this.presenter, 5);
            sidesCntInput.Message = "输入边数: ";
            this.dynamicInputer.StartInput(sidesCntInput);
            sidesCntInput.finish += this.OnSidesCntInputReturn;
            sidesCntInput.cancel += this.OnSidesCntInputReturn;
        }

        /// <summary>
        /// 边数输入结束事件响应
        /// </summary>
        private void OnSidesCntInputReturn(DynInputCtrl sender, DynInputResult result)
        {
            DynInputResult<int> ret = result as DynInputResult<int>;
            if (ret != null
                && ret.status == DynInputStatus.OK)
            {
                _sides = (uint)ret.value;
                _step = Step.Step2_IORC;

                DynInputString icInput = new DynInputString(this.presenter, "I");
                icInput.Message = "输入选项[内接于圆(I)/外切于圆(C)]: ";
                this.dynamicInputer.StartInput(icInput);
                icInput.finish += this.OnIcInputReturn;
                icInput.cancel += this.OnIcInputReturn;
            }
            else
            {
                _mgr.CancelCurrentCommand();
            }

            sender.finish -= this.OnSidesCntInputReturn;
            sender.cancel -= this.OnSidesCntInputReturn;
        }

        /// <summary>
        /// 内接于圆还是外切于圆
        /// 输入结束事件响应
        /// </summary>
        private void OnIcInputReturn(DynInputCtrl sender, DynInputResult result)
        {
            DynInputResult<string> ret = result as DynInputResult<string>;
            if (ret != null
                && ret.status == DynInputStatus.OK)
            {
                if (ret.value.Trim().ToUpper() == "I")
                {
                    _option = Option.InscribedInCircle;
                }
                else
                {
                    _option = Option.CircumscribedAboutCircle;
                }

                _step = Step.Step3_SpecifyPointCenter;
            }
            else
            {
                _mgr.CancelCurrentCommand();
            }

            sender.finish -= this.OnIcInputReturn;
            sender.cancel -= this.OnIcInputReturn;
        }

        public override EventResult OnMouseDown(MouseEventArgs e)
        {
            switch (_step)
            {
                case Step.Step3_SpecifyPointCenter:
                    if (e.Button == MouseButtons.Left)
                    {
                        _center = this.pointer.currentSnapPoint;
                        _step = Step.Step4_SpecifyPointOther;
                    }
                    break;

                case Step.Step4_SpecifyPointOther:
                    if (e.Button == MouseButtons.Left)
                    {
                        _point = this.pointer.currentSnapPoint;
                        this.UpdatePolygon();

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

            if (_step == Step.Step4_SpecifyPointOther)
            {
                _point = this.pointer.currentSnapPoint;
                this.UpdatePolygon();
            }

            return EventResult.Handled;
        }

        public override void OnPaint(Graphics g)
        {
            if (_polygon != null)
            {
                this.presenter.DrawEntity(g, _polygon);
            }
        }
    }
}
