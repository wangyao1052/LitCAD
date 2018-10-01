using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;
using LitCAD.UI;

namespace LitCAD.Commands.Modify
{
    /// <summary>
    /// 偏移命令
    /// </summary>
    internal class OffsetCmd : ModifyCmd
    {
        /// <summary>
        /// 结果图元
        /// </summary>
        private List<Entity> _resultEntities = new List<Entity>();

        /// <summary>
        /// 当前正在操作的图元
        /// 参数
        /// </summary>
        private Entity _currEntity = null;
        private Offset._OffsetOperation _currOffsetOp = null;
        private double _offsetDis = 0.0;

        /// <summary>
        /// 步骤
        /// </summary>
        private enum Step
        {
            // 指定偏移距离
            Step1_SpecifyOffsetDistance = 1,
            // 选择要偏移的对象
            Step2_SelectObject = 2,
            // 指定偏移侧
            Step3_SpecifyOffsetSide = 3,
        }
        private Step _step = Step.Step1_SpecifyOffsetDistance;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            if (this.presenter.selections.Count == 1)
            {
                foreach (Selection sel in this.presenter.selections)
                {
                    Entity entity = this.database.GetObject(sel.objectId) as Entity;
                    _currEntity = entity;
                    _currOffsetOp = Offset._OffsetOpsMgr.Instance.NewOffsetOperation(_currEntity);
                    break;
                }
            }
            if (_currEntity == null)
            {
                this.presenter.selections.Clear();
            }

            _step = Step.Step1_SpecifyOffsetDistance;
            this.pointer.mode = Pointer.Mode.Locate;

            DynInputDouble offdisInput = new DynInputDouble(this.presenter, 0.0);
            offdisInput.Message = "指定偏移距离: ";
            this.dynamicInputer.StartInput(offdisInput);
            offdisInput.finish += this.OnOffsetDistanceInputReturn;
            offdisInput.cancel += this.OnOffsetDistanceInputReturn;
        }

        /// <summary>
        /// 提交到数据库
        /// </summary>
        protected override void Commit()
        {
            foreach (Entity item in _resultEntities)
            {
                _mgr.presenter.AppendEntity(item);
            }
        }

        /// <summary>
        /// 回滚撤销
        /// </summary>
        protected override void Rollback()
        {
            foreach (Entity item in _resultEntities)
            {
                item.Erase();
            }
        }

        public override EventResult OnMouseDown(MouseEventArgs e)
        {
            switch (_step)
            {
                case Step.Step1_SpecifyOffsetDistance:
                    break;

                case Step.Step2_SelectObject:
                    break;

                case Step.Step3_SpecifyOffsetSide:
                    if (e.Button == MouseButtons.Left
                        && _currOffsetOp != null)
                    {
                        if (_currOffsetOp.Do(_offsetDis, this.pointer.currentSnapPoint))
                        {
                            _resultEntities.Add(_currOffsetOp.result);
                            _mgr.FinishCurrentCommand();
                        }
                        else
                        {
                            _mgr.CancelCurrentCommand();
                        }
                    }
                    break;

                default:
                    break;
            }

            return EventResult.Handled;
        }

        public override EventResult OnMouseUp(MouseEventArgs e)
        {
            switch (_step)
            {
                case Step.Step1_SpecifyOffsetDistance:
                    break;

                case Step.Step2_SelectObject:
                    if (e.Button == MouseButtons.Right)
                    {
                        if (this.presenter.selections.Count > 0)
                        {
                            foreach (Selection sel in _mgr.presenter.selections)
                            {
                                DBObject dbobj = this.database.GetObject(sel.objectId);
                                _currEntity = dbobj as Entity;
                                _currOffsetOp = Offset._OffsetOpsMgr.Instance.NewOffsetOperation(_currEntity);
                                break;
                            }

                            this.pointer.mode = Pointer.Mode.Locate;
                            _step = Step.Step3_SpecifyOffsetSide;
                        }
                        else
                        {
                            _mgr.CancelCurrentCommand();
                        }
                    }
                    break;

                case Step.Step3_SpecifyOffsetSide:
                    break;

                default:
                    break;
            }

            return EventResult.Handled;
        }

        public override EventResult OnMouseMove(MouseEventArgs e)
        {
            if (_step == Step.Step3_SpecifyOffsetSide)
            {
                if (_currOffsetOp != null)
                {
                    _currOffsetOp.Do(_offsetDis, this.pointer.currentSnapPoint);
                }
            }

            return EventResult.Handled;
        }

        public override EventResult OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (_resultEntities.Count > 0)
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
            if (_currOffsetOp != null
                && _currOffsetOp.result != null)
            {
                this.presenter.DrawEntity(g, _currOffsetOp.result);
            }
        }

        private void OnOffsetDistanceInputReturn(DynInputCtrl sender, DynInputResult result)
        {
            DynInputResult<double> ret = result as DynInputResult<double>;
            if (ret != null
                && ret.status == DynInputStatus.OK)
            {
                _offsetDis = Math.Abs(ret.value);

                if (_currEntity != null)
                {
                    this.pointer.mode = Pointer.Mode.Locate;
                    _step = Step.Step3_SpecifyOffsetSide;
                }
                else
                {
                    _step = Step.Step2_SelectObject;
                    this.pointer.mode = Pointer.Mode.Select;
                }
            }
            else
            {
                _mgr.CancelCurrentCommand();
            }

            sender.finish -= this.OnOffsetDistanceInputReturn;
            sender.cancel -= this.OnOffsetDistanceInputReturn;
        }
    }
}
