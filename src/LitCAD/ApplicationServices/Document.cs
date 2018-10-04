using System;
using System.Collections.Generic;

using LitCAD.DatabaseServices;
using LitCAD.Colors;

namespace LitCAD.ApplicationServices
{
    public class Document : IDocument
    {
        /// <summary>
        /// 数据库
        /// </summary>
        private Database _database = null;
        public Database database
        {
            get { return _database; }
        }

        /// <summary>
        /// 当前块名称
        /// </summary>
        private string _currentBlockName = "ModelSpace";
        public string currentBlockName
        {
            get { return _currentBlockName; }
        }

        /// <summary>
        /// 选择集
        /// </summary>
        private Selections _selections = null;
        public Selections selections
        {
            get { return _selections; }
        }

        /// <summary>
        /// 当前图层
        /// </summary>
        private ObjectId _currLayerId = ObjectId.Null;
        public ObjectId currentLayerId
        {
            get { return _currLayerId; }
            set
            {
                if (_currLayerId != value)
                {
                    Layer layer = _database.GetObject(value) as Layer;
                    if (layer != null)
                    {
                        ObjectId last = _currLayerId;
                        _currLayerId = value;
                        if (currentLayerChanged != null)
                        {
                            currentLayerChanged.Invoke(last, _currLayerId);
                        }
                    }
                    else
                    {
                        throw new System.Exception("invalid layer id");
                    }
                }
            }
        }

        public delegate void CurrentLayerChanged(ObjectId last, ObjectId current);
        public event CurrentLayerChanged currentLayerChanged;

        /// <summary>
        /// 常用颜色集
        /// </summary>
        private CommonColors _commonColors = null;
        internal CommonColors commonColors
        {
            get { return _commonColors; }
        }

        /// <summary>
        /// 当前图元颜色
        /// </summary>
        private Color _currColor = Color.ByLayer;
        public Color currentColor
        {
            get { return _currColor; }
            set
            {
                Color last = _currColor;
                _currColor = value;
                if (currentColorChanged != null)
                {
                    currentColorChanged.Invoke(last, _currColor);
                }
            }
        }

        public System.Drawing.Color currentColorValue
        {
            get
            {
                switch (_currColor.colorMethod)
                {
                    case ColorMethod.ByLayer:
                        Layer layer = _database.GetObject(_currLayerId) as Layer;
                        if (layer != null)
                        {
                            return layer.colorValue;
                        }
                        else
                        {
                            return System.Drawing.Color.FromArgb(
                                _currColor.r, _currColor.g, _currColor.b);
                        }

                    case ColorMethod.ByColor:
                    case ColorMethod.ByBlock:
                    case ColorMethod.None:
                    default:
                        return System.Drawing.Color.FromArgb(
                            _currColor.r, _currColor.g, _currColor.b);
                }
            }
        }

        public delegate void CurrentColorChanged(Color last, Color current);
        public event CurrentColorChanged currentColorChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        internal Document()
        {
            _database = new DatabaseServices.Database();
            _selections = new Selections();
            _commonColors = new CommonColors();
            _currLayerId = _database.layerTable["0"].id;
            _currColor = Color.ByLayer;
        }
    }
}
