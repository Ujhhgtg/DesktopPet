using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace DesktopPet.Display.Xorg;

[SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments")]
internal class XorgPInvoke
{
    [DllImport("libX11.so")]
    public static extern IntPtr XOpenDisplay(string display);

    [DllImport("libX11.so")]
    public static extern int DefaultScreen(IntPtr display);

    [DllImport("libX11.so")]
    public static extern IntPtr RootWindow(IntPtr display, int screen);

    [DllImport("libX11.so")]
    public static extern IntPtr XCreateSimpleWindow(IntPtr display, IntPtr parent, int x, int y, uint width, uint height, uint borderWidth, ulong border, ulong background);

    [DllImport("libX11.so")]
    public static extern void XMapWindow(IntPtr display, IntPtr window);

    [DllImport("libX11.so")]
    public static extern IntPtr XCreatePixmap(IntPtr display, IntPtr drawable, uint width, uint height, uint depth);

    [DllImport("libX11.so")]
    public static extern IntPtr XCreateGC(IntPtr display, IntPtr drawable, ulong valuemask, IntPtr values);

    [DllImport("libX11.so")]
    public static extern void XSetForeground(IntPtr display, IntPtr gc, ulong foreground);

    [DllImport("libX11.so")]
    public static extern void XFillRectangle(IntPtr display, IntPtr drawable, IntPtr gc, int x, int y, uint width, uint height);

    [DllImport("libX11.so")]
    public static extern void XFreeGC(IntPtr display, IntPtr gc);

    [DllImport("libX11.so")]
    public static extern void XFreePixmap(IntPtr display, IntPtr pixmap);

    [DllImport("libX11.so")]
    public static extern void XNextEvent(IntPtr display, out XEvent xevent);

    [DllImport("libX11.so")]
    public static extern void XCloseDisplay(IntPtr display);

    
    [DllImport("libXext.so")]
    public static extern void XShapeCombineMask(IntPtr display, IntPtr window, int shapeKind, int x, int y, IntPtr pixmap, int operation);

    public const int ShapeInput = 2; // Shape input kind
    public const int ShapeSet = 0;   // Shape operation

    [StructLayout(LayoutKind.Sequential)]
    public struct XEvent
    {
        public int type;
        public IntPtr pad0;
        public IntPtr pad1;
        public IntPtr pad2;
        public IntPtr pad3;
        public IntPtr pad4;
        public IntPtr pad5;
        public IntPtr pad6;
        public IntPtr pad7;
        public IntPtr pad8;
        public IntPtr pad9;
        public IntPtr pad10;
        public IntPtr pad11;
        public IntPtr pad12;
        public IntPtr pad13;
        public IntPtr pad14;
        public IntPtr pad15;
        public IntPtr pad16;
        public IntPtr pad17;
        public IntPtr pad18;
        public IntPtr pad19;
        public IntPtr pad20;
    }
}