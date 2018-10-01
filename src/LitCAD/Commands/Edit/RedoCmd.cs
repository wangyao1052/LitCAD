using System;
using System.Collections.Generic;
using System.Text;

namespace LitCAD.Commands.Edit
{
    internal class RedoCmd : Command
    {
        public override void Initialize()
        {
            base.Initialize();

            _mgr.FinishCurrentCommand();
        }
    }
}
