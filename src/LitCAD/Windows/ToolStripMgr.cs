using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace LitCAD.Windows
{
    public class ToolStripMgr
    {
        // 菜单项
        private Dictionary<string, ToolStripMenuItem> _menuItems = new Dictionary<string, ToolStripMenuItem>();
        private Dictionary<string, EventHandler> _menuItemsClick = new Dictionary<string, EventHandler>();
        // 工具条项
        private List<ToolStripItem> _toolbarItems = new List<ToolStripItem>();

        // 工具条
        private Dictionary<string, ToolStrip> _toolStrips = new Dictionary<string, ToolStrip>();
        // 菜单
        private Dictionary<string, MenuStrip> _menuStrips = new Dictionary<string, MenuStrip>();
        // 状态栏
        private Dictionary<string, StatusStrip> _statusStrips = new Dictionary<string, StatusStrip>();
        // ToolStripPanel
        private Dictionary<DockStyle, ToolStripPanel> _toolStripPanels = new Dictionary<DockStyle, ToolStripPanel>();

        public ToolStripMgr()
        {
        }

        public ToolStrip GetToolStrip(string name, bool createWhenNotExist = true)
        {
            if (_toolStrips.ContainsKey(name))
            {
                return _toolStrips[name];
            }

            if (createWhenNotExist)
            {
                ToolStrip toolStrip = new ToolStrip();
                _toolStrips[name] = toolStrip;

                return toolStrip;
            }
            else
            {
                return null;
            }
        }

        public MenuStrip GetMenuStrip(string name, bool createWhenNotExist = true)
        {
            if (_menuStrips.ContainsKey(name))
            {
                return _menuStrips[name];
            }

            if (createWhenNotExist)
            {
                MenuStrip menuStrip = new MenuStrip();
                _menuStrips[name] = menuStrip;

                return menuStrip;
            }
            else
            {
                return null;
            }
        }

        public StatusStrip GetStatusStrip(string name, bool createWhenNotExist = true)
        {
            if (_statusStrips.ContainsKey(name))
            {
                return _statusStrips[name];
            }

            if (createWhenNotExist)
            {
                StatusStrip statusStrip = new StatusStrip();
                _statusStrips[name] = statusStrip;

                return statusStrip;

            }
            else
            {
                return null;
            }
        }

        public ToolStripPanel GetToolStripPanle(DockStyle dockStyle, bool createWhenNotExist = true)
        {
            if (_toolStripPanels.ContainsKey(dockStyle))
            {
                return _toolStripPanels[dockStyle];
            }

            if (createWhenNotExist)
            {
                ToolStripPanel panel = new ToolStripPanel();
                panel.Dock = dockStyle;
                _toolStripPanels[dockStyle] = panel;

                return panel;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 新建菜单项
        /// </summary>
        public ToolStripMenuItem NewMenuItem(
            string id,
            string text,
            Image image,
            EventHandler eventHandler)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = text;
            menuItem.Image = image;
            menuItem.Click += eventHandler;

            _menuItems[id] = menuItem;
            _menuItemsClick[id] = eventHandler;
            return menuItem;
        }

        public ToolStripMenuItem GetMenuItem(string menuItemId)
        {
            if (_menuItems.ContainsKey(menuItemId))
            {
                return _menuItems[menuItemId];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 新建工具条项
        /// </summary>
        public ToolStripButton NewToolStripButton(
            string id)
        {
            if (_menuItems.ContainsKey(id)
                && _menuItemsClick.ContainsKey(id))
            {
                ToolStripMenuItem menuItem = _menuItems[id];

                return NewToolStripButton(menuItem.Text, menuItem.Image, _menuItemsClick[id]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 新建工具条项
        /// </summary>
        public ToolStripButton NewToolStripButton(
            string text,
            Image image,
            EventHandler eventHandler)
        {
            ToolStripButton tsbtn = new ToolStripButton();
            tsbtn.Text = text;
            tsbtn.Image = image;
            tsbtn.Click += eventHandler;

            _toolbarItems.Add(tsbtn);
            return tsbtn;
        }

        /// <summary>
        /// 添加工具条项
        /// </summary>
        public void AddToolStripItem(ToolStripItem tsitem)
        {
            _toolbarItems.Add(tsitem);
        }

        /// <summary>
        /// Disable所有
        /// </summary>
        public void DisableAll()
        {
            foreach (KeyValuePair<string, ToolStripMenuItem> kvp in _menuItems)
            {
                kvp.Value.Enabled = false;
            }

            foreach (ToolStripItem item in _toolbarItems)
            {
                item.Enabled = false;
            }
        }
    }
}
