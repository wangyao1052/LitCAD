using System;

namespace LitCAD.DatabaseServices
{
    /// <summary>
    /// 矩形包围框
    /// </summary>
    public struct Bounding
    {
        /// <summary>
        /// Center
        /// </summary>
        public LitMath.Vector2 center
        {
            get
            { 
                return new LitMath.Vector2(
                    (_left + _right) / 2,
                    (_bottom + _top) / 2); 
            }
        }

        /// <summary>
        /// Width
        /// </summary>
        private double _width;
        public double width
        {
            get { return _width; }
            //set { _width = value; }
        }

        /// <summary>
        /// Height
        /// </summary>
        private double _height;
        public double height
        {
            get { return _height; }
            //set { _height = value; }
        }

        /// <summary>
        /// Left
        /// </summary>
        private double _left;
        public double left
        {
            get { return _left; }
        }

        /// <summary>
        /// Right
        /// </summary>
        private double _right;
        public double right
        {
            get { return _right; }
        }

        /// <summary>
        /// Top
        /// </summary>
        private double _top;
        public double top
        {
            get { return _top; }
        }

        /// <summary>
        /// Bottom
        /// </summary>
        private double _bottom;
        public double bottom
        {
            get { return _bottom; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Bounding(LitMath.Vector2 point1, LitMath.Vector2 point2)
        {
            if (point1.x < point2.x)
            {
                _left = point1.x;
                _right = point2.x;
            }
            else
            {
                _left = point2.x;
                _right = point1.x;
            }

            if (point1.y < point2.y)
            {
                _bottom = point1.y;
                _top = point2.y;
            }
            else
            {
                _bottom = point2.y;
                _top = point1.y;
            }

            _width = _right - _left;
            _height = _top - _bottom;
        }

        public Bounding(LitMath.Vector2 center, double width, double height)
        {
            _left = center.x - width / 2;
            _right = center.x + width / 2;
            _bottom = center.y - height / 2;
            _top = center.y + height / 2;
            _width = _right - _left;
            _height = _top - _bottom;
        }

        /// <summary>
        /// Check whether contains bounding
        /// </summary>
        public bool Contains(Bounding bounding)
        {
            return this.Contains(bounding.left, bounding.bottom)
                && this.Contains(bounding.right, bounding.top);
        }

        /// <summary>
        /// Check whether contains point
        /// </summary>
        public bool Contains(LitMath.Vector2 point)
        {
            return this.Contains(point.x, point.y);
        }

        /// <summary>
        /// Check whether contains point: (x, y)
        /// </summary>
        public bool Contains(double x, double y)
        {
            return x >= this.left
                && x <= this.right
                && y >= this.bottom
                && y <= this.top;
        }

        /// <summary>
        /// Check whether intersect with bounding
        /// </summary>
        public bool IntersectWith(Bounding bounding)
        {
            bool b1 = (bounding.left >= this.left && bounding.left <= this.right)
                || (bounding.right >= this.left && bounding.right <= this.right)
                || (bounding.left <= this.left && bounding.right >= this.right);

            if (b1)
            {
                bool b2 = (this.bottom >= bounding.bottom && this.bottom <= bounding.top)
                    || (this.top >= bounding.bottom && this.top <= bounding.top)
                    || (this.bottom <= this.bottom && this.top >= bounding.top);
                if (b2)
                {
                    return true;
                }
            }
            

            return false;
        }

        private bool ValueInRange(double value, double min, double max)
        {
            return value >= min && value <= max;
        }
    }
}
