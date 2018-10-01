using System;

using LitCAD.DatabaseServices;

namespace LitCAD.UI
{
    internal abstract class EntityHitter
    {
        internal abstract bool Hit(PickupBox pkbox, Entity entity);
    }
}
