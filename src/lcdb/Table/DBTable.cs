using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace LitCAD.DatabaseServices
{
    public abstract class DBTable : DBObject, IEnumerable<DBTableRecord>
    {
        /// <summary>
        /// 类名
        /// </summary>
        public override string className
        {
            get { return "DBTable"; }
        }

        protected List<DBTableRecord> _items = new List<DBTableRecord>();
        protected Dictionary<string, DBTableRecord> _dictName2Item = new Dictionary<string, DBTableRecord>();

        public delegate void ItemAdded(DBTableRecord item);
        public event ItemAdded itemAdded;

        public delegate void ItemRemoved(DBTableRecord item);
        public event ItemRemoved itemRemoved;

        /// <summary>
        /// 数据库
        /// </summary>
        private Database _database = null;
        public override Database database
        {
            get
            {
                return _database;
            }
        }

        /// <summary>
        /// 数据表
        /// </summary>
        public override DBTable dbtable
        {
            get { return this; }
        }

        /// <summary>
        /// 数据表项数目
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        internal DBTable(Database db, ObjectId tableId)
        {
            _database = db;
            _id = tableId;
        }

        /// <summary>
        /// 克隆函数
        /// </summary>
        public override object Clone()
        {
            throw new System.Exception("DBTable can not be cloned.");
        }

        protected override DBObject CreateInstance()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected override void _Erase()
        {
        }

        /// <summary>
        /// 添加表格项
        /// </summary>
        public virtual ObjectId Add(DBTableRecord tblRecord)
        {
            if (tblRecord.id != ObjectId.Null)
            {
                throw new System.Exception("TableRecord is not newly created");
            }

            return _Add(tblRecord);
        }

        protected ObjectId _Add(DBTableRecord tblRecord)
        {
            _items.Add(tblRecord);
            tblRecord._dbtable = this;
            _dictName2Item[tblRecord.name] = tblRecord;
            tblRecord.SetParent(this);
            _database.IdentifyObject(tblRecord);

            if (itemAdded != null)
            {
                itemAdded.Invoke(tblRecord);
            }

            return tblRecord.id;
        }

        /// <summary>
        /// 移除表格项
        /// </summary>
        public virtual void Remove(DBTableRecord tblRecord)
        {
            if (_items.Remove(tblRecord))
            {
                _dictName2Item.Remove(tblRecord.name);

                if (itemRemoved != null)
                {
                    itemRemoved.Invoke(tblRecord);
                }
            }
        }

        /// <summary>
        /// 清空表
        /// </summary>
        internal virtual void Clear()
        {
            _items.Clear();
            _dictName2Item.Clear();
        }

        /// <summary>
        /// 是否包含表格项
        /// </summary>
        public bool Has(string key)
        {
            return _dictName2Item.ContainsKey(key);
        }

        public bool Has(ObjectId id)
        {
            foreach (DBTableRecord item in _items)
            {
                if (item.id == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 根据名称获取表格项
        /// </summary>
        public DBTableRecord this[string key]
        {
            get
            {
                if (this.Has(key))
                {
                    return _dictName2Item[key];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 写XML
        /// </summary>
        public override void XmlOut(Filer.XmlFiler filer)
        {
            Filer.XmlFilerImpl filerImpl = filer as Filer.XmlFilerImpl;

            base.XmlOut(filer);
            foreach (DBTableRecord item in _items)
            {
                filerImpl.NewSubNodeAndInsert(item.className);
                item.XmlOut(filer);
                filerImpl.Pop();
            }
        }

        /// <summary>
        /// 读XML
        /// </summary>
        public override void XmlIn(Filer.XmlFiler filer)
        {
            base.XmlIn(filer);
        }

        #region IEnumerable<DBTableRecord>
        public IEnumerator<DBTableRecord> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
