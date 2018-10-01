using System;
using System.Collections.Generic;

namespace LitCAD.DatabaseServices
{
    internal class ObjectIdMgr
    {
        private Database _database;
        private uint _currentId = 10000;

        internal ObjectIdMgr(Database db)
        {
            _database = db;
        }

        internal ObjectId NextId
        {
            get { return new ObjectId(++_currentId); }
        }
    }
}
