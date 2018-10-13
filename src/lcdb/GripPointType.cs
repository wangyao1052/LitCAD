using System;

namespace LitCAD.DatabaseServices
{
    /// <summary>
    /// 夹点类型
    /// </summary>
    public enum GripPointType
    {
        // 未定义
        Undefined = 0,
        // 端点
        End = 1,
        // 中点
        Mid = 2,
        // 中心点
        Center = 3,
        // 节点
        Node = 4,
        // 象限点
        Quad = 5,
    }
}
