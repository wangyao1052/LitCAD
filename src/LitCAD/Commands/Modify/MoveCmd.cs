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
    /// 移动命令
    /// </summary>
    internal class MoveCmd : ModifyCmd
    {
        /// <summary>
        /// 操作的图元
        /// </summary>
        private List<Entity> _items = new List<Entity>();
        private List<Entity> _copys = new List<Entity>();

        private void InitItems()
        {
            Document doc = _mgr.presenter.document as Document;
            foreach (Selection sel in _mgr.presenter.selections)
            {
                DBObject dbobj = doc.database.GetObject(sel.objectId);
                if (dbobj != null && dbobj is Entity)
                {
                    Entity entity = dbobj as Entity;
                    Entity copy = entity.Clone() as Entity;

                    _items.Add(entity);
                    _copys.Add(copy);
                }
            }
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

        public override void Initialize()
        {
            base.Initialize();

            //
            if (_mgr.presenter.selections.Count > 0)
            {
                _step = Step.Step2_SpecifyBasePoint;
                InitItems();
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
            foreach (Entity item in _items)
            {
                item.Translate(this.translation);
            }
        }

        /// <summary>
        /// 回滚撤销
        /// </summary>
        protected override void Rollback()
        {
            foreach (Entity item in _items)
            {
                item.Translate(-this.translation);
            }
        }

        public override EventResult OnMouseDown(MouseEventArgs e)
        {
            if (_step == Step.Step1_SelectObjects)
            {
            }
            else if (_step == Step.Step2_SpecifyBasePoint)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _pathLine.startPoint = this.pointer.currentSnapPoint;
                    _pathLine.endPoint = _pathLine.startPoint;

                    _step = Step.Step3_SpecifySecondPoint;
                }
            }
            else if (_step == Step.Step3_SpecifySecondPoint)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _pathLine.endPoint = this.pointer.currentSnapPoint;

                    _mgr.FinishCurrentCommand();
                }
            }

            return EventResult.Handled;
        }

        public override EventResult OnMouseUp(MouseEventArgs e)
        {
            if (_step == Step.Step1_SelectObjects)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (_mgr.presenter.selections.Count > 0)
                    {
                        _step = Step.Step2_SpecifyBasePoint;
                        InitItems();

                        this.pointer.mode = Pointer.Mode.Locate;
                    }
                    else
                    {
                        _mgr.CancelCurrentCommand();
                    }
                }
            }
            else if (_step == Step.Step2_SpecifyBasePoint)
            {
            }
            else if (_step == Step.Step3_SpecifySecondPoint)
            {
            }

            return EventResult.Handled;
        }

        public override EventResult OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return EventResult.Unhandled;
            }

            if (_step == Step.Step1_SelectObjects)
            {
            }
            else if (_step == Step.Step2_SpecifyBasePoint)
            {
            }
            else if (_step == Step.Step3_SpecifySecondPoint)
            {
                LitMath.Vector2 preTranslation = this.translation;
                _pathLine.endPoint = this.pointer.currentSnapPoint;
                LitMath.Vector2 offset = this.translation - preTranslation;
                foreach (Entity copy in _copys)
                {
                    copy.Translate(offset);
                }
            }

            return EventResult.Handled;
        }

        public override EventResult OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (_copys.Count > 0)
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
            switch (_step)
            {
                case Step.Step1_SelectObjects:
                    break;

                case Step.Step2_SpecifyBasePoint:
                    break;

                case Step.Step3_SpecifySecondPoint:
                    foreach (Entity copyItem in _copys)
                    {
                        _mgr.presenter.DrawEntity(g, copyItem);
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
