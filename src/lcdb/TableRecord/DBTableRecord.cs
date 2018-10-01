using System;
using System.Collections.Generic;

namespace LitCAD.DatabaseServices
{
    /// <summary>
    /// 命名的数据库对象
    /// </summary>
    public abstract class DBTableRecord : DBObject
    {
        /// <summary>
        /// 名称
        /// </summary>
        protected string _name = "";
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// 数据表
        /// </summary>
        internal DBTable _dbtable = null;
        public override DBTable dbtable
        {
            get { return _dbtable; }
        }

        /// <summary>
        /// 克隆函数
        /// </summary>
        public override object Clone()
        {
            DBTableRecord tblRec = base.Clone() as DBTableRecord;
            tblRec._name = _name;
            tblRec._dbtable = null;
            return tblRec;
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected override void _Erase()
        {
            if (_dbtable != null)
            {
                _dbtable.Remove(this);
            }
        }
    }
}
