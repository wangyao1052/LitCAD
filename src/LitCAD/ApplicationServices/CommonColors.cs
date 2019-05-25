using System;
using System.Collections.Generic;

using LitCAD.Colors;

namespace LitCAD.ApplicationServices
{
    /// <summary>
    /// 常用颜色集合
    /// </summary>
    internal class CommonColors : IEnumerable<Color>
    {
        private Dictionary<Color, string> _predefinedColors = new Dictionary<Color, string>();
        private List<Color> _commonColors = new List<Color>();

        public CommonColors()
        {
            InitPredefinedColors();
        }

        /// <summary>
        /// Initialize predefined colors
        /// </summary>
        private void InitPredefinedColors()
        {
            _predefinedColors.Add(Color.ByLayer, Color.ByLayer.Name);
            _predefinedColors.Add(Color.ByBlock, Color.ByBlock.Name);

            // Red
            _predefinedColors.Add(Color.FromRGB(255, 0, 0), "红");
            // Yellow
            _predefinedColors.Add(Color.FromRGB(255, 255, 0), "黄");
            // Green
            _predefinedColors.Add(Color.FromRGB(0, 255, 0), "绿");
            // Cyan
            _predefinedColors.Add(Color.FromRGB(0, 255, 255), "青");
            // Blue
            _predefinedColors.Add(Color.FromRGB(0, 0, 255), "蓝");
            // Magenta
            _predefinedColors.Add(Color.FromRGB(255, 0, 255), "洋红");
            // White
            _predefinedColors.Add(Color.FromRGB(255, 255, 255), "白");
        }

        public string GetColorName(Color color)
        {
            if (_predefinedColors.ContainsKey(color))
            {
                return _predefinedColors[color];
            }
            else
            {
                return color.Name;
            }
        }

        public bool Add(Color color)
        {
            if (_predefinedColors.ContainsKey(color)
                || _commonColors.Contains(color))
            {
                return false;
            }
            else
            {
                _commonColors.Add(color);
                return true;
            }
        }

        public void Clear()
        {
            _commonColors.Clear();
        }

        #region IEnumerable<Color>
        public IEnumerator<Color> GetEnumerator()
        {
            List<Color> colors = new List<Color>(_predefinedColors.Count + _commonColors.Count);
            foreach (Color color in _predefinedColors.Keys)
            {
                colors.Add(color);
            }
            foreach (Color color in _commonColors)
            {
                colors.Add(color);
            }

            return colors.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
