using System;
using System.Collections.Generic;

namespace LitCAD.DatabaseServices
{
    public class BlockTable : DBTable
    {
        internal BlockTable(Database db)
            : base(db, Database.BlockTableId)
        {
        }
    }
}
