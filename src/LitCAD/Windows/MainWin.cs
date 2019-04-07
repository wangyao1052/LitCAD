using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LitCAD.Windows
{
    public partial class MainWin : Form
    {
        private MainWin()
        {
            InitializeComponent();
            SetupToolStripUI();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2)
            {
                OpenFile(args[1]);
            }
            else
            {
                NewFile();
            }

            Application.Idle += this.OnIdle;
        }

        /// <summary>
        /// 单例
        /// </summary>
        private static MainWin _instance = null;
        public static MainWin Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainWin();
                }
                return _instance;
            }
        }

        private ToolStripMgr _toolStripMgr = new ToolStripMgr();

        /// <summary>
        /// 设置菜单、工具条、状态条
        /// </summary>
        private void SetupToolStripUI()
        {
            // 主菜单
            MenuStrip menuMain = SetupMainMenu();
            this.MainMenuStrip = menuMain;

            // 工具条
            List<ToolStrip> toolStripList = SetupToolbar();

            // Top Panel
            ToolStripPanel topPanel = _toolStripMgr.GetToolStripPanle(DockStyle.Top, true);
            this.Controls.Add(topPanel);

            // 将主菜单和工具条添加到 Top Panel
            for (int i = toolStripList.Count - 1; i >= 0; --i)
            {
                topPanel.Join(toolStripList[i]);
            }
            topPanel.Join(menuMain);
        }

        /// <summary>
        /// 设置主菜单
        /// </summary>
        private MenuStrip SetupMainMenu()
        {
            MenuStrip menuMain = _toolStripMgr.GetMenuStrip("Main", true);

            SetupMainMenu_File(menuMain);

            return menuMain;
        }

        /// <summary>
        /// 文件菜单
        /// </summary>
        private void SetupMainMenu_File(MenuStrip menuMain)
        {
            ToolStripMenuItem menuFile = new ToolStripMenuItem();
            menuFile.Text = "文件";
            menuMain.Items.Add(menuFile);

            // 新建
            ToolStripMenuItem _new = _toolStripMgr.NewMenuItem(
                "file_new",
                "新建...",
                Resource1.file_new.ToBitmap(),
                this.OnFileNew);
            menuFile.DropDownItems.Add(_new);

            // 打开
            ToolStripMenuItem open = _toolStripMgr.NewMenuItem(
                "file_open",
                "打开...",
                Resource1.file_open.ToBitmap(),
                this.OnFileOpen);
            menuFile.DropDownItems.Add(open);

            // 保存
            ToolStripMenuItem save = _toolStripMgr.NewMenuItem(
                "file_save",
                "保存",
                Resource1.file_save.ToBitmap(),
                this.OnFileSave);
            menuFile.DropDownItems.Add(save);

            // 另存为
            ToolStripMenuItem saveas = _toolStripMgr.NewMenuItem(
                "file_saveas",
                "另存为...",
                Resource1.file_saveas.ToBitmap(),
                this.OnFileSaveAs);
            menuFile.DropDownItems.Add(saveas);
        }

        /// <summary>
        /// 设置工具条
        /// </summary>
        private List<ToolStrip> SetupToolbar()
        {
            List<ToolStrip> toolStripList = new List<ToolStrip>();

            // 文件
            ToolStrip fileToolstrip = SetupToolbar_File();
            toolStripList.Add(fileToolstrip);

            // 编辑
            ToolStrip editToolstrip = SetupToolbar_Edit();
            toolStripList.Add(editToolstrip);

            // 绘制
            ToolStrip drawToolstrip = SetupToolbar_Draw();
            toolStripList.Add(drawToolstrip);

            // 修改
            ToolStrip modifyToolstrip = SetupToolbar_Modify();
            toolStripList.Add(modifyToolstrip);

            // 图层
            ToolStrip layerToolstrip = SetupToolbar_Layer();
            toolStripList.Add(layerToolstrip);

            // 特性
            ToolStrip propertyToolstrip = SetupToolbar_Property();
            toolStripList.Add(propertyToolstrip);

            return toolStripList;
        }

        /// <summary>
        /// 工具条: 文件
        /// </summary>
        private ToolStrip SetupToolbar_File()
        {
            ToolStrip fileToolstrip = _toolStripMgr.GetToolStrip("File", true);

            // 新建
            ToolStripButton _new = _toolStripMgr.NewToolStripButton("file_new");
            _new.Text = "";
            fileToolstrip.Items.Add(_new);

            // 打开
            ToolStripButton open = _toolStripMgr.NewToolStripButton("file_open");
            open.Text = "";
            fileToolstrip.Items.Add(open);

            // 保存
            ToolStripButton save = _toolStripMgr.NewToolStripButton("file_save");
            save.Text = "";
            fileToolstrip.Items.Add(save);

            // 另存为
            ToolStripButton saveas = _toolStripMgr.NewToolStripButton("file_saveas");
            saveas.Text = "";
            fileToolstrip.Items.Add(saveas);

            return fileToolstrip;
        }

        /// <summary>
        /// 工具条: 编辑
        /// </summary>
        private ToolStrip SetupToolbar_Edit()
        {
            ToolStrip editToolstrip = _toolStripMgr.GetToolStrip("Edit", true);

            return editToolstrip;
        }

        /// <summary>
        /// 工具条: 绘制
        /// </summary>
        private ToolStrip SetupToolbar_Draw()
        {
            ToolStrip drawToolstrip = _toolStripMgr.GetToolStrip("Draw", true);

            return drawToolstrip;
        }

        /// <summary>
        /// 工具条: 修改
        /// </summary>
        private ToolStrip SetupToolbar_Modify()
        {
            ToolStrip modifyToolstrip = _toolStripMgr.GetToolStrip("Modify", true);

            return modifyToolstrip;
        }

        /// <summary>
        /// 工具条: 图层
        /// </summary>
        private ToolStrip SetupToolbar_Layer()
        {
            ToolStrip layerToolstrip = _toolStripMgr.GetToolStrip("Layer", true);

            return layerToolstrip;
        }

        /// <summary>
        /// 工具条: 特性
        /// </summary>
        private ToolStrip SetupToolbar_Property()
        {
            ToolStrip propertyToolstrip = _toolStripMgr.GetToolStrip("Property", true);

            return propertyToolstrip;
        }

        /// <summary>
        /// 新建菜单项
        /// </summary>
        private ToolStripMenuItem NewMenuItem(
            string text,
            Image image,
            EventHandler eventHandler)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = text;
            menuItem.Image = image;
            menuItem.Click += eventHandler;
            return menuItem;
        }

        /// <summary>
        /// 新建文件
        /// </summary>
        private void OnFileNew(object sender, EventArgs e)
        {
            NewFile();
        }

        private void NewFile()
        {
            DocumentForm docForm = new DocumentForm();
            docForm.Text = GetNextNewFileName();
            docForm.MdiParent = this;
            docForm.WindowState = FormWindowState.Maximized;
            docForm.Show();
        }

        private string GetNextNewFileName()
        {
            string strBase = "new";
            uint id = 1;

            foreach (Form form in this.MdiChildren)
            {
                DocumentForm docForm = form as DocumentForm;
                if (docForm == null)
                {
                    continue;
                }

                string fileName = "";
                LitCAD.DatabaseServices.Database db = docForm.document.database;
                if (db.fileName != null)
                {
                    fileName = System.IO.Path.GetFileNameWithoutExtension(db.fileName);
                }
                else
                {
                    fileName = docForm.Text;
                }
                fileName = fileName.ToLower();

                if (fileName.IndexOf(strBase) == 0)
                {
                    fileName = fileName.Substring(strBase.Length);
                    uint number = 0;
                    if (uint.TryParse(fileName, out number))
                    {
                        if (number >= id)
                        {
                            id = number + 1;
                        }
                    }
                }
            }

            return string.Format("{0}{1}", strBase, id);
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        private void OnFileOpen(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "LitCAD文件(*.litcad)|*.litcad";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string strFileFullPath = ofd.FileName;
                OpenFile(strFileFullPath);
            }
        }

        private void OpenFile(string fileFullPath)
        {
            // 检查是否已经打开
            string fileFullPathLower = fileFullPath.ToLower();
            foreach (Form form in this.MdiChildren)
            {
                DocumentForm childDocForm = form as DocumentForm;
                if (childDocForm == null)
                {
                    continue;
                }

                string strDocPath = childDocForm.fileFullPath.ToLower();
                if (fileFullPathLower == strDocPath)
                {
                    childDocForm.Activate();
                    return;
                }
            }

            // 打开文件
            DocumentForm docForm = new DocumentForm();
            if (fileFullPath != null
                && System.IO.File.Exists(fileFullPath))
            {
                docForm.Open(fileFullPath);
            }
            docForm.MdiParent = this;
            docForm.WindowState = FormWindowState.Maximized;
            docForm.Show();
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        private void OnFileSave(object sender, EventArgs e)
        {
            DocumentForm activeDocForm = this.ActiveMdiChild as DocumentForm;
            if (activeDocForm == null)
            {
                return;
            }

            LitCAD.DatabaseServices.Database db = activeDocForm.document.database;
            if (db.fileName == null)
            {
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "保存";
                savedialog.Filter = "LitCAD文件(*.litcad)|*.litcad";
                savedialog.FilterIndex = 0;
                savedialog.RestoreDirectory = true;
                savedialog.CheckPathExists = true;
                savedialog.FileName = activeDocForm.Text;
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    string fileFullPath = savedialog.FileName;
                    activeDocForm.SaveAs(fileFullPath, true);
                }
            }
            else
            {
                activeDocForm.Save();
            }
        }

        /// <summary>
        /// 文件另存为
        /// </summary>
        private void OnFileSaveAs(object sender, EventArgs e)
        {
            DocumentForm activeDocForm = this.ActiveMdiChild as DocumentForm;
            if (activeDocForm == null)
            {
                return;
            }

            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "另存为";
            savedialog.Filter = "LitCAD文件(*.litcad)|*.litcad";
            savedialog.FilterIndex = 0;
            savedialog.RestoreDirectory = true;
            savedialog.CheckPathExists = true;
            savedialog.FileName = "";
            if (savedialog.ShowDialog() == DialogResult.OK)
            {
                string fileFullPath = savedialog.FileName;
                activeDocForm.SaveAs(fileFullPath);
            }
        }

        /// <summary>
        /// 子窗体激活事件
        /// </summary>
        protected override void OnMdiChildActivate(EventArgs e)
        {
            base.OnMdiChildActivate(e);

            DocumentForm activeDocForm = this.ActiveMdiChild as DocumentForm;
            foreach (Control ctrl in Controls)
            {
                if (ctrl is ToolStripPanel)
                    ((ToolStripPanel)ctrl).SuspendLayout();
            }
            if (activeDocForm != null)
            {
                ToolStripManager.RevertMerge(_toolStripMgr.GetToolStrip("Edit"));
                ToolStripManager.Merge(activeDocForm.toolstripMgr.GetToolStrip("Edit"), _toolStripMgr.GetToolStrip("Edit"));

                ToolStripManager.RevertMerge(_toolStripMgr.GetToolStrip("Draw"));
                ToolStripManager.Merge(activeDocForm.toolstripMgr.GetToolStrip("Draw"), _toolStripMgr.GetToolStrip("Draw"));

                ToolStripManager.RevertMerge(_toolStripMgr.GetToolStrip("Modify"));
                ToolStripManager.Merge(activeDocForm.toolstripMgr.GetToolStrip("Modify"), _toolStripMgr.GetToolStrip("Modify"));

                ToolStripManager.RevertMerge(_toolStripMgr.GetToolStrip("Layer"));
                ToolStripManager.Merge(activeDocForm.toolstripMgr.GetToolStrip("Layer"), _toolStripMgr.GetToolStrip("Layer"));

                ToolStripManager.RevertMerge(_toolStripMgr.GetToolStrip("Property"));
                ToolStripManager.Merge(activeDocForm.toolstripMgr.GetToolStrip("Property"), _toolStripMgr.GetToolStrip("Property"));
            }
            foreach (Control ctrl in Controls)
            {
                if (ctrl is ToolStripPanel)
                    ((ToolStripPanel)ctrl).ResumeLayout();
            }
        }

        /// <summary>
        /// 空闲事件
        /// </summary>
        private void OnIdle(object sender, EventArgs e)
        {
            DocumentForm currActiveDocForm = this.ActiveMdiChild as DocumentForm;
            if (currActiveDocForm != null)
            {
                currActiveDocForm.UpdateUI();
            }
        }
    }
}
