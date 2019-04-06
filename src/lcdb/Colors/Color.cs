using System;

namespace LitCAD.Colors
{
    public struct Color
    {
        //
        private ColorMethod _colorMethod;
        public ColorMethod colorMethod
        {
            get { return _colorMethod; }
        }

        /// <summary>
        /// 颜色RGB
        /// </summary>
        private byte _r;
        private byte _g;
        private byte _b;
        public byte r
        {
            get { return _r; }
        }

        public byte g
        {
            get { return _g; }
        }

        public byte b
        {
            get { return _b; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private Color(byte r, byte g, byte b)
        {
            _colorMethod = ColorMethod.ByColor;
            _r = r;
            _g = g;
            _b = b;
        }

        public string Name
        {
            get
            {
                switch (_colorMethod)
                {
                    case ColorMethod.ByLayer:
                        return "ByLayer";

                    case ColorMethod.ByBlock:
                        return "ByBlock";

                    case ColorMethod.None:
                        return "None";

                    case ColorMethod.ByColor:
                        return string.Format("{0},{1},{2}", _r, _g, _b);

                    default:
                        return "";
                }
            }
        }

        public static Color FromRGB(byte r, byte g, byte b)
        {
            return new Color(r, g, b);
        }

        public static Color FromColor(System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B);
        }

        public static Color ByLayer
        {
            get
            {
                Color color = new Color();
                color._colorMethod = ColorMethod.ByLayer;
                color._r = 255;
                color._g = 255;
                color._b = 255;
                return color;
            }
        }

        public static Color ByBlock
        {
            get
            {
                Color color = new Color();
                color._colorMethod = ColorMethod.ByBlock;
                color._r = 255;
                color._g = 255;
                color._b = 255;
                return color;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1},{2},{3}", _colorMethod.ToString(), _r, _g, _b);
        }

        internal static bool TryParse(string str, out Color result)
        {
            string[] arr = str.Split(':');
            if (arr.Length != 2)
            {
                result = Color.ByLayer;
                return false;
            }

            //
            ColorMethod colorMethod = (ColorMethod)Enum.Parse(typeof(LitCAD.Colors.ColorMethod), arr[0], true);

            //
            string[] rgb = arr[1].Split(',');
            if (rgb.Length != 3)
            {
                result = Color.ByLayer;
                return false;
            }

            byte red = 0;
            byte green = 0;
            byte blue = 0;
            if (byte.TryParse(rgb[0], out red)
                && byte.TryParse(rgb[1], out green)
                && byte.TryParse(rgb[2], out blue))
            {
                result = new Color();
                result._colorMethod = colorMethod;
                result._r = red;
                result._g = green;
                result._b = blue;

                return true;
            }
            else
            {
                result = Color.ByLayer;
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Color))
                return false;

            return Equals((Color)obj);
        }

        public bool Equals(Color rhs)
        {
            if (_colorMethod != rhs._colorMethod)
            {
                return false;
            }

            switch (_colorMethod)
            {
                case ColorMethod.ByColor:
                    return _r == rhs._r 
                        && _g == rhs._g 
                        && _b == rhs._b;

                case ColorMethod.ByBlock:
                case ColorMethod.ByLayer:
                case ColorMethod.None:
                default:
                    return true;
            }
        }

        public override int GetHashCode()
        {
            return _colorMethod.GetHashCode();
        }

        public static bool operator ==(Color lhs, Color rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Color lhs, Color rhs)
        {
            return !(lhs == rhs);
        }
    }
}
