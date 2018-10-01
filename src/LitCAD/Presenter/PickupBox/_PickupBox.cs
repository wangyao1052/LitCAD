using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.ApplicationServices;
using LitCAD.DatabaseServices;

namespace LitCAD.UI
{
    /// <summary>
    /// 拾取框
    /// 以Canvas坐标为准
    /// </summary>
    internal class PickupBox
    {
        private Presenter _presenter = null;
        internal Presenter presenter
        {
            get { return _presenter; }
        }
        private Dictionary<Type, EntityHitter> _entType2EntHitter = new Dictionary<Type, EntityHitter>();

        /// <summary>
        ///  中心位置
        /// </summary>
        private LitMath.Vector2 _center = new LitMath.Vector2(0, 0);
        internal LitMath.Vector2 center
        {
            get { return _center; }
            set { _center = value; }
        }

        /// <summary>
        /// 边长
        /// </summary>
        private int _side = 20;
        internal int side
        {
            get { return _side; }
            set
            {
                if (_side != value)
                {
                    _side = value;
                    //this.RenewBitmap();
                }
            }
        }

        /// <summary>
        /// 位图
        /// </summary>
        //private Bitmap _bitmap = null;
        //private void RenewBitmap()
        //{
        //    _bitmap = new Bitmap(_side * 3, _side * 3);
        //    Graphics graphics = Graphics.FromImage(_bitmap);

        //    Pen pen = GDIResMgr.Instance.GetPen(Color.White, 1);
        //    graphics.DrawRectangle(pen,
        //        (_bitmap.Width - _side) / 2, (_bitmap.Height - _side) / 2,
        //        _side, _side);
        //    graphics.DrawLine(pen,
        //        _bitmap.Width / 2, 0,
        //        _bitmap.Width / 2, _bitmap.Height);
        //    graphics.DrawLine(pen,
        //        0, _bitmap.Height / 2,
        //        _bitmap.Width, _bitmap.Height / 2);
        //}

        /// <summary>
        /// 预留外包围盒
        /// </summary>
        private Bounding _reservedBounding = new Bounding();
        internal Bounding reservedBounding
        {
            get { return _reservedBounding; }
        }

        private void UpdateReservedBounding()
        {
            LitMath.Vector2 centerInModel = _presenter.CanvasToModel(_center);
            double sideInModel = _presenter.CanvasToModel(_side);
            _reservedBounding = new Bounding(centerInModel, sideInModel, sideInModel);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        internal PickupBox(Presenter presenter)
        {
            _presenter = presenter;
            //RenewBitmap();

            //
            RegisterEntityHitter(typeof(Line), new LineHitter());
            RegisterEntityHitter(typeof(Xline), new XlineHitter());
            RegisterEntityHitter(typeof(Ray), new RayHitter());
            RegisterEntityHitter(typeof(Polyline), new PolylineHitter());
            RegisterEntityHitter(typeof(Circle), new CircleHitter());
            RegisterEntityHitter(typeof(Arc), new ArcHitter());
            RegisterEntityHitter(typeof(Text), new TextHitter());
        }

        private void RegisterEntityHitter(Type entType, EntityHitter entHitter)
        {
            _entType2EntHitter[entType] = entHitter;
        }

        ///// <summary>
        ///// Paint
        ///// </summary>
        //internal void OnPaint(Graphics graphics)
        //{
        //    graphics.DrawImage(_bitmap,
        //        (float)(_center.x - _bitmap.Width / 2), (float)(_center.y - _bitmap.Height / 2));
        //}

        internal List<Selection> Select(Block block)
        {
            UpdateReservedBounding();

            List<Selection> sels = new List<Selection>();
            foreach (Entity entity in block)
            {
                if (HitEntity(entity))
                {
                    Selection selection = new Selection();
                    selection.objectId = entity.id;
                    selection.position = _presenter.CanvasToModel(this.center);
                    sels.Add(selection);
                }
            }

            return sels;
        }

        private bool HitEntity(Entity entity)
        {
            if (_entType2EntHitter.ContainsKey(entity.GetType()))
            {
                return _entType2EntHitter[entity.GetType()].Hit(this, entity);
            }
            else
            {
                return false;
            }
        }
    }
}
