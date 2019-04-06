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

        internal void reset()
        {
            if (_database != null)
            {
                ObjectId max = _database.currentMaxId;
                if (max.id > 10000)
                {
                    _currentId = max.id;
                }
            }
        }

        internal ObjectId NextId
        {
            get { return new ObjectId(++_currentId); }
        }
    }
}
