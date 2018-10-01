using System;
using System.Collections.Generic;
using System.Drawing;

namespace LitCAD
{
    /// <summary>
    /// https://www.codeproject.com/articles/4958/combining-gdi-and-gdi-to-draw-rubber-band-rectangl
    /// </summary>
    internal static class XorGDI
    {
        internal enum PenStyles
        {
            PS_SOLID = 0,
            PS_DASH = 1,
            PS_DOT = 2,
            PS_DASHDOT = 3,
            PS_DASHDOTDOT = 4
        }

        private static int NULL_BRUSH = 5;
        private static int R2_XORPEN = 7;
        private static int TRANSPARENT = 1;

        public static void DrawRectangle(
            Graphics g,
            PenStyles penStyle, Color penColor, int penWidth,
            LitMath.Vector2 pnt1, LitMath.Vector2 pnt2)
        {
            // Extract the Win32 HDC from the Graphics object supplied.
            IntPtr hdc = g.GetHdc();

            // Create a pen with a dotted style to draw the border of the
            // rectangle.
            IntPtr gdiPen = CreatePen(penStyle, penWidth, RGB(penColor.R, penColor.G, penColor.B));

            //
            SetBkMode(hdc, TRANSPARENT);

            // Set the ROP cdrawint mode to XOR.
            SetROP2(hdc, R2_XORPEN);

            // Select the pen into the device context.
            IntPtr oldPen = SelectObject(hdc, gdiPen);

            // Create a stock NULL_BRUSH brush and select it into the device
            // context so that the rectangle isn't filled.
            IntPtr oldBrush = SelectObject(hdc, GetStockObject(NULL_BRUSH));

            // Now XOR the hollow rectangle on the Graphics object with
            // a dotted outline.
            Rectangle(hdc, (int)pnt1.x, (int)pnt1.y, (int)pnt2.x, (int)pnt2.y);

            // Put the old stuff back where it was.
            SelectObject(hdc, oldBrush); // no need to delete a stock object
            SelectObject(hdc, oldPen);
            DeleteObject(gdiPen);		// but we do need to delete the pen

            // Return the device context to Windows.
            g.ReleaseHdc(hdc);
        }

        public static void DrawRectangle(
            Graphics g, IntPtr pen,
            LitMath.Vector2 pnt1, LitMath.Vector2 pnt2)
        {
            // Extract the Win32 HDC from the Graphics object supplied.
            IntPtr hdc = g.GetHdc();

            //
            SetBkMode(hdc, TRANSPARENT);

            // Set the ROP cdrawint mode to XOR.
            SetROP2(hdc, R2_XORPEN);

            // Select the pen into the device context.
            IntPtr oldPen = SelectObject(hdc, pen);

            // Create a stock NULL_BRUSH brush and select it into the device
            // context so that the rectangle isn't filled.
            IntPtr oldBrush = SelectObject(hdc, GetStockObject(NULL_BRUSH));

            // Now XOR the hollow rectangle on the Graphics object with
            // a dotted outline.
            Rectangle(hdc, (int)pnt1.x, (int)pnt1.y, (int)pnt2.x, (int)pnt2.y);

            // Put the old stuff back where it was.
            SelectObject(hdc, oldBrush); // no need to delete a stock object
            SelectObject(hdc, oldPen);

            // Return the device context to Windows.
            g.ReleaseHdc(hdc);
        }

        // Use Interop to call the corresponding Win32 GDI functions
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern int SetROP2(
                IntPtr hdc,		// Handle to a Win32 device context
                int enDrawMode	// Drawing mode
                );

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern int SetBkMode(IntPtr hdc, int iBkMode);

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        internal static extern IntPtr CreatePen(
                PenStyles enPenStyle,	// Pen style from enum PenStyles
                int nWidth,				// Width of pen
                int crColor				// Color of pen
                );
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern bool DeleteObject(
                IntPtr hObject	// Win32 GDI handle to object to delete
                );
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern IntPtr SelectObject(
                IntPtr hdc,		// Win32 GDI device context
                IntPtr hObject	// Win32 GDI handle to object to select
                );
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern void Rectangle(
                IntPtr hdc,			// Handle to a Win32 device context
                int X1,				// x-coordinate of top left corner
                int Y1,				// y-cordinate of top left corner
                int X2,				// x-coordinate of bottom right corner
                int Y2				// y-coordinate of bottm right corner
                );
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern IntPtr GetStockObject(
                int brStyle	// Selected from the WinGDI.h BrushStyles enum
                );

        // C# version of Win32 RGB macro
        internal static int RGB(int R, int G, int B)
        {
            return (R | (G << 8) | (B << 16));
        }
    }
}
