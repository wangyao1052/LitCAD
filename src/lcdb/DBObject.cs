using System;
using System.Collections.Generic;
using System.Text;

namespace LitCAD.DatabaseServices
{
    public abstract class DBObject : ICloneable
    {
        /// <summary>
        /// ID
        /// </summary>
        protected ObjectId _id = ObjectId.Null;
        public ObjectId id
        {
            get { return _id; }
        }

        internal void SetId(ObjectId newId)
        {
            _id = newId;
        }

        /// <summary>
        /// 数据库
        /// </summary>
        public virtual Database database
        {
            get
            {
                if (dbtable != null)
                {
                    return dbtable.database;
                }

                return null;
            }
        }

        /// <summary>
        /// 数据表
        /// </summary>
        public virtual DBTable dbtable
        {
            get
            {
                if (_parent == null)
                    return null;

                DBObject parentObj = _parent;
                while (parentObj != null)
                {
                    if (parentObj is DBTableRecord)
                    {
                        DBTableRecord tblRec = parentObj as DBTableRecord;
                        return tblRec.dbtable;
                    }

                    parentObj = parentObj.parent;
                }

                return null;
            }
        }

        /// <summary>
        /// 父物体
        /// </summary>
        protected DBObject _parent = null;
        public DBObject parent
        {
            get { return _parent; }
        }

        public ObjectId parentId
        {
            get
            {
                if (_parent == null)
                    return ObjectId.Null;
                return _parent.id;
            }
        }

        internal void SetParent(DBObject parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DBObject()
        {
        }

        /// <summary>
        /// 克隆函数
        /// </summary>
        public virtual object Clone()
        {
            DBObject dbobj = this.CreateInstance();
            dbobj._id = ObjectId.Null;
            dbobj._parent = null;
            return dbobj;
        }

        protected abstract DBObject CreateInstance();

        /// <summary>
        /// 移除
        /// </summary>
        public void Erase()
        {
            Database db = this.database;
            if (db != null)
            {
                db.UnmapObject(this);
                _Erase();
                this._id = ObjectId.Null;
            }
        }

        protected virtual void _Erase()
        {
        }
    }
}
