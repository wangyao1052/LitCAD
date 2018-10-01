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
using lcColor = LitCAD.Colors.Color;

namespace LitCAD.Windows
{
    internal partial class LayerItemForm : Form
    {
        private readonly Layer _layer = null;
        private lcColor _lastColor;
        private readonly Database _database;

        /// <summary>
        /// 新增还是修改
        /// </summary>
        public enum Mode
        {
            Add = 0,
            Modify = 1,
        }
        private Mode _mode = Mode.Add;

        //
        public LayerItemForm(Mode mode, Layer layer, Database database)
        {
            _mode = mode;
            _database = database;
            if (_mode == Mode.Modify)
            {
                _layer = layer;
            }
            else
            {
                _layer = new Layer();
                _layer.name = "";
                _layer.color = lcColor.FromRGB(255, 255, 255);
            }

            InitializeComponent();
            InitializePredefinedColors();

            if (_layer != null)
            {
                this.textboxName.Text = _layer.name;
            }

            InitializeColorCombo();
        }

        /// <summary>
        /// 预制颜色
        /// </summary>
        private Dictionary<lcColor, string> _predefinedColors = new Dictionary<lcColor, string>();
        private void InitializePredefinedColors()
        {
            _predefinedColors.Add(lcColor.FromRGB(255, 0, 0), "红");
            _predefinedColors.Add(lcColor.FromRGB(255, 255, 0), "黄");
            _predefinedColors.Add(lcColor.FromRGB(0, 255, 0), "绿");
            _predefinedColors.Add(lcColor.FromRGB(0, 255, 255), "青");
            _predefinedColors.Add(lcColor.FromRGB(0, 0, 255), "蓝");
            _predefinedColors.Add(lcColor.FromRGB(255, 0, 255), "洋红");
            _predefinedColors.Add(lcColor.FromRGB(255, 255, 255), "白");

            if (_layer != null)
            {
                if (!_predefinedColors.ContainsKey(_layer.color))
                {
                    _predefinedColors.Add(_layer.color, _layer.color.Name);
                }
            }
        }

        /// <summary>
        /// Color combobox
        /// </summary>
        private int _indexToInsertCustomColor = -1;
        private void InitializeColorCombo()
        {
            // predefined colors
            foreach (KeyValuePair<lcColor, string> kvp in _predefinedColors)
            {
                ColorItem colorItem = new ColorItem();
                colorItem.color = kvp.Key;
                colorItem.text = kvp.Value;

                this.comboColor.Items.Add(colorItem);
            }
            _indexToInsertCustomColor = this.comboColor.Items.Count;

            // select custom color
            ColorItem selectColorItem = new ColorItem();
            selectColorItem.text = null;
            this.comboColor.Items.Add(selectColorItem);

            //
            _lastColor = _layer.color;
            this.SetColorComboValue(_lastColor);
            this.comboColor.SelectedIndexChanged += this.OnColorComboSelectedIndexChanged;
        }

        private void SetColorComboValue(lcColor color)
        {
            int index = -1;
            for (int i = 0; i < this.comboColor.Items.Count; ++i)
            {
                ColorItem colorItem = this.comboColor.Items[i] as ColorItem;
                if (colorItem.text != null)
                {
                    if (colorItem.color == color)
                    {
                        index = i;
                        break;
                    }
                }
            }

            if (index == -1)
            {
                ColorItem colorItem = new ColorItem();
                colorItem.color = color;
                this.comboColor.Items.Insert(_indexToInsertCustomColor, colorItem);
                index = _indexToInsertCustomColor;
            }

            this.comboColor.SelectedIndex = index;
        }

        private void OnColorComboSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboColor.SelectedItem == null)
            {
                return;
            }

            ColorItem selColorItem = this.comboColor.SelectedItem as ColorItem;
            if (selColorItem.text == null)
            {
                ColorDialog colorDlg = new ColorDialog();
                colorDlg.AllowFullOpen = true;
                colorDlg.SolidColorOnly = true;
                DialogResult dlgRet = colorDlg.ShowDialog();
                if (dlgRet == DialogResult.OK)
                {
                    lcColor color = lcColor.FromColor(colorDlg.Color);
                    _lastColor = color;
                    this.SetColorComboValue(color);
                }
                else
                {
                    this.SetColorComboValue(_lastColor);
                }
            }
            else
            {
                _lastColor = selColorItem.color;
            }
        }

        private class ColorItem
        {
            public lcColor color;
            public string text = "";

            public override string ToString()
            {
                if (text == null)
                {
                    return "选择颜色";
                }
                else if (text == "")
                {
                    return color.Name;
                }
                else
                {
                    return text;
                }
            }
        }

        private string layerName
        {
            get
            {
                return this.textboxName.Text.Trim();
            }
        }

        private lcColor layerColor
        {
            get
            {
                if (this.comboColor.SelectedItem == null)
                {
                    return lcColor.FromRGB(255, 255, 255);
                }

                ColorItem colorItem = this.comboColor.SelectedItem as ColorItem;
                return colorItem.color;
            }
        }

        /// <summary>
        /// 图层
        /// </summary>
        internal Layer layer
        {
            get
            {
                Layer layer = _layer.Clone() as Layer;
                layer.name = this.layerName;
                layer.color = this.layerColor;
                return layer;
            }
        }

        /// <summary>
        /// OK button
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.layerName == "")
            {
                MessageBox.Show("图层名不能为空");
                return;
            }

            foreach (Layer layer in _database.layerTable)
            {
                if (layer != _layer)
                {
                    if (layer.name == this.layerName)
                    {
                        string msg = string.Format("图层名: {0} 已经存在", this.layerName);
                        MessageBox.Show(msg);
                        return;
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Cancel button
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
