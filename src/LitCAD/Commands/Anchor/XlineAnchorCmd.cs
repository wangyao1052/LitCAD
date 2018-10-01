using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Anchor
{
    /// <summary>
    /// 构造线锚点命令
    /// </summary>
    internal class XlineAnchorCmd : AnchorCmd
    {
        /// <summary>
        /// 锚点
        /// </summary>
        private XlineAnchor _xlineAnchor;
        protected override EntityAnchor anchor
        {
            get { return _xlineAnchor; }
        }

        /// <summary>
        /// 图元拷贝
        /// </summary>
        private Xline _src;
        private Xline _result;

        protected override Entity _originalEntityCopy
        {
            get { return _src; }
        }

        protected override Entity _resultEntityCopy
        {
            get { return _result; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public XlineAnchorCmd(EntityAnchor xlineAnchor)
        {
            _xlineAnchor = xlineAnchor as XlineAnchor;
            _src = _xlineAnchor.xline.Clone() as Xline;
            _result = _src.Clone() as Xline;
        }

        /// <summary>
        /// 提交到数据库
        /// </summary>
        protected override void Commit()
        {
            _xlineAnchor.xline.basePoint = _result.basePoint;
            _xlineAnchor.xline.direction = _result.direction;
        }

        /// <summary>
        /// 回滚撤销
        /// </summary>
        protected override void Rollback()
        {
            _xlineAnchor.xline.basePoint = _src.basePoint;
            _xlineAnchor.xline.direction = _src.direction;
        }

        public override EventResult OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            switch (_xlineAnchor.type)
            {
                case XlineAnchor.Type.BasePoint:
                    _result.basePoint = _mousePosInModel;
                    break;

                case XlineAnchor.Type.DirectionPoint:
                case XlineAnchor.Type.DirectionPointMinus:
                    LitMath.Vector2 dir = (_mousePosInModel - _result.basePoint).normalized;
                    if (dir.x != 0 && dir.y != 0)
                    {
                        _result.direction = dir;
                    }
                    break;
            }

            return EventResult.Handled;
        }
    }
}
