using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LitCAD.Commands
{
    /// <summary>
    /// 命令管理器
    /// </summary>
    internal class CommandsMgr
    {
        /// <summary>
        /// Presenter
        /// </summary>
        private Presenter _presenter = null;
        public Presenter presenter
        {
            get { return _presenter; }
        }

        /// <summary>
        /// 命令完成事件
        /// </summary>
        public delegate void CommandEvent(Command cmd);
        public event CommandEvent commandFinished;
        public event CommandEvent commandCanceled;

        /// <summary>
        /// 命令列表
        /// </summary>
        private List<Command> _undoCmds = new List<Command>();
        private List<Command> _redoCmds = new List<Command>();

        internal bool canUndo
        {
            get { return _undoCmds.Count > 0; }
        }

        internal bool canRedo
        {
            get { return _redoCmds.Count > 0; }
        }

        /// <summary>
        /// 当前命令
        /// </summary>
        private Command _currentCmd = null;
        public Command CurrentCmd
        {
            get { return _currentCmd; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CommandsMgr(Presenter presenter)
        {
            _presenter = presenter;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void DoCommand(Command cmd)
        {
            if (_currentCmd != null)
            {
                return;
            }

            _currentCmd = cmd;
            _currentCmd.cmdMgr = this;
            _currentCmd.Initialize();
        }

        /// <summary>
        /// 完成当前命令
        /// </summary>
        public void FinishCurrentCommand()
        {
            if (_currentCmd != null)
            {
                _currentCmd.Finish();
                if (_currentCmd is Edit.UndoCmd)
                {
                    this.Undo();
                }
                else if (_currentCmd is Edit.RedoCmd)
                {
                    this.Redo();
                }
                else
                {
                    _undoCmds.Add(_currentCmd);
                    _redoCmds.Clear();
                }

                commandFinished.Invoke(_currentCmd);
                _currentCmd = null;
            }
        }

        /// <summary>
        /// 取消当前命令
        /// </summary>
        public void CancelCurrentCommand()
        {
            if (_currentCmd != null)
            {
                _currentCmd.Cancel();

                commandCanceled.Invoke(_currentCmd);
                _currentCmd = null;
            }
        }

        /// <summary>
        /// 撤销
        /// </summary>
        private void Undo()
        {
            if (_undoCmds.Count == 0)
            {
                return;
            }

            Command cmd = _undoCmds[_undoCmds.Count - 1];
            _undoCmds.RemoveAt(_undoCmds.Count - 1);
            cmd.Undo();
            _redoCmds.Add(cmd);
        }

        /// <summary>
        /// 重做
        /// </summary>
        private void Redo()
        {
            if (_redoCmds.Count == 0)
            {
                return;
            }

            Command cmd = _redoCmds[_redoCmds.Count - 1];
            _redoCmds.RemoveAt(_redoCmds.Count - 1);
            cmd.Redo();
            _undoCmds.Add(cmd);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        public void OnPaint(Graphics g)
        {
            if (_currentCmd != null)
            {
                _currentCmd.OnPaint(g);
            }
        }

        /// <summary>
        /// Mouse Down
        /// </summary>
        public void OnMouseDown(MouseEventArgs e)
        {
            if (_currentCmd != null)
            {
                _currentCmd.OnMouseDown(e);
            }
        }

        /// <summary>
        /// Mouse Up
        /// </summary>
        public void OnMouseUp(MouseEventArgs e)
        {
            if (_currentCmd != null)
            {
                _currentCmd.OnMouseUp(e);
            }
        }

        /// <summary>
        /// Mouse Move
        /// </summary>
        public void OnMouseMove(MouseEventArgs e)
        {
            if (_currentCmd != null)
            {
                _currentCmd.OnMouseMove(e);
            }
        }

        /// <summary>
        /// Key Down
        /// </summary>
        public void OnKeyDown(KeyEventArgs e)
        {
            if (_currentCmd != null)
            {
                Command.EventResult eRet = _currentCmd.OnKeyDown(e);
                if (eRet.status == Command.EventResultStatus.Unhandled)
                {
                    if (e.KeyCode == Keys.Escape)
                    {
                        this.CancelCurrentCommand();
                    }
                }
            }
        }

        /// <summary>
        /// Key Up
        /// </summary>
        public void OnKeyUp(KeyEventArgs e)
        {
            if (_currentCmd != null)
            {
                Command.EventResult eRet = _currentCmd.OnKeyUp(e);
            }
        }
    }
}
