using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace LitCAD.Windows.Controls
{
    internal class DynamicInputTextBox : TextBox
    {
        public delegate void MessageHandler(object sender);
        public event MessageHandler keyEscDown;
        public event MessageHandler keySpaceDown;
        public event MessageHandler keyEnterDown;
        public event MessageHandler keyTabDown;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (keyEscDown != null)
                    {
                        keyEscDown.Invoke(this);
                    }
                    break;

                case Keys.Space:
                    if (keySpaceDown != null)
                    {
                        keySpaceDown.Invoke(this);
                    }
                    break;

                case Keys.Enter:
                    if (keyEnterDown != null)
                    {
                        keyEnterDown.Invoke(this);
                    }
                    break;

                case Keys.Tab:
                    if (keyTabDown != null)
                    {
                        keyTabDown.Invoke(this);
                    }
                    break;

                default:
                    base.OnKeyDown(e);
                    break;
            }
        }
    }
}
