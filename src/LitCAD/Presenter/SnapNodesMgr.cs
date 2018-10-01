using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    /// <summary>
    /// 捕捉节点管理器
    /// </summary>
    internal class SnapNodesMgr
    {
        private Presenter _presenter = null;
        private Dictionary<Type, SnapNodeHost> _type2SnapNodeHost = null;
        private SnapNode _currSnapNode = null;
        internal SnapNode currentSnapNode
        {
            get { return _currSnapNode; }
        }

        public SnapNodesMgr(Presenter presenter)
        {
            _presenter = presenter;
            _type2SnapNodeHost = new Dictionary<Type, SnapNodeHost>();

            Initialize();
        }

        private void Initialize()
        {
            RegisterSnapNodeHost(typeof(Line), new SnapNodeHostLine());
            RegisterSnapNodeHost(typeof(Polyline), new SnapNodeHostPolyline());
            RegisterSnapNodeHost(typeof(Circle), new SnapNodeHostCircle());
            RegisterSnapNodeHost(typeof(Arc), new SnapNodeHostArc());
            RegisterSnapNodeHost(typeof(Text), new SnapNodeHostText());
        }

        private void RegisterSnapNodeHost(Type type, SnapNodeHost snapNodeHost)
        {
            _type2SnapNodeHost[type] = snapNodeHost;
        }

        public LitMath.Vector2 Snap(double x, double y)
        {
            return this.Snap(new LitMath.Vector2(x, y));
        }

        internal LitMath.Vector2 Snap(LitMath.Vector2 posInCanvas)
        {
            LitMath.Vector2 posInModel = _presenter.CanvasToModel(posInCanvas);

            foreach (Entity entity in _presenter.currentBlock)
            {
                Type type = entity.GetType();
                if (_type2SnapNodeHost.ContainsKey(type))
                {
                    SnapNodeHost snapNodeHost = _type2SnapNodeHost[type];
                    snapNodeHost.UpdateSnapNodes(entity);
                    foreach (SnapNode snapNode in snapNodeHost)
                    {
                        double dis = (snapNode.position - posInModel).length;
                        double disInCanvas = _presenter.ModelToCanvas(dis);
                        if (disInCanvas <= snapNode.threshold)
                        {
                            _currSnapNode = snapNode;
                            return _currSnapNode.position;
                        }
                    }
                }
            }

            _currSnapNode = null;
            return posInModel;
        }

        public void Clear()
        {
            _currSnapNode = null;
        }

        public void OnPaint(Graphics g)
        {
            if (_currSnapNode != null)
            {
                _currSnapNode.OnDraw(_presenter, g);
            }
        }
    }
}
