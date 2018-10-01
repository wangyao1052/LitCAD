using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class PolylineAnchorCreator : EntityAnchorCreator
    {
        public override List<EntityAnchor> NewAnchors(Entity entity)
        {
            Polyline polyline = entity as Polyline;

            List<EntityAnchor> lstAnchor = new List<EntityAnchor>();
            int numOfVertices = polyline.NumberOfVertices;
            for (int i = 0; i < numOfVertices; ++i)
            {
                PolylineAnchor polylineAnchor = new PolylineAnchor(polyline, PolylineAnchor.Type.VertexPoint, i);
                lstAnchor.Add(polylineAnchor);
            }
            for (int i = 0; i < numOfVertices - 1; ++i)
            {
                PolylineAnchor polylineAnchor = new PolylineAnchor(polyline, PolylineAnchor.Type.MiddlePoint, -1, i);
                lstAnchor.Add(polylineAnchor);
            }
            if (polyline.closed
                && numOfVertices > 2)
            {
                PolylineAnchor polylineAnchor = new PolylineAnchor(polyline, PolylineAnchor.Type.MiddlePoint, -1, numOfVertices - 1);
                lstAnchor.Add(polylineAnchor);
            }

            return lstAnchor;
        }
    }
}
