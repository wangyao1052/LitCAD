using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class AnchorsMgr
    {
        private Presenter _presenter = null;
        private Dictionary<ObjectId, List<EntityAnchor>> _anchors = new Dictionary<ObjectId, List<EntityAnchor>>();
        private EntityAnchor _currAnchor = null;
        internal EntityAnchor currentAnchor
        {
            get { return _currAnchor; }
        }

        internal Commands.Command currentAnchorCmd
        {
            get
            {
                if (_currAnchor == null)
                {
                    return null;
                }

                if (_anchorType2CmdType.ContainsKey(_currAnchor.GetType()))
                {
                    object[] parameters = new object[1];
                    parameters[0] = _currAnchor;
                    return Activator.CreateInstance(_anchorType2CmdType[_currAnchor.GetType()], parameters) as Commands.Command;
                }
                else
                {
                    return null;
                }
            }
        }
        
        private Dictionary<Type, EntityAnchorCreator> _entityType2AnchorCreator = new Dictionary<Type,EntityAnchorCreator>();
        private Dictionary<Type, Type> _anchorType2CmdType = new Dictionary<Type, Type>();

        public AnchorsMgr(Presenter presenter)
        {
            _presenter = presenter;
            Initialize();
        }

        private void Initialize()
        {
            RegisterEntityAnchorCreator(typeof(Line), new LineAnchorCreator());
            RegisterEntityAnchorCreator(typeof(Xline), new XlineAnchorCreator());
            RegisterEntityAnchorCreator(typeof(Ray), new RayAnchorCreator());
            RegisterEntityAnchorCreator(typeof(Circle), new CircleAnchorCreator());
            RegisterEntityAnchorCreator(typeof(Polyline), new PolylineAnchorCreator());
            RegisterEntityAnchorCreator(typeof(Arc), new ArcAnchorCreator());
            RegisterEntityAnchorCreator(typeof(Text), new TextAnchorCreator());

            RegisterAnchorCmd(typeof(LineAnchor), typeof(Commands.Anchor.LineAnchorCmd));
            RegisterAnchorCmd(typeof(XlineAnchor), typeof(Commands.Anchor.XlineAnchorCmd));
            RegisterAnchorCmd(typeof(RayAnchor), typeof(Commands.Anchor.RayAnchorCmd));
            RegisterAnchorCmd(typeof(CircleAnchor), typeof(Commands.Anchor.CircleAnchorCmd));
            RegisterAnchorCmd(typeof(PolylineAnchor), typeof(Commands.Anchor.PolylineAnchorCmd));
            RegisterAnchorCmd(typeof(ArcAnchor), typeof(Commands.Anchor.ArcAnchorCmd));
            RegisterAnchorCmd(typeof(TextAnchor), typeof(Commands.Anchor.TextAnchorCmd));
        }

        private void RegisterEntityAnchorCreator(Type entityType, EntityAnchorCreator creator)
        {
            _entityType2AnchorCreator[entityType] = creator;
        }

        private void RegisterAnchorCmd(Type anchorType, Type anchorCmdType)
        {
            _anchorType2CmdType[anchorType] = anchorCmdType;
        }

        internal void Update()
        {
            Document doc = _presenter.document as Document;
            if (doc.selections.Count == 0)
            {
                _anchors.Clear();
                return;
            }

            Dictionary<ObjectId, List<EntityAnchor>> oldAnchors = _anchors;
            _anchors = new Dictionary<ObjectId, List<EntityAnchor>>();
            foreach (Selection sel in doc.selections)
            {
                if (sel.objectId == ObjectId.Null)
                {
                    continue;
                }

                DBObject dbobj = doc.database.GetObject(sel.objectId);
                if (dbobj == null)
                {
                    continue;
                }
                if (!(dbobj is Entity))
                {
                    continue;
                }

                if (oldAnchors.ContainsKey(sel.objectId))
                {
                    _anchors[sel.objectId] = oldAnchors[sel.objectId];
                }
                else
                {
                    if (_entityType2AnchorCreator.ContainsKey(dbobj.GetType()))
                    {
                        _anchors[sel.objectId] = _entityType2AnchorCreator[dbobj.GetType()].NewAnchors(dbobj as Entity);
                    }
                }
            }
        }

        internal void Clear()
        {
            _anchors.Clear();
        }

        internal void OnPaint(Graphics graphics)
        {
            foreach (List<EntityAnchor> anchorLst in _anchors.Values)
            {
                foreach (EntityAnchor anchor in anchorLst)
                {
                    if (anchor == _currAnchor)
                    {
                        anchor.OnDraw(_presenter, graphics, Color.FromArgb(255, 100, 100));
                    }
                    else
                    {
                        anchor.OnDraw(_presenter, graphics, Color.Blue);
                    }
                }
            }
        }

        internal LitMath.Vector2 Snap(LitMath.Vector2 posInCanvas)
        {
            LitMath.Vector2 posInModel = _presenter.CanvasToModel(posInCanvas);

            foreach (List<EntityAnchor> anchorLst in _anchors.Values)
            {
                foreach (EntityAnchor anchor in anchorLst)
                {
                    if (anchor.HitTest(_presenter, posInCanvas))
                    {
                        _currAnchor = anchor;
                        return anchor.position;
                    }
                }
            }

            _currAnchor = null;
            return posInModel;
        }

        //public Commands.Command OnMouseDown(MouseEventArgs e)
        //{
        //    if (_anchors.Count == 0)
        //    {
        //        return null;
        //    }

        //    LitMath.Vector2 point = new LitMath.Vector2(e.X, e.Y);
        //    foreach (List<EntityAnchor> anchorLst in _anchors.Values)
        //    {
        //        foreach (EntityAnchor anchor in anchorLst)
        //        {
        //            if (anchor.HitTest(_presenter, point))
        //            {
        //                if (_anchorType2CmdType.ContainsKey(anchor.GetType()))
        //                {
        //                    object[] parameters = new object[1];
        //                    parameters[0] = anchor;
        //                    return Activator.CreateInstance(_anchorType2CmdType[anchor.GetType()], parameters) as Commands.Command;
        //                }
        //            }
        //        }
        //    }

        //    return null;
        //}
    }
}
