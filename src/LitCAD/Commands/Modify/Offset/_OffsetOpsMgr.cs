using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD.Commands.Modify.Offset
{
    internal class _OffsetOpsMgr
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static _OffsetOpsMgr _instance = null;
        public static _OffsetOpsMgr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new _OffsetOpsMgr();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 图元类型<>Offset操作类型
        /// </summary>
        private Dictionary<Type, Type> _entType2OffsetOpType = new Dictionary<Type, Type>();

        /// <summary>
        /// 构造函数
        /// </summary>
        private _OffsetOpsMgr()
        {
            RegisterOffsetOpType(typeof(Line), typeof(OffsetLine));
            RegisterOffsetOpType(typeof(Circle), typeof(OffsetCircle));
            RegisterOffsetOpType(typeof(Arc), typeof(OffsetArc));
        }

        /// <summary>
        /// 注册Offset操作类型
        /// </summary>
        private void RegisterOffsetOpType(Type entType, Type offsetOpType)
        {
            _entType2OffsetOpType[entType] = offsetOpType;
        }

        /// <summary>
        /// 新建图元Offset操作对象
        /// </summary>
        public _OffsetOperation NewOffsetOperation(Entity entity)
        {
            Type entType = entity.GetType();
            if (_entType2OffsetOpType.ContainsKey(entType))
            {
                Type offsetOpType = _entType2OffsetOpType[entType];

                object[] parameters = new object[1];
                parameters[0] = entity;
                return Activator.CreateInstance(offsetOpType, parameters) as _OffsetOperation;
            }
            else
            {
                return null;
            }
        }
    }
}
