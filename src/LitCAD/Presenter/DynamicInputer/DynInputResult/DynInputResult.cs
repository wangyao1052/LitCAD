using System;

namespace LitCAD.UI
{
    /// <summary>
    /// 动态输入返回结果
    /// </summary>
    internal abstract class DynInputResult
    {
        protected DynInputStatus _status = DynInputStatus.OK;
        public DynInputStatus status
        {
            get { return _status; }
        }

        public DynInputResult(DynInputStatus status)
        {
            _status = status;
        }
    }
}
