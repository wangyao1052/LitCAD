using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD.UI
{
    internal class Pointer
    {
        private Presenter _presenter = null;

        /// <summary>
        /// 模式
        /// </summary>
        internal enum Mode
        {
            // 默认模式
            Default = 0,
            // 选择模式
            Select = 1,
            // 定位模式
            Locate = 2,
            // 拖动模式
            Drag = 3,
        }
        private Mode _mode = Mode.Default;
        internal Mode mode
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    UpdateBitmap();
                }
            }
        }

        /// <summary>
        ///  中心位置
        ///  Canvas CSYS
        /// </summary>
        private LitMath.Vector2 _pos = new LitMath.Vector2(0, 0);
        internal LitMath.Vector2 position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        /// <summary>
        /// 捕捉节点位置
        /// Model CSYS
        /// </summary>
        private LitMath.Vector2 _currSnapPoint = new LitMath.Vector2(0, 0);
        internal LitMath.Vector2 currentSnapPoint
        {
            get { return _currSnapPoint; }
        }

        /// <summary>
        /// 尺寸
        /// </summary>
        private PickupBox _pickupBox = null;
        private SelectRectangle _selRect = null;

        private LocateCross _locateCross = null;

        private SnapNodesMgr _snapNodesMgr = null;
        private AnchorsMgr _anchorMgr = null;

        internal int pickupBoxSide
        {
            get { return _pickupBox.side; }
            set
            {
                if (_pickupBox.side != value)
                {
                    _pickupBox.side = value;
                    UpdateBitmap();
                }
            }
        }

        internal int locateCrossLength
        {
            get { return _locateCross.length; }
            set
            {
                if (_locateCross.length != value)
                {
                    _locateCross.length = value;
                    UpdateBitmap();
                }
            }
        }

        private bool _isShowAnchor = true;
        internal bool isShowAnchor
        {
            get { return _isShowAnchor; }
            set
            {
                if (_isShowAnchor != value)
                {
                    _anchorMgr.Clear();
                    _isShowAnchor = value;
                    if (_isShowAnchor)
                    {
                        _anchorMgr.Update();
                    }
                }
            }
        }

        /// <summary>
        /// 位图
        /// </summary>
        private Bitmap _bitmap = null;
        private void UpdateBitmap()
        {
            _bitmap = new Bitmap(_locateCross.length, _locateCross.length);
            Graphics graphics = Graphics.FromImage(_bitmap);

            Pen pen = GDIResMgr.Instance.GetPen(Color.White, 1);

            if (_mode == Mode.Default || _mode == Mode.Select)
            {
                graphics.DrawRectangle(pen,
                (_bitmap.Width - _pickupBox.side) / 2, (_bitmap.Height - _pickupBox.side) / 2,
                _pickupBox.side, _pickupBox.side);
            }

            if (_mode == Mode.Default || _mode == Mode.Locate)
            {
                graphics.DrawLine(pen,
                _bitmap.Width / 2, 0,
                _bitmap.Width / 2, _bitmap.Height);
                graphics.DrawLine(pen,
                    0, _bitmap.Height / 2,
                    _bitmap.Width, _bitmap.Height / 2);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        internal Pointer(Presenter presenter)
        {
            _presenter = presenter;

            _pickupBox = new PickupBox(_presenter);
            _pickupBox.side = 20;

            _locateCross = new LocateCross(_presenter);
            _locateCross.length = 60;

            _snapNodesMgr = new SnapNodesMgr(_presenter);
            _anchorMgr = new AnchorsMgr(_presenter);

            UpdateBitmap();
        }

        internal Commands.Command OnMouseDown(MouseEventArgs e)
        {
            _pos.x = e.X;
            _pos.y = e.Y;
            Commands.Command cmd = null;

            switch (_mode)
            {
                case Mode.Default:
                    #region
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            if (_anchorMgr.currentGripPoint == null)
                            {
                                _pickupBox.center = _pos;
                                List<Selection> sels = _pickupBox.Select(_presenter.currentBlock);
                                if (sels.Count > 0)
                                {
                                    if (IsShiftKeyDown)
                                    {
                                        (_presenter.document as Document).selections.Remove(sels);
                                    }
                                    else
                                    {
                                        (_presenter.document as Document).selections.Add(sels);
                                    }
                                }
                                else
                                {
                                    _selRect = new SelectRectangle(_presenter);
                                    _selRect.startPoint = _selRect.endPoint = _pos;
                                }
                            }
                            else
                            {
                                Database db = (_presenter.document as Document).database;
                                Entity entity = db.GetObject(_anchorMgr.currentGripEntityId) as Entity;
                                if (entity != null)
                                {
                                    LitCAD.Commands.GripPointMoveCmd gripMoveCmd = new Commands.GripPointMoveCmd(
                                        entity, _anchorMgr.currentGripPointIndex, _anchorMgr.currentGripPoint);
                                    cmd = gripMoveCmd;
                                }

                                
                                //Commands.Command anchorCmd = _anchorMgr.currentAnchorCmd;
                                //if (anchorCmd != null)
                                //{
                                //    cmd = anchorCmd;
                                //}
                            }
                        }
                    }
                    #endregion
                    break;

                case Mode.Select:
                    #region
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            _pickupBox.center = _pos;
                            List<Selection> sels = _pickupBox.Select(_presenter.currentBlock);
                            if (sels.Count > 0)
                            {
                                if (IsShiftKeyDown)
                                {
                                    (_presenter.document as Document).selections.Remove(sels);
                                }
                                else
                                {
                                    (_presenter.document as Document).selections.Add(sels);
                                }
                            }
                            else
                            {
                                _selRect = new SelectRectangle(_presenter);
                                _selRect.startPoint = _selRect.endPoint = _pos;
                            }
                        }
                    }
                    #endregion
                    break;

                case Mode.Locate:
                    _currSnapPoint = _snapNodesMgr.Snap(_pos);
                    break;

                case Mode.Drag:
                    break;

                default:
                    break;
            }

            return cmd;
        }

        internal void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_selRect != null)
                {
                    List<Selection> sels = _selRect.Select(_presenter.currentBlock);
                    if (sels.Count > 0)
                    {
                        if (IsShiftKeyDown)
                        {
                            (_presenter.document as Document).selections.Remove(sels);
                        }
                        else
                        {
                            (_presenter.document as Document).selections.Add(sels);
                        }
                        
                    }
                }
                _selRect = null;
            }
        }

        internal void OnMouseMove(MouseEventArgs e)
        {
            _pos.x = e.X;
            _pos.y = e.Y;

            switch (_mode)
            {
                case Mode.Default:
                    if (_selRect != null)
                    {
                        _selRect.endPoint = _pos;
                        _presenter.RepaintCanvas();
                    }
                    else
                    {
                        _currSnapPoint = _anchorMgr.Snap(_pos);
                    }
                    break;

                case Mode.Select:
                    if (_selRect != null)
                    {
                        _selRect.endPoint = _pos;
                        _presenter.RepaintCanvas();
                    }
                    break;

                case Mode.Locate:
                    _currSnapPoint = _snapNodesMgr.Snap(_pos);
                    break;

                case Mode.Drag:
                    break;

                default:
                    break;
            }

            _presenter.RepaintCanvas();
        }

        internal void OnMouseDoubleClick(MouseEventArgs e)
        {
            switch (mode)
            {
                case Mode.Default:
                    if (e.Button == MouseButtons.Left)
                    {
                        if (_anchorMgr.currentGripPoint == null)
                        {
                            _pickupBox.center = _pos;
                            List<Selection> sels = _pickupBox.Select(_presenter.currentBlock);
                            if (sels.Count > 0)
                            {
                                foreach (Selection sel in sels)
                                {
                                    DBObject dbobj = (_presenter.document as Document).database.GetObject(sel.objectId);
                                    if (dbobj != null && dbobj is Text)
                                    {
                                        (_presenter.document as Document).selections.Clear();
                                    }
                                }
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        internal bool OnKeyDown(KeyEventArgs e)
        {
            return false;
        }

        internal bool OnKeyUp(KeyEventArgs e)
        {
            return false;
        }

        internal void OnPaint(Graphics graphics)
        {
            if (_isShowAnchor)
            {
                _anchorMgr.OnPaint(graphics);
            }

            switch (_mode)
            {
                case Mode.Default:
                    {
                        if (_selRect != null)
                        {
                            _selRect.OnPaint(graphics);
                        }
                        else
                        {
                            LitMath.Vector2 currSnapPointInCanvas = _presenter.ModelToCanvas(_currSnapPoint);
                            graphics.DrawImage(_bitmap,
                                (float)(currSnapPointInCanvas.x - _bitmap.Width / 2),
                                (float)(currSnapPointInCanvas.y - _bitmap.Height / 2));
                        }
                    }
                    break;

                case Mode.Select:
                    if (_selRect != null)
                    {
                        _selRect.OnPaint(graphics);
                    }
                    else
                    {
                        graphics.DrawImage(_bitmap,
                            (float)(_pos.x - _bitmap.Width / 2),
                            (float)(_pos.y - _bitmap.Height / 2));
                    }
                    break;

                case Mode.Locate:
                    {
                        LitMath.Vector2 currSnapPointInCanvas = _presenter.ModelToCanvas(_currSnapPoint);
                        graphics.DrawImage(_bitmap,
                            (float)(currSnapPointInCanvas.x - _bitmap.Width / 2),
                            (float)(currSnapPointInCanvas.y - _bitmap.Height / 2));

                        _presenter.canvasDraw.graphics = graphics;
                        _snapNodesMgr.OnPaint(_presenter.canvasDraw);
                    }
                    break;

                case Mode.Drag:
                    break;

                default:
                    break;
            }
        }

        internal void OnSelectionChanged()
        {
            if (_isShowAnchor)
            {
                _anchorMgr.Update();
            }
        }

        internal void UpdateGripPoints()
        {
            _anchorMgr.Clear();
            if (_isShowAnchor)
            {
                _anchorMgr.Update();
            }
        }

        private bool IsShiftKeyDown
        {
            get
            {
                return (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            }
        }
    }
}
