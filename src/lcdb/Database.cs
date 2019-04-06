using System;
using System.Collections.Generic;
using System.Xml;

using LitCAD.Colors;

namespace LitCAD.DatabaseServices
{
    public class Database
    {
        /// <summary>
        /// 块表
        /// </summary>
        private BlockTable _blockTable = null;
        public BlockTable blockTable
        {
            get { return _blockTable; }
        }
        public static ObjectId BlockTableId
        {
            get { return new ObjectId(TableIds.BlockTableId); }
        }

        /// <summary>
        /// 图层表
        /// </summary>
        private LayerTable _layerTable = null;
        public static ObjectId LayerTableId
        {
            get { return new ObjectId(TableIds.LayerTableId); }
        }
        public LayerTable layerTable
        {
            get { return _layerTable; }
        }

        /// <summary>
        /// ID
        /// </summary>
        private Dictionary<ObjectId, DBObject> _dictId2Object = null;
        internal ObjectId currentMaxId
        {
            get
            {
                if (_dictId2Object == null || _dictId2Object.Count == 0)
                {
                    return ObjectId.Null;
                }
                else
                {
                    ObjectId id = ObjectId.Null;
                    foreach (KeyValuePair<ObjectId, DBObject> kvp in _dictId2Object)
                    {
                        if (kvp.Key.CompareTo(id) > 0)
                        {
                            id = kvp.Key;
                        }
                    }
                    return id;
                }
            }
        }

        private ObjectIdMgr _idMgr = null;

        /// <summary>
        /// 文件名
        /// </summary>
        private string _fileName = null;
        public string fileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Database()
        {
            _dictId2Object = new Dictionary<ObjectId, DBObject>();
            _idMgr = new ObjectIdMgr(this);

            _blockTable = new BlockTable(this);
            Block modelSpace = new Block();
            modelSpace.name = "ModelSpace";
            _blockTable.Add(modelSpace);
            IdentifyDBTable(_blockTable);

            _layerTable = new LayerTable(this);
            IdentifyDBTable(_layerTable);
        }

        /// <summary>
        /// 通过ID获取数据库对象
        /// </summary>
        public DBObject GetObject(ObjectId oid)
        {
            if (_dictId2Object.ContainsKey(oid))
            {
                return _dictId2Object[oid];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="fileFullPath">文件路径</param>
        public void Open(string fileFullPath)
        {
            if (_fileName == null
                || _fileName == "")
            {
                XmlIn(fileFullPath);
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        public void Save()
        {
            if (_fileName != null
                && System.IO.File.Exists(_fileName))
            {
                XmlOut(_fileName);
            }
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="fileFullPath">文件路径</param>
        /// <param name="rename">是否重命名</param>
        public void SaveAs(string fileFullPath, bool rename = false)
        {
            XmlOut(fileFullPath);
            if (rename)
            {
                _fileName = fileFullPath;
            }
        }

        /// <summary>
        /// 写XML文件
        /// </summary>
        /// <param name="xmlFileFullPath">XML文件全路径</param>
        internal void XmlOut(string xmlFileFullPath)
        {
            Filer.XmlFilerImpl xmlFilerImpl = new Filer.XmlFilerImpl();

            //
            xmlFilerImpl.NewSubNodeAndInsert("Database");
            {
                // block table
                xmlFilerImpl.NewSubNodeAndInsert(_blockTable.className);
                _blockTable.XmlOut(xmlFilerImpl);
                xmlFilerImpl.Pop();

                // layer table
                xmlFilerImpl.NewSubNodeAndInsert(_layerTable.className);
                _layerTable.XmlOut(xmlFilerImpl);
                xmlFilerImpl.Pop();
            }
            xmlFilerImpl.Pop();

            //
            xmlFilerImpl.Save(xmlFileFullPath);
        }

        /// <summary>
        /// 读XML文件
        /// </summary>
        internal bool XmlIn(string xmlFileFullPath)
        {
            Filer.XmlFilerImpl xmlFilerImpl = new Filer.XmlFilerImpl();
            xmlFilerImpl.Load(xmlFileFullPath);

            //
            XmlDocument xmldoc = xmlFilerImpl.xmldoc;
            XmlNode dbNode = xmldoc.SelectSingleNode("Database");
            if (dbNode == null)
            {
                return false;
            }
            xmlFilerImpl.curXmlNode = dbNode;

            //
            ClearLayerTable();
            ClearBlockTable();

            // layer table
            XmlNode layerTblNode = dbNode.SelectSingleNode(_layerTable.className);
            if (layerTblNode == null)
            {
                return false;
            }
            xmlFilerImpl.curXmlNode = layerTblNode;
            _layerTable.XmlIn(xmlFilerImpl);

            // block table
            XmlNode blockTblNode = dbNode.SelectSingleNode(_blockTable.className);
            if (blockTblNode == null)
            {
                return false;
            }
            xmlFilerImpl.curXmlNode = blockTblNode;
            _blockTable.XmlIn(xmlFilerImpl);

            //
            _fileName = xmlFileFullPath;
            _idMgr.reset();
            return true;
        }

        /// <summary>
        /// 清空图层表
        /// </summary>
        private void ClearLayerTable()
        {
            List<Layer> allLayers = new List<Layer>();
            foreach (Layer layer in _layerTable)
            {
                allLayers.Add(layer);
            }
            _layerTable.Clear();

            foreach (Layer layer in allLayers)
            {
                layer.Erase();
            }
        }

        /// <summary>
        /// 清空块表
        /// </summary>
        private void ClearBlockTable()
        {
            Dictionary<Entity, Entity> allEnts = new Dictionary<Entity,Entity>();
            List<Block> allBlocks = new List<Block>();

            foreach (Block block in _blockTable)
            {
                foreach (Entity entity in block)
                {
                    allEnts[entity] = entity;
                }
                block.Clear();
                allBlocks.Add(block);
            }
            _blockTable.Clear();

            foreach (KeyValuePair<Entity, Entity> kvp in allEnts)
            {
                kvp.Key.Erase();
            }

            foreach (Block block in allBlocks)
            {
                block.Erase();
            }
        }

        #region IdentityObject
        private void IdentifyDBTable(DBTable table)
        {
            MapSingleObject(table);
        }

        internal void IdentifyObject(DBObject obj)
        {
            IdentifyObjectSingle(obj);
            if (obj is Block)
            {
                Block block = obj as Block;
                foreach (Entity entity in block)
                {
                    IdentifyObjectSingle(entity);
                }
            }
        }

        private void IdentifyObjectSingle(DBObject obj)
        {
            if (obj.id.isNull)
            {
                obj.SetId(_idMgr.NextId);
            }
            MapSingleObject(obj);
        }
        #endregion

        #region MapObject
        private void MapObject(DBObject obj)
        {
            MapSingleObject(obj);
            if (obj is Block)
            {
                Block block = obj as Block;
                foreach (Entity entity in block)
                {
                    MapSingleObject(entity);
                }
            }
        }

        internal void UnmapObject(DBObject obj)
        {
            UnmapSingleObject(obj);
            if (obj is Block)
            {
                Block block = obj as Block;
                foreach (Entity entity in block)
                {
                    UnmapSingleObject(entity);
                }
            }
        }

        private void MapSingleObject(DBObject obj)
        {
            _dictId2Object[obj.id] = obj;
        }

        private void UnmapSingleObject(DBObject obj)
        {
            _dictId2Object.Remove(obj.id);
        }
        #endregion
    }
}
