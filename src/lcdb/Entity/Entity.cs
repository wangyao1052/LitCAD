using System;
using System.Collections.Generic;

using LitCAD.Colors;

namespace LitCAD.DatabaseServices
{
    public abstract class Entity : DBObject
    {
        /// <summary>
        /// 外围边框
        /// </summary>
        public abstract Bounding bounding
        {
            get;
        }

        /// <summary>
        /// 颜色
        /// </summary>
        private Color _color = Color.ByLayer;
        public Color color
        {
            get { return _color; }
            set { _color = value; }
        }

        public System.Drawing.Color colorValue
        {
            get
            {
                switch (_color.colorMethod)
                {
                    case ColorMethod.ByBlock:
                        if (this.parent != null
                            && this.parent is BlockReference)
                        {
                            BlockReference blockRef = this.parent as BlockReference;
                            return blockRef.colorValue;
                        }
                        else
                        {
                            return System.Drawing.Color.FromArgb(_color.r, _color.g, _color.b);
                        }

                    case ColorMethod.ByLayer:
                        Database db = this.database;
                        if (db != null
                            && db.layerTable.Has(this.layer))
                        {
                            Layer layer = db.layerTable[this.layer] as Layer;
                            return layer.colorValue;
                        }
                        else
                        {
                            return System.Drawing.Color.FromArgb(_color.r, _color.g, _color.b);
                        }

                    case ColorMethod.ByColor:
                    case ColorMethod.None:
                    default:
                        return System.Drawing.Color.FromArgb(_color.r, _color.g, _color.b);
                }
            }
        }

        /// <summary>
        /// 线宽
        /// </summary>
        private LineWeight _lineWeight = LineWeight.ByLayer;
        public LineWeight lineWeight
        {
            get { return _lineWeight; }
            set { _lineWeight = value; }
        }

        /// <summary>
        /// 图层
        /// </summary>
        private ObjectId _layerId = ObjectId.Null;
        public ObjectId layerId
        {
            get { return _layerId; }
            set { _layerId = value; }
        }

        public string layer
        {
            get
            {
                Database db = this.database;
                if (db != null 
                    && _layerId != ObjectId.Null
                    && db.layerTable.Has(_layerId))
                {
                    Layer layerRecord = db.GetObject(_layerId) as Layer;
                    return layerRecord.name;
                }
                return "";
            }
            set
            {
                Database db = this.database;
                if (db != null
                    && db.layerTable.Has(value))
                {
                    _layerId = db.layerTable[value].id;
                }
            }
        }

        

        /// <summary>
        /// 克隆函数
        /// </summary>
        public override object Clone()
        {
            Entity entity = base.Clone() as Entity;
            entity._color = _color;
            entity._layerId = _layerId;
            return entity;
        }

        /// <summary>
        /// 平移
        /// </summary>
        public abstract void Translate(LitMath.Vector2 translation);

        /// <summary>
        /// Transform
        /// 前提: 变换不改变图元整体形状
        /// </summary>
        public abstract void TransformBy(LitMath.Matrix3 transform);

        /// <summary>
        /// 移除
        /// </summary>
        protected override void _Erase()
        {
            if (_parent != null)
            {
                Block block = _parent as Block;
                block.RemoveEntity(this);
            }
        }
    }
}
