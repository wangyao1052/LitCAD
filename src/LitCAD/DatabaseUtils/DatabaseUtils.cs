using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD.DBUtils
{
    internal class DatabaseUtils
    {
        private Database _db;

        internal DatabaseUtils(Database db)
        {
            _db = db;
        }

        internal bool IsLayerCanDelete(ObjectId layerId)
        {
            if (layerId == _db.layerTable.layerZeroId)
            {
                return false;
            }

            foreach (Block block in _db.blockTable)
            {
                foreach (Entity entity in block)
                {
                    if (entity.layerId == layerId)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
