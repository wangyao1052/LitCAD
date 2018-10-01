using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal class TextAnchorCreator : EntityAnchorCreator
    {
        public override List<EntityAnchor> NewAnchors(Entity entity)
        {
            List<EntityAnchor> lstAnchor = new List<EntityAnchor>();
            Text text = entity as Text;
            if (text == null)
            {
                return lstAnchor;
            }

            TextAnchor textBasePnt = new TextAnchor(text, TextAnchor.Type.BasePoint);
            lstAnchor.Add(textBasePnt);

            return lstAnchor;
        }
    }
}
