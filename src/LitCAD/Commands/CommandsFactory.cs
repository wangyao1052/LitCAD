using System;
using System.Collections.Generic;

namespace LitCAD.Commands
{
    internal class CommandsFactory
    {
        private Dictionary<string, CommandProxyBase> _cmdName2Proxy 
            = new Dictionary<string, CommandProxyBase>();

        private void RegisterCommand(string cmdName, CommandProxyBase proxy)
        {
            _cmdName2Proxy[cmdName] = proxy;
        }

        public CommandsFactory()
        {
            // Draw
            RegisterCommand(CommandNames.Draw_Line,
                new CommandProxy<Draw.LinesChainCmd>());
            RegisterCommand(CommandNames.Draw_Xline,
                new CommandProxy<Draw.XlineCmd>());
            RegisterCommand(CommandNames.Draw_Ray,
                new CommandProxy<Draw.RayCmd>());
            RegisterCommand(CommandNames.Draw_Polyline,
                new CommandProxy<Draw.PolylineCmd>());
            RegisterCommand(CommandNames.Draw_Polygon,
                new CommandProxy<Draw.PolygonCmd>());
            RegisterCommand(CommandNames.Draw_Rectangle,
                new CommandProxy<Draw.RectangleCmd>());
            RegisterCommand(CommandNames.Draw_Circle,
                new CommandProxy<Draw.CircleCmd>());
            RegisterCommand(CommandNames.Draw_Arc,
                new CommandProxy<Draw.ArcCmd>());

            // Edit
            RegisterCommand(CommandNames.Edit_Redo,
                new CommandProxy<Edit.RedoCmd>());
            RegisterCommand(CommandNames.Edit_Undo,
                new CommandProxy<Edit.UndoCmd>());

            // Modify
            RegisterCommand(CommandNames.Modify_Delete,
                new CommandProxy<Modify.DeleteCmd>());
            RegisterCommand(CommandNames.Modify_Copy,
                new CommandProxy<Modify.CopyCmd>());
            RegisterCommand(CommandNames.Modify_Mirror,
                new CommandProxy<Modify.MirrorCmd>());
            RegisterCommand(CommandNames.Modify_Offset,
                new CommandProxy<Modify.OffsetCmd>());
            RegisterCommand(CommandNames.Modify_Move,
                new CommandProxy<Modify.MoveCmd>());
        }

        /// <summary>
        /// 新建命令
        /// </summary>
        public Command NewCommand(string cmdName)
        {
            if (_cmdName2Proxy.ContainsKey(cmdName))
            {
                return _cmdName2Proxy[cmdName].NewCmd();
            }
            else
            {
                return null;
            }
        }

        private abstract class CommandProxyBase
        {
            public abstract Command NewCmd();
        }

        private class CommandProxy<T> : CommandProxyBase where T : Command, new()
        {
            public override Command NewCmd()
            {
                return new T();
            }
        }
    }
}
