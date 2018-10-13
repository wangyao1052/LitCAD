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

        private Dictionary<ObjectId, List<GripPoint>> _gripPnts = new Dictionary<ObjectId, List<GripPoint>>();
        private GripPoint _currGripPoint = null;
        internal GripPoint currentGripPoint
        {
            get { return _currGripPoint; }
        }
        private ObjectId _currGripEntityId = ObjectId.Null;
        internal ObjectId currentGripEntityId
        {
            get { return _currGripEntityId; }
        }
        private int _currGripPointIndex = -1;
        internal int currentGripPointIndex
        {
            get { return _currGripPointIndex; }
        }

        public AnchorsMgr(Presenter presenter)
        {
            _presenter = presenter;
        }

        internal void Update()
        {
            Document doc = _presenter.document as Document;
            if (doc.selections.Count == 0)
            {
                _gripPnts.Clear();
                return;
            }

            Dictionary<ObjectId, List<GripPoint>> oldGripPnts = _gripPnts;
            _gripPnts = new Dictionary<ObjectId, List<GripPoint>>();
            foreach (Selection sel in doc.selections)
            {
                if (sel.objectId == ObjectId.Null)
                {
                    continue;
                }
                if (oldGripPnts.ContainsKey(sel.objectId))
                {
                    _gripPnts[sel.objectId] = oldGripPnts[sel.objectId];
                    continue;
                }

                DBObject dbobj = doc.database.GetObject(sel.objectId);
                if (dbobj == null)
                {
                    continue;
                }
                Entity entity = dbobj as Entity;
                if (entity == null)
                {
                    continue;
                }

                List<GripPoint> entGripPnts = entity.GetGripPoints();
                if (entGripPnts != null && entGripPnts.Count > 0)
                {
                    _gripPnts[sel.objectId] = entGripPnts;
                }
            }
        }

        internal void Clear()
        {
            _gripPnts.Clear();
        }

        internal void OnPaint(Graphics graphics)
        {
            foreach (KeyValuePair<ObjectId, List<GripPoint>> kvp in _gripPnts)
            {
                foreach (GripPoint gripPnt in kvp.Value)
                {
                    double width = 10;
                    double height = 10;
                    LitMath.Vector2 posInCanvas = _presenter.ModelToCanvas(gripPnt.position);
                    posInCanvas.x -= width / 2;
                    posInCanvas.y -= height / 2;
                    _presenter.FillRectangle(graphics, GDIResMgr.Instance.GetBrush(Color.Blue), posInCanvas, width, height, CSYS.Canvas);
                }
            }
        }

        internal LitMath.Vector2 Snap(LitMath.Vector2 posInCanvas)
        {
            LitMath.Vector2 posInModel = _presenter.CanvasToModel(posInCanvas);

            foreach (KeyValuePair<ObjectId, List<GripPoint>> kvp in _gripPnts)
            {
                int index = -1;
                foreach (GripPoint gripPnt in kvp.Value)
                {
                    ++index;
                    double width = 10;
                    double height = 10;
                    LitMath.Vector2 gripPosInCanvas = _presenter.ModelToCanvas(gripPnt.position);
                    gripPosInCanvas.x -= width / 2;
                    gripPosInCanvas.y -= height / 2;
                    LitMath.Rectangle2 rect = new LitMath.Rectangle2(gripPosInCanvas, width, height);

                    if (MathUtils.IsPointInRectangle(posInCanvas, rect))
                    {
                        _currGripPoint = gripPnt;
                        _currGripEntityId = kvp.Key;
                        _currGripPointIndex = index;
                        return gripPnt.position;
                    }
                }
            }

            _currGripPoint = null;
            _currGripEntityId = ObjectId.Null;
            _currGripPointIndex = -1;
            return posInModel;
        }
    }
}
