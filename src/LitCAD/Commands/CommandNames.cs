using System;

namespace LitCAD.Commands
{
    internal class CommandNames
    {
        // 绘制
        public static string Draw_Line      = "line";
        public static string Draw_Xline     = "xline";
        public static string Draw_Ray       = "ray";
        public static string Draw_Polyline  = "polyline";
        public static string Draw_Polygon   = "polygon";
        public static string Draw_Rectangle = "rectangle";
        public static string Draw_Circle    = "circle";
        public static string Draw_Arc       = "arc";

        // 编辑
        public static string Edit_Redo = "redo";
        public static string Edit_Undo = "undo";

        // 修改
        public static string Modify_Delete = "delete";
        public static string Modify_Copy   = "copy";
        public static string Modify_Mirror = "mirror";
        public static string Modify_Offset = "offset";
        public static string Modify_Move   = "move";
    }
}
