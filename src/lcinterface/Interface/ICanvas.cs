using System;
using System.Collections.Generic;
using System.Text;

namespace LitCAD
{
    /// <summary>
    /// 画布接口
    /// </summary>
    public interface ICanvas
    {
        // 画布宽
        double width { get; }
        // 画布高
        double height { get; }

        /// <summary>
        /// 设置Presenter
        /// </summary>
        void SetPresenter(IPresenter controller);

        /// <summary>
        /// 添加子元素
        /// </summary>
        void AddChild(object child);

        /// <summary>
        /// 删除子元素
        /// </summary>
        void RemoveChild(object child);

        /// <summary>
        /// 重绘
        /// </summary>
        void Repaint();
        void Repaint(double x, double y, double width, double height);

        /// <summary>
        /// 获取与设置鼠标位置
        /// </summary>
        LitMath.Vector2 GetMousePosition();
        void SetMousePosition(LitMath.Vector2 postion);
    }
}
