using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal abstract class EntityDrawer
    {
        protected Presenter _presenter;

        protected EntityDrawer(Presenter presenter)
        {
            _presenter = presenter;
        }

        internal abstract void Draw(Graphics graphics, Entity entity, Pen pen);

        protected Pen GetPen(Entity entity)
        {
            if (entity.database != null)
            {
                return GDIResMgr.Instance.GetPen(entity.colorValue, 1);
            }
            else
            {
                if (entity.color.colorMethod == Colors.ColorMethod.ByLayer)
                {
                    Database db = _presenter.document.database;
                    Layer layer = db.GetObject(entity.layerId) as Layer;
                    if (layer != null)
                    {
                        return GDIResMgr.Instance.GetPen(layer.colorValue, 1);
                    }
                    else
                    {
                        return GDIResMgr.Instance.GetPen(entity.colorValue, 1);
                    }
                }
                else
                {
                    return GDIResMgr.Instance.GetPen(entity.colorValue, 1);
                }
                
            }
        }
    }
}
