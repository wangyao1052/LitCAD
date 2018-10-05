using System;

namespace LitCAD.DatabaseServices
{
    /// <summary>
    /// 对象捕捉模式
    /// </summary>
    public enum ObjectSnapMode
    {
        // 未定义
        Undefined = 0,
        // 端点
        End = 1,
        // 中点
        Mid = 2,
        // 中心点
        Center = 4,
        // 节点
        Node = 8,
        // 象限点
        Quad = 16,
        // 插入点
        Ins = 32,
        // 垂足
        Perpendicular = 64,
        // 切点
        Tangent = 128,
        // 最近点
        Near = 256,
    }
}
