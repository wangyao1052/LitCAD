using System;
using System.Collections.Generic;

namespace LitCAD.DatabaseServices
{
    public class LayerTable : DBTable
    {
        public ObjectId layerZeroId = ObjectId.Null;

        internal LayerTable(Database db)
            : base(db, Database.LayerTableId)
        {
            Layer layerZero = new Layer("0");
            this.Add(layerZero);

            layerZeroId = layerZero.id;
        }
    }
}
