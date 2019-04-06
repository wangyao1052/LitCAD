using System;
using System.Collections.Generic;
using System.Xml;

namespace LitCAD.DatabaseServices
{
    public class LayerTable : DBTable
    {
        /// <summary>
        /// 类名
        /// </summary>
        public override string className
        {
            get { return "LayerTable"; }
        }

        /// <summary>
        /// 
        /// </summary>
        private ObjectId _layerZeroId = ObjectId.Null;
        public ObjectId layerZeroId
        {
            get { return _layerZeroId; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        internal LayerTable(Database db)
            : base(db, Database.LayerTableId)
        {
            Layer layerZero = new Layer("0");
            this.Add(layerZero);

            _layerZeroId = layerZero.id;
        }

        /// <summary>
        /// 读XML
        /// </summary>
        public override void XmlIn(Filer.XmlFiler filer)
        {
            Filer.XmlFilerImpl filerImpl = filer as Filer.XmlFilerImpl;

            base.XmlIn(filer);

            XmlNode curXmlNode = filerImpl.curXmlNode;
            XmlNodeList layers = curXmlNode.SelectNodes("Layer");
            foreach (XmlNode layerNode in layers)
            {
                Layer layer = new Layer();
                filerImpl.curXmlNode = layerNode;
                layer.XmlIn(filerImpl);
                this._Add(layer);
            }
            filerImpl.curXmlNode = curXmlNode;
        }
    }
}
