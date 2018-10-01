using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;
using LitCAD.ApplicationServices;
using LitCAD.UI;

namespace LitCAD.Commands
{
    internal abstract class Command
    {
        /// <summary>
        /// 命令管理器
        /// </summary>
        protected CommandsMgr _mgr = null;
        internal CommandsMgr cmdMgr
        {
            get { return _mgr; }
            set { _mgr = value; }
        }

        internal Presenter presenter
        {
            get { return _mgr.presenter; }
        }

        internal Document document
        {
            get { return _mgr.presenter.document; }
        }

        internal Database database
        {
            get { return _mgr.presenter.document.database; }
        }

        internal Pointer pointer
        {
            get { return _mgr.presenter.pointer; }
        }

        internal DynamicInputer dynamicInputer
        {
            get { return _mgr.presenter.dynamicInputer; }
        }

        protected UI.Pointer.Mode _lastPointerMode;
        protected bool _lastShowAnchor;

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {
            _lastPointerMode = this.pointer.mode;
            _lastShowAnchor = this.pointer.isShowAnchor;

            this.pointer.isShowAnchor = false;
        }

        /// <summary>
        /// 结束
        /// </summary>
        public virtual void Terminate()
        {
            this.pointer.mode = _lastPointerMode;
            this.pointer.isShowAnchor = _lastShowAnchor;
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public virtual void Undo()
        {
            this.Rollback();
        }

        /// <summary>
        /// 重做
        /// </summary>
        public virtual void Redo()
        {
            this.Commit();
        }

        /// <summary>
        /// 完成
        /// </summary>
        public virtual void Finish()
        {
            this.Commit();
            this.Terminate();
            
        }

        /// <summary>
        /// 取消
        /// </summary>
        public virtual void Cancel()
        {
            this.Terminate();
        }

        /// <summary>
        /// 提交到数据库
        /// </summary>
        protected virtual void Commit()
        {
        }

        /// <summary>
        /// 回滚撤销
        /// </summary>
        protected virtual void Rollback()
        {
        }

        /// <summary>
        /// Mouse Down
        /// </summary>
        public virtual EventResult OnMouseDown(MouseEventArgs e)
        {
            return EventResult.Unhandled;
        }

        /// <summary>
        /// Mouse Up
        /// </summary>
        public virtual EventResult OnMouseUp(MouseEventArgs e)
        {
            return EventResult.Unhandled;
        }

        /// <summary>
        /// Mouse Move
        /// </summary>
        public virtual EventResult OnMouseMove(MouseEventArgs e)
        {
            return EventResult.Unhandled;
        }

        /// <summary>
        /// Mouse Wheel
        /// </summary>
        public virtual EventResult OnMouseWheel(MouseEventArgs e)
        {
            return EventResult.Unhandled;
        }

        /// <summary>
        /// Key Down
        /// </summary>
        public virtual EventResult OnKeyDown(KeyEventArgs e)
        {
            return EventResult.Unhandled;
        }

        /// <summary>
        /// Key Up
        /// </summary>
        public virtual EventResult OnKeyUp(KeyEventArgs e)
        {
            return EventResult.Unhandled;
        }

        /// <summary>
        /// Paint
        /// </summary>
        public virtual void OnPaint(Graphics g)
        {
        }

        /// <summary>
        /// 事件处理结果
        /// </summary>
        public class EventResult
        {
            public EventResultStatus status = EventResultStatus.Invalid;
            public object data = null;

            public static EventResult Unhandled
            {
                get
                {
                    EventResult eRet = new EventResult();
                    eRet.status = EventResultStatus.Unhandled;
                    return eRet;
                }
            }

            public static EventResult Handled
            {
                get
                {
                    EventResult eRet = new EventResult();
                    eRet.status = EventResultStatus.Handled;
                    return eRet;
                }
            }
        }

        /// <summary>
        /// 事件结果状态
        /// </summary>
        public enum EventResultStatus
        {
            // 无效
            Invalid = 0,
            // 处理了
            Handled = 1,
            // 未处理
            Unhandled = 2,
        }
    }
}
