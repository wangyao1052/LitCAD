using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;

namespace LitCAD
{
    internal abstract class EntityAnchorCreator
    {
        public abstract List<EntityAnchor> NewAnchors(Entity entity);
    }
}
