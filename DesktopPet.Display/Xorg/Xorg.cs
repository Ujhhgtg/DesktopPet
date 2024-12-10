using System;

namespace DesktopPet.Display.Xorg;

public static class Xorg
{
    public static IntPtr GetDisplay()
    {
        return XorgPInvoke.XOpenDisplay(null!);
    }

    public static int GetDefaultScreen(IntPtr display)
    {
        return XorgPInvoke.DefaultScreen(display);
    }

    public static void MaskWindow(IntPtr display, IntPtr window)
    {
        var emptyPixmap = XorgPInvoke.XCreatePixmap(display, window, 1, 1, 1);
        var gc = XorgPInvoke.XCreateGC(display, emptyPixmap, 0, IntPtr.Zero);
        XorgPInvoke.XShapeCombineMask(display, window, XorgPInvoke.ShapeInput, 0, 0, emptyPixmap, XorgPInvoke.ShapeSet);
        XorgPInvoke.XFreeGC(display, gc);
        XorgPInvoke.XFreePixmap(display, emptyPixmap);
    }
}