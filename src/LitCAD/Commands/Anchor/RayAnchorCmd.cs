using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Anchor
{
    /// <summary>
    /// 射线锚点命令
    /// </summary>
    internal class RayAnchorCmd : AnchorCmd
    {
        /// <summary>
        /// 锚点
        /// </summary>
        private RayAnchor _rayAnchor;
        protected override EntityAnchor anchor
        {
            get { return _rayAnchor; }
        }

        /// <summary>
        /// 图元拷贝
        /// </summary>
        private Ray _src;
        private Ray _result;

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
        public RayAnchorCmd(EntityAnchor rayAnchor)
        {
            _rayAnchor = rayAnchor as RayAnchor;
            _src = _rayAnchor.ray.Clone() as Ray;
            _result = _src.Clone() as Ray;
        }

        /// <summary>
        /// 提交到数据库
        /// </summary>
        protected override void Commit()
        {
            _rayAnchor.ray.basePoint = _result.basePoint;
            _rayAnchor.ray.direction = _result.direction;
        }

        /// <summary>
        /// 回滚撤销
        /// </summary>
        protected override void Rollback()
        {
            _rayAnchor.ray.basePoint = _src.basePoint;
            _rayAnchor.ray.direction = _src.direction;
        }

        public override EventResult OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            switch (_rayAnchor.type)
            {
                case RayAnchor.Type.BasePoint:
                    _result.basePoint = _mousePosInModel;
                    break;

                case RayAnchor.Type.DirectionPoint:
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
