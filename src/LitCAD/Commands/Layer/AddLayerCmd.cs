using System;
using System.Collections.Generic;
using System.Windows.Forms;

using LitCAD.DatabaseServices;
using LitCAD.Windows;

namespace LitCAD.Commands
{
    /// <summary>
    /// 新增图层命令
    /// </summary>
    internal class AddLayerCmd : Command
    {
        private Layer _layer = null;

        public override void Initialize()
        {
            LayerItemForm dlg = new LayerItemForm(LayerItemForm.Mode.Add, null, this.database);
            DialogResult dlgRet = dlg.ShowDialog();
            if (dlgRet == DialogResult.OK)
            {
                _layer = dlg.layer;
                _mgr.FinishCurrentCommand();
            }
            else
            {
                _mgr.CancelCurrentCommand();
            }
        }

        public override void Undo()
        {
            base.Undo();

            if (_layer != null)
            {
                this.database.layerTable.Remove(_layer);
            }
        }

        public override void Redo()
        {
            base.Redo();

            this.CommitToDatabase();
        }

        private void CommitToDatabase()
        {
            if (_layer != null)
            {
                this.database.layerTable.Add(_layer);
            }
        }

        public override void Finish()
        {
            this.CommitToDatabase();

            base.Finish();
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
