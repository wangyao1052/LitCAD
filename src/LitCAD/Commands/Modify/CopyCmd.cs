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
    /// 复制命令
    /// </summary>
    internal class CopyCmd : ModifyCmd
    {
        /// <summary>
        /// 操作的图元
        /// </summary>
        private List<Entity> _items = new List<Entity>();
        private List<Entity> _tempItemsToDraw = new List<Entity>();
        private void InitializeItemsToCopy()
        {
            Document doc = _mgr.presenter.document;
            foreach (Selection sel in _mgr.presenter.selections)
            {
                DBObject dbobj = doc.database.GetObject(sel.objectId);
                if (dbobj != null && dbobj is Entity)
                {
                    Entity entity = dbobj as Entity;
                    _items.Add(entity);

                    Entity tempEntity = entity.Clone() as Entity;
                    _tempItemsToDraw.Add(tempEntity);
                }
            }
        }

        /// <summary>
        /// 复制动作结果
        /// </summary>
        private class CopyAction
        {
            public List<Entity> copyItems = new List<Entity>();
            public LitMath.Line2 pathLine = new LitMath.Line2();
        }
        private List<CopyAction> _actions = new List<CopyAction>();
        private void FinishOneCopyAction()
        {
            CopyAction copyAction = new CopyAction();

            foreach (Entity item in _items)
            {
                Entity copyItem = item.Clone() as Entity;
                copyItem.Translate(this.translation);

                copyAction.copyItems.Add(copyItem);
            }
            copyAction.pathLine = _pathLine;

            _actions.Add(copyAction);
        }

        /// <summary>
        /// 步骤
        /// </summary>
        private enum Step
        {
            Step1_SelectObjects = 1,
            Step2_SpecifyBasePoint = 2,
            Step3_SpecifySecondPoint = 3,
        }
        private Step _step = Step.Step1_SelectObjects;

        /// <summary>
        /// 移动路径线
        /// </summary>
        private LitMath.Line2 _pathLine = new LitMath.Line2();
        private LitMath.Vector2 translation
        {
            get
            {
                return _pathLine.endPoint - _pathLine.startPoint;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            //
            if (this.presenter.selections.Count > 0)
            {
                _step = Step.Step2_SpecifyBasePoint;
                InitializeItemsToCopy();
                this.pointer.mode = Pointer.Mode.Locate;
            }
            else
            {
                _step = Step.Step1_SelectObjects;
                this.pointer.mode = Pointer.Mode.Select;
            }
        }

        /// <summary>
        /// 提交到数据库
        /// </summary>
        protected override void Commit()
        {
            foreach (CopyAction action in _actions)
            {
                foreach (Entity copyItem in action.copyItems)
                {
                    _mgr.presenter.AppendEntity(copyItem);
                }
            }
        }

        /// <summary>
        /// 回滚撤销
        /// </summary>
        protected override void Rollback()
        {
            foreach (CopyAction action in _actions)
            {
                foreach (Entity copyItem in action.copyItems)
                {
                    copyItem.Erase();
                }
            }
        }

        public override EventResult OnMouseDown(MouseEventArgs e)
        {
            switch (_step)
            {
                case Step.Step1_SelectObjects:
                    break;

                case Step.Step2_SpecifyBasePoint:
                    if (e.Button == MouseButtons.Left)
                    {
                        _pathLine.startPoint = this.pointer.currentSnapPoint;
                        _pathLine.endPoint = _pathLine.startPoint;

                        _step = Step.Step3_SpecifySecondPoint;
                    }
                    break;

                case Step.Step3_SpecifySecondPoint:
                    if (e.Button == MouseButtons.Left)
                    {
                        _pathLine.endPoint = this.pointer.currentSnapPoint;

                        FinishOneCopyAction();
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
                case Step.Step1_SelectObjects:
                    if (e.Button == MouseButtons.Right)
                    {
                        if (_mgr.presenter.selections.Count > 0)
                        {
                            _step = Step.Step2_SpecifyBasePoint;
                            InitializeItemsToCopy();

                            this.pointer.mode = Pointer.Mode.Locate;
                        }
                        else
                        {
                            _mgr.CancelCurrentCommand();
                        }
                    }
                    break;

                case Step.Step2_SpecifyBasePoint:
                    break;

                case Step.Step3_SpecifySecondPoint:
                    break;

                default:
                    break;
            }

            return EventResult.Handled;
        }

        public override EventResult OnMouseMove(MouseEventArgs e)
        {
            switch (_step)
            {
                case Step.Step1_SelectObjects:
                    break;

                case Step.Step2_SpecifyBasePoint:
                    break;

                case Step.Step3_SpecifySecondPoint:
                    LitMath.Vector2 preTranslation = this.translation;
                    _pathLine.endPoint = this.pointer.currentSnapPoint;

                    LitMath.Vector2 offset = this.translation - preTranslation;
                    foreach (Entity tempEntity in _tempItemsToDraw)
                    {
                        tempEntity.Translate(offset);
                    }
                    break;

                default:
                    break;
            }

            return EventResult.Handled;
        }

        public override EventResult OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (_step == Step.Step3_SpecifySecondPoint)
                //{
                    if (_actions.Count > 0)
                    {
                        _mgr.FinishCurrentCommand();
                    }
                    else
                    {
                        _mgr.CancelCurrentCommand();
                    }
                //}
            }

            return EventResult.Handled;
        }

        public override EventResult OnKeyUp(KeyEventArgs e)
        {
            return EventResult.Handled;
        }

        public override void OnPaint(Graphics g)
        {
            switch (_step)
            {
                case Step.Step1_SelectObjects:
                    break;

                case Step.Step2_SpecifyBasePoint:
                    break;

                case Step.Step3_SpecifySecondPoint:
                    foreach (CopyAction action in _actions)
                    {
                        foreach (Entity entity in action.copyItems)
                        {
                            _mgr.presenter.DrawEntity(g, entity);
                        }
                    }

                    foreach (Entity tempItem in _tempItemsToDraw)
                    {
                        _mgr.presenter.DrawEntity(g, tempItem);
                    }

                    _mgr.presenter.DrawLine(g, GDIResMgr.Instance.GetPen(Color.White, 1),
                        _pathLine.startPoint, _pathLine.endPoint,
                        CSYS.Model);
                    break;

                default:
                    break;
            }
        }
    }
}
