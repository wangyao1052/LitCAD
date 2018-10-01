using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD.UI
{
    internal class DynamicInputer
    {
        private Presenter _presenter = null;

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool active
        {
            get
            {
                return _currInputCtrl != null;
            }
        }

        /// <summary>
        /// 是否独占
        /// </summary>
        public bool exclusive
        {
            get
            {
                return _currInputCtrl != null ? _currInputCtrl.exclusive : false;
            }
        }

        /// <summary>
        /// 位置
        /// </summary>
        private LitMath.Vector2 _position = new LitMath.Vector2();
        public LitMath.Vector2 position
        {
            get { return _position; }
            set 
            {
                _position = value;
                if (_currInputCtrl != null)
                {
                    _currInputCtrl.position = _position;
                }
            }
        }

        /// <summary>
        /// 动态输入控件
        /// </summary>
        private DynInputCtrl _currInputCtrl = null;
        private DynInputString _cmdInput = null;
        public DynInputString cmdInput
        {
            get { return _cmdInput; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="presenter"></param>
        public DynamicInputer(Presenter presenter)
        {
            _presenter = presenter;

            _cmdInput = new DynInputString(_presenter, "");
        }

        /// <summary>
        /// 启动命令动态输入
        /// </summary>
        public bool StartCmd(KeyEventArgs e)
        {
            // 非字符则返回false
            if ((uint)e.KeyCode < 65
                || (uint)e.KeyCode > 90)
            {
                return false;
            }

            //
            _currInputCtrl = _cmdInput;
            _cmdInput.position = _position;
            _cmdInput.text = KeyDataToString(e.KeyData);
            _cmdInput.Start();

            return true;
        }

        public bool StartInput(DynInputCtrl inputCtrl)
        {
            _currInputCtrl = inputCtrl;
            _currInputCtrl.position = _position;
            _currInputCtrl.Start();

            return true;
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            this.position = new LitMath.Vector2(e.X, e.Y);
        }

        /// <summary>
        /// 键值转换为字符串
        /// </summary>
        #region
        [DllImport("user32.dll")]
        static extern int MapVirtualKey(uint uCode, uint uMapType);

        public static string KeyDataToString(Keys keydata)
        {
            int nonVirtualKey = MapVirtualKey((uint)keydata, 2);
            char mappedChar = Convert.ToChar(nonVirtualKey);

            return mappedChar.ToString();
        }
        #endregion
    }
}
