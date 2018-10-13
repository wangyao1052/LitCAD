using System;

namespace LitCAD
{
    public enum TextAlignment
    {
        LeftBottom = 0,
        LeftMiddle = 1,
        LeftTop = 2,
        CenterBottom = 3,
        CenterMiddle = 4,
        CenterTop = 5,
        RightBottom = 6,
        RightMiddle = 7,
        RightTop = 8,
    }

    public interface IGraphicsDraw
    {
        void DrawLine(LitMath.Vector2 startPoint, LitMath.Vector2 endPoint);

        void DrawXLine(LitMath.Vector2 basePoint, LitMath.Vector2 direction);

        void DrawRay(LitMath.Vector2 basePoint, LitMath.Vector2 direction);

        void DrawCircle(LitMath.Vector2 center, double radius);

        /// <summary>
        /// 绘制圆弧
        /// 以逆时针方式绘制
        /// </summary>
        /// <param name="center">圆弧中心</param>
        /// <param name="radius">圆弧半径</param>
        /// <param name="startAngle">起始角度(弧度)</param>
        /// <param name="endAngle">结束角度(弧度)</param>
        void DrawArc(LitMath.Vector2 center, double radius, double startAngle, double endAngle);

        void DrawRectangle(LitMath.Vector2 position, double width, double height);

        LitMath.Vector2 DrawText(LitMath.Vector2 position, string text, double height, string font, TextAlignment textAlign);
    }
}
