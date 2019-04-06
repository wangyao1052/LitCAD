using System;
using System.Collections.Generic;

namespace LitCAD.DatabaseServices
{
    public struct ObjectId : IComparable<ObjectId>, IEquatable<ObjectId>
    {
        private uint _id;
        public uint id
        {
            get { return _id; }
        }

        internal ObjectId(uint id = 0)
        {
            _id = id;
        }

        public static ObjectId Null
        {
            get
            {
                return new ObjectId(0);
            }
        }

        public bool isNull
        {
            get { return _id == 0; }
        }

        public override string ToString()
        {
            return _id.ToString();
        }

        internal static bool TryParse(string s, out ObjectId result)
        {
            uint uid = 0;
            if (uint.TryParse(s, out uid))
            {
                result = new ObjectId(uid);
                return true;
            }
            else
            {
                result = ObjectId.Null;
                return false;
            }
        }

        #region IEquatable<ObjectId>
        public override bool Equals(object obj)
        {
            if (!(obj is ObjectId))
                return false;

            return Equals((ObjectId)obj);
        }

        public bool Equals(ObjectId other)
        {
            return _id == other._id;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
        #endregion

        #region IComparable<ObjectId>
        public int CompareTo(ObjectId other)
        {
            return this._id.CompareTo(other._id);
        }
        #endregion

        public static bool operator ==(ObjectId lhs, ObjectId rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ObjectId lhs, ObjectId rhs)
        {
            return !(lhs == rhs);
        }
    }
}
