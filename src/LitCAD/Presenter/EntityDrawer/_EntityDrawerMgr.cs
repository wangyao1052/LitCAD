using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class EntityDrawerMgr
    {
        private Presenter _presenter;
        private Dictionary<Type, EntityDrawer> _entityType2Drawer;

        internal EntityDrawerMgr(Presenter presenter)
        {
            _presenter = presenter;
            _entityType2Drawer = new Dictionary<Type, EntityDrawer>();

            Initialize();
        }

        private void Initialize()
        {
            RegisterEntityDrawer(typeof(Line), new LineDrawer(_presenter));
            RegisterEntityDrawer(typeof(Xline), new XlineDrawer(_presenter));
            RegisterEntityDrawer(typeof(Ray), new RayDrawer(_presenter));
            RegisterEntityDrawer(typeof(Circle), new CircleDrawer(_presenter));
            RegisterEntityDrawer(typeof(Polyline), new PolylineDrawer(_presenter));
            RegisterEntityDrawer(typeof(Arc), new ArcDrawer(_presenter));
            RegisterEntityDrawer(typeof(Text), new TextDrawer(_presenter));
        }

        private void RegisterEntityDrawer(Type entityType, EntityDrawer entityDrawer)
        {
            _entityType2Drawer[entityType] = entityDrawer;
        }

        internal void DrawEntity(Graphics graphics, Entity entity, Pen pen)
        {
            Type type = entity.GetType();
            if (_entityType2Drawer.ContainsKey(type))
            {
                _entityType2Drawer[type].Draw(graphics, entity, pen);
            }
        }
    }
}
