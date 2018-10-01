using System;
using System.Drawing;
using System.Windows.Forms;

namespace LitCAD.UI
{
    internal class LocateCross
    {
        private Presenter _presenter = null;

        /// <summary>
        /// 长度
        /// </summary>
        private int _length = 20;
        internal int length
        {
            get { return _length; }
            set { _length = value; }
        }

        internal LocateCross(Presenter presenter)
        {
            _presenter = presenter;
        }
    }
}
