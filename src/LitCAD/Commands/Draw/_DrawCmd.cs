using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;
using LitCAD.ApplicationServices;
using LitCAD.UI;

namespace LitCAD.Commands.Draw
{
    internal abstract class DrawCmd : Command
    {
        protected abstract IEnumerable<Entity> newEntities { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _mgr.presenter.selections.Clear();
            this.pointer.isShowAnchor = false;
        }

        /// <summary>
        /// 结束
        /// </summary>
        public override void Terminate()
        {
            _mgr.presenter.selections.Clear();

            base.Terminate();
        }

        /// <summary>
        /// 提交到数据库
        /// </summary>
        protected override void Commit()
        {
            foreach (Entity entity in this.newEntities)
            {
                this.presenter.AppendEntity(entity);
            }
        }

        /// <summary>
        /// 回滚撤销
        /// </summary>
        protected override void Rollback()
        {
            foreach (Entity entity in this.newEntities)
            {
                entity.Erase();
            }
        }
    }
}
