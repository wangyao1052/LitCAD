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
    /// 镜像命令
    /// </summary>
    internal class MirrorCmd : ModifyCmd
    {
        /// <summary>
        /// 源图元
        /// </summary>
        private List<Entity> _entities = new List<Entity>();

        /// <summary>
        /// 结果图元
        /// </summary>
        private List<Entity> _resultEntities = new List<Entity>();

        /// <summary>
        /// 源图元是否被删除
        /// </summary>
        private bool _isSrcDeleted = false;

        /// <summary>
        /// 镜像线
        /// </summary>
        private Line _mirrorLine = null;

        /// <summary>
        /// 步骤
        /// </summary>
        private enum Step
        {
            // 选择对象
            Step1_SelectObject = 1,
            // 指定镜像线第一点
            Step2_SpecifyMirrorLinePoint1st = 2,
            // 指定镜像线第二点
            Step3_SpecifyMirrorLinePoint2nd = 3,
            // 是否删除源对象
            Step4_WhetherDelSrc = 4,
        }
        private Step _step = Step.Step1_SelectObject;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            if (this.presenter.selections.Count > 0)
            {
                foreach (Selection sel in this.presenter.selections)
                {
                    Entity entity = this.database.GetObject(sel.objectId) as Entity;
                    if (entity != null)
                    {
                        _entities.Add(entity);
                    }
                }
            }

            if (_entities.Count > 0)
            {
                this.pointer.mode = Pointer.Mode.Locate;
                _step = Step.Step2_SpecifyMirrorLinePoint1st;
            }
            else
            {
                this.presenter.selections.Clear();
                _step = Step.Step1_SelectObject;
                this.pointer.mode = Pointer.Mode.Select;
            }
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
                case Step.Step1_SelectObject:
                    break;

                case Step.Step2_SpecifyMirrorLinePoint1st:
                    if (e.Button == MouseButtons.Left)
                    {
                        _mirrorLine = new Line();
                        _mirrorLine.startPoint = this.pointer.currentSnapPoint;
                        _mirrorLine.endPoint = _mirrorLine.startPoint;

                        _step = Step.Step3_SpecifyMirrorLinePoint2nd;
                    }
                    break;

                case Step.Step3_SpecifyMirrorLinePoint2nd:
                    if (e.Button == MouseButtons.Left)
                    {
                        _mirrorLine.endPoint = this.pointer.currentSnapPoint;
                        this.UpdateResultEntities();

                        _step = Step.Step4_WhetherDelSrc;
                        _mgr.FinishCurrentCommand();
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
                case Step.Step1_SelectObject:
                    if (e.Button == MouseButtons.Right)
                    {
                        if (this.presenter.selections.Count > 0)
                        {
                            foreach (Selection sel in _mgr.presenter.selections)
                            {
                                DBObject dbobj = this.database.GetObject(sel.objectId);
                                Entity entity = dbobj as Entity;
                                _entities.Add(entity);
                            }

                            this.pointer.mode = Pointer.Mode.Locate;
                            _step = Step.Step2_SpecifyMirrorLinePoint1st;
                        }
                        else
                        {
                            _mgr.CancelCurrentCommand();
                        }
                    }
                    break;

                case Step.Step2_SpecifyMirrorLinePoint1st:
                    break;

                case Step.Step3_SpecifyMirrorLinePoint2nd:
                    break;

                case Step.Step4_WhetherDelSrc:
                    break;

                default:
                    break;
            }

            return EventResult.Handled;
        }

        public override EventResult OnMouseMove(MouseEventArgs e)
        {
            if (_step == Step.Step3_SpecifyMirrorLinePoint2nd)
            {
                _mirrorLine.endPoint = this.pointer.currentSnapPoint;
                this.UpdateResultEntities();
            }

            return EventResult.Handled;
        }

        public override EventResult OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _mgr.CancelCurrentCommand();
            }

            return EventResult.Handled;
        }

        public override EventResult OnKeyUp(KeyEventArgs e)
        {
            return EventResult.Handled;
        }

        public override void OnPaint(Graphics g)
        {
            if (_step == Step.Step3_SpecifyMirrorLinePoint2nd)
            {
                this.presenter.DrawEntity(g, _mirrorLine);
            }

            if (_step == Step.Step3_SpecifyMirrorLinePoint2nd
                || _step == Step.Step4_WhetherDelSrc)
            {
                foreach (Entity entity in _resultEntities)
                {
                    this.presenter.DrawEntity(g, entity);
                }
            }
        }

        /// <summary>
        /// 刷新结果图元
        /// </summary>
        private void UpdateResultEntities()
        {
            LitMath.Matrix3 mirrorMatrix = MathUtils.MirrorMatrix(
                        new LitMath.Line2(_mirrorLine.startPoint, _mirrorLine.endPoint));
            _resultEntities.Clear();
            foreach (Entity entity in _entities)
            {
                Entity copy = entity.Clone() as Entity;
                copy.TransformBy(mirrorMatrix);
                _resultEntities.Add(copy);
            }
        }
    }
}
