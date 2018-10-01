using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD.ApplicationServices
{
    public struct Selection
    {
        private ObjectId _objectId;
        public ObjectId objectId
        {
            get { return _objectId; }
            set { _objectId = value; }
        }

        private LitMath.Vector2 _position;
        public LitMath.Vector2 position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Selection(ObjectId objectId, LitMath.Vector2 pickPosition)
        {
            _objectId = objectId;
            _position = pickPosition;
        }
    }
}
