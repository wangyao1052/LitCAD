using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;
using LitCAD.Utils;

namespace LitCAD.Windows
{
    public partial class LayersManagementForm : Form
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        private LayersManagementForm()
        {
            InitializeComponent();
            InitializeLayerListView();
            this.Owner = MainWin.Instance;
            DocumentForm activeDocForm = MainWin.Instance.ActiveMdiChild as DocumentForm;
            if (activeDocForm != null)
            {
                _docForm = activeDocForm;
            }
            else
            {
                _docForm = null;
            }
            MainWin.Instance.MdiChildActivate += this.OnDocumentFormActivated;
        }

        /// <summary>
        /// 单例
        /// </summary>
        private static LayersManagementForm _instance = null;
        private static int index = 0;
        internal static LayersManagementForm Instance
        {
            get
            {
                if (_instance == null)
                {
                    index++;
                    _instance = new LayersManagementForm();
                }
                
                return _instance;
            }
        }

        /// <summary>
        /// 文档
        /// </summary>
        private DocumentForm _docForm = null;

        /// <summary>
        /// 文档窗口激活相应函数
        /// </summary>
        private void OnDocumentFormActivated(object sender, EventArgs e)
        {
            DocumentForm docForm = MainWin.Instance.ActiveMdiChild as DocumentForm;
            if (docForm != null)
            {
                _docForm = docForm; ;
                if (this.Visible)
                {
                    this.ReUpdateListView();
                }
            }
            else
            {
                _docForm = null;
                this.Hide();
            }
        }

        private void InitializeLayerListView()
        {
            //
            this.layerListView.View = View.Details;
            this.layerListView.FullRowSelect = true;

            //
            ColumnHeader headerName = new ColumnHeader();
            headerName.Text = "名称";
            headerName.TextAlign = HorizontalAlignment.Left;
            headerName.Width = 100;
            this.layerListView.Columns.Add(headerName);

            ColumnHeader headerColor = new ColumnHeader();
            headerColor.Text = "颜色";
            headerColor.TextAlign = HorizontalAlignment.Left;
            headerColor.Width = 100;
            this.layerListView.Columns.Add(headerColor);
        }

        /// <summary>
        /// 刷新重绘图层列表
        /// </summary>
        private void ReUpdateListView()
        {
            this.layerListView.BeginUpdate();

            this.layerListView.Items.Clear();
            if (_docForm != null)
            {
                foreach (Layer layer in _docForm.document.database.layerTable)
                {
                    this.AddLayerToListView(layer);
                }
            }
            
            this.layerListView.EndUpdate();
        }

        private void AddLayerToListView(Layer layer)
        {
            ListViewItem layerItem = new ListViewItem();
            layerItem.Text = layer.name;
            layerItem.SubItems.Add(_docForm.document.commonColors.GetColorName(layer.color));
            layerItem.Tag = new LayerItemData(layer);

            this.layerListView.Items.Add(layerItem);
        }

        private void UpdateLayerListViewItem(ListViewItem item)
        {
            LayerItemData layerData = item.Tag as LayerItemData;
            item.Text = layerData.layerCopy.name;
            item.SubItems[1].Text = _docForm.document.commonColors.GetColorName(layerData.layerCopy.color);
        }

        /// <summary>
        /// Add layer
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (_docForm != null)
            {
                Commands.AddLayerCmd cmd = new Commands.AddLayerCmd();
                _docForm.presenter.OnCommand(cmd);
                this.ReUpdateListView();
            }
        }

        /// <summary>
        /// delete layer
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.layerListView.SelectedItems.Count <= 0)
            {
                return;
            }

            List<Layer> layers = new List<Layer>();
            for (int i = 0; i < this.layerListView.SelectedItems.Count; ++i)
            {
                ListViewItem selItem = this.layerListView.SelectedItems[i];
                LayerItemData layerData = selItem.Tag as LayerItemData;
                Layer layer = layerData.layer;
                layers.Add(layer);
            }

            Commands.RemoveLayersCmd cmd = new Commands.RemoveLayersCmd(layers);
            _docForm.presenter.OnCommand(cmd);
            this.ReUpdateListView();
        }

        /// <summary>
        /// modify layer
        /// </summary>
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (this.layerListView.SelectedItems.Count != 1)
            {
                return;
            }

            ListViewItem selItem = this.layerListView.SelectedItems[0];
            LayerItemData layerData = selItem.Tag as LayerItemData;
            Layer layer = layerData.layer;

            Commands.ModifyLayerCmd cmd = new Commands.ModifyLayerCmd(layer);
            _docForm.presenter.OnCommand(cmd);
            this.ReUpdateListView();
        }

        private class LayerItemData
        {
            public Layer layer = null;
            public Layer layerCopy = null;
            public bool isExistInDB = false;

            public LayerItemData(Layer layerArg)
            {
                this.layer = layerArg;
                this.layerCopy = this.layer.Clone() as Layer;
                if (this.layer.database != null)
                {
                    this.isExistInDB = true;
                }
                else
                {
                    this.isExistInDB = false;
                }
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible)
            {
                this.ReUpdateListView();
            }
            else
            {
                this.layerListView.Items.Clear();
            }
        }

        /// <summary>
        /// 第一次显示窗体事件
        /// </summary>
        private void LayersManagementForm_Shown(object sender, EventArgs e)
        {
            if (index == 1)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(
                    MainWin.Instance.Location.X + MainWin.Instance.Width / 2 - this.Width / 2,
                    MainWin.Instance.Location.Y + MainWin.Instance.Height / 2 - this.Height / 2);
            }
            else
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = _location;
            }
        }

        private static Point _location;
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _location = this.Location;
            MainWin.Instance.MdiChildActivate -= this.OnDocumentFormActivated;
            base.OnFormClosing(e);
            _instance = null;
        }
    }
}
