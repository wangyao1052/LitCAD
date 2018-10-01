using System;

namespace LitCAD.DatabaseServices
{
    /// <summary>
    /// 块参照
    /// </summary>
    public class BlockReference : Entity
    {
        /// <summary>
        /// 外围边框
        /// </summary>
        public override Bounding bounding
        {
            get
            {
                return new Bounding();
            }
        }

        /// <summary>
        /// 克隆函数
        /// </summary>
        public override object Clone()
        {
            BlockReference blkRef = base.Clone() as BlockReference;
            return blkRef;
        }

        protected override DBObject CreateInstance()
        {
            return new BlockReference();
        }

        /// <summary>
        /// 平移
        /// </summary>
        public override void Translate(LitMath.Vector2 translation)
        {
        }

        /// <summary>
        /// Transform
        /// </summary>
        public override void TransformBy(LitMath.Matrix3 transform)
        {
        }
    }
}
