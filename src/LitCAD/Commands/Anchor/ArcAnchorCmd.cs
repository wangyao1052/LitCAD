using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Anchor
{
    /// <summary>
    /// 圆弧锚点命令
    /// </summary>
    internal class ArcAnchorCmd : AnchorCmd
    {
        /// <summary>
        /// 锚点
        /// </summary>
        private ArcAnchor _arcAnchor;
        protected override EntityAnchor anchor
        {
            get { return _arcAnchor; }
        }

        /// <summary>
        /// 图元拷贝
        /// </summary>
        private Arc _originalArcCopy;
        private Arc _resultArcCopy;

        protected override Entity _originalEntityCopy
        {
            get { return _originalArcCopy; }
        }

        protected override Entity _resultEntityCopy
        {
            get { return _resultArcCopy; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ArcAnchorCmd(EntityAnchor arcAnchor)
        {
            _arcAnchor = arcAnchor as ArcAnchor;
            _originalArcCopy = _arcAnchor.arc.Clone() as Arc;
            _resultArcCopy = _arcAnchor.arc.Clone() as Arc; ;

            _arcStartPoint = _arcAnchor.arc.startPoint;
            _arcEndPoint = _arcAnchor.arc.endPoint;
            _arcMiddlePoint = DBUtils.ArcUtils.ArcMiddlePoint(_arcAnchor.arc);
        }

        private LitMath.Vector2 _arcStartPoint;
        private LitMath.Vector2 _arcEndPoint;
        private LitMath.Vector2 _arcMiddlePoint;

        /// <summary>
        /// 撤销
        /// </summary>
        public override void Undo()
        {
            base.Undo();

            _arcAnchor.arc.center = _originalArcCopy.center;
            _arcAnchor.arc.radius = _originalArcCopy.radius;
            _arcAnchor.arc.startAngle = _originalArcCopy.startAngle;
            _arcAnchor.arc.endAngle = _originalArcCopy.endAngle;
        }

        /// <summary>
        /// 重做
        /// </summary>
        public override void Redo()
        {
            base.Redo();

            _arcAnchor.arc.center = _resultArcCopy.center;
            _arcAnchor.arc.radius = _resultArcCopy.radius;
            _arcAnchor.arc.startAngle = _resultArcCopy.startAngle;
            _arcAnchor.arc.endAngle = _resultArcCopy.endAngle;
        }

        /// <summary>
        /// 完成
        /// </summary>
        public override void Finish()
        {
            _arcAnchor.arc.center = _resultArcCopy.center;
            _arcAnchor.arc.radius = _resultArcCopy.radius;
            _arcAnchor.arc.startAngle = _resultArcCopy.startAngle;
            _arcAnchor.arc.endAngle = _resultArcCopy.endAngle;

            base.Finish();
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public override void Cancel()
        {
            base.Cancel();
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        public override EventResult OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            switch (_arcAnchor.type)
            {
                case ArcAnchor.Type.Center:
                    _resultArcCopy.center = _mousePosInModel;
                    break;

                case ArcAnchor.Type.Start:
                case ArcAnchor.Type.End:
                case ArcAnchor.Type.Middle:
                    LitMath.Vector2 startPoint = _arcStartPoint;
                    LitMath.Vector2 endPoint = _arcEndPoint;
                    LitMath.Vector2 middlePoint = _arcMiddlePoint;
                    if (_arcAnchor.type == ArcAnchor.Type.Start)
                    {
                        startPoint = _mousePosInModel;
                    }
                    else if (_arcAnchor.type == ArcAnchor.Type.End)
                    {
                        endPoint = _mousePosInModel;
                    }
                    else if (_arcAnchor.type == ArcAnchor.Type.Middle)
                    {
                        middlePoint = _mousePosInModel;
                    }

                    LitMath.Circle2 newCircle = LitMath.Circle2.From3Points(
                        startPoint, middlePoint, endPoint);
                    if (newCircle.radius > 0)
                    {
                        LitMath.Vector2 xPositive = new LitMath.Vector2(1, 0);
                        double startAngle = LitMath.Vector2.SignedAngleInRadian(xPositive,
                            startPoint - newCircle.center);
                        double endAngle = LitMath.Vector2.SignedAngleInRadian(xPositive,
                            endPoint - newCircle.center);
                        double middleAngle = LitMath.Vector2.SignedAngleInRadian(xPositive,
                            middlePoint - newCircle.center);
                        startAngle = MathUtils.NormalizeRadianAngle(startAngle);
                        endAngle = MathUtils.NormalizeRadianAngle(endAngle);
                        middleAngle = MathUtils.NormalizeRadianAngle(middleAngle);

                        _resultArcCopy.center = newCircle.center;
                        _resultArcCopy.radius = newCircle.radius;
                        if (AngleInArcRange(middleAngle, startAngle, endAngle))
                        {
                            _resultArcCopy.startAngle = startAngle;
                            _resultArcCopy.endAngle = endAngle;
                        }
                        else
                        {
                            _resultArcCopy.startAngle = endAngle;
                            _resultArcCopy.endAngle = startAngle;
                        }
                    }
                    break;
            }

            return EventResult.Handled;
        }

        private bool AngleInArcRange(double angle, double startAngle, double endAngle)
        {
            if (endAngle >= startAngle)
            {
                return angle >= startAngle
                    && angle <= endAngle;
            }
            else
            {
                return angle >= startAngle
                    || angle <= endAngle;
            }
        }

        /// <summary>
        /// 绘制路径
        /// </summary>
        protected override void DrawPath(Graphics g)
        {
            base.DrawPath(g);
        }
    }
}
