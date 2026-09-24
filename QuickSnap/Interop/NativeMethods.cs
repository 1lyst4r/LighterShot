using System;
using System.Runtime.InteropServices;

namespace LightlyShot.Interop;
internal static class NativeMethods
{
    public const int KeyboardHookId = 13;      // WH_KEYBOARD_LL
    public const int MouseHookId = 14;         // WH_MOUSE_LL

    public const int KeyDownMessage = 0x0100;
    public const int KeyUpMessage = 0x0101;
    public const int SystemKeyDownMessage = 0x0104;   // key down while Alt is held
    public const int SystemKeyUpMessage = 0x0105;
    public const int XButtonDownMessage = 0x020B;     // mouse side buttons
    public const int XButtonUpMessage = 0x020C;

    public delegate IntPtr LowLevelHookCallback(int hookCode, IntPtr messageId, IntPtr eventData);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int hookId, LowLevelHookCallback callback, IntPtr moduleHandle, uint threadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

    [DllImport("user32.dll")]
    public static extern IntPtr CallNextHookEx(IntPtr hookHandle, int hookCode, IntPtr messageId, IntPtr eventData);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr GetModuleHandle(string? moduleName);

    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int virtualKey);
    public static readonly IntPtr TopmostWindowGroup = new(-1);   // HWND_TOPMOST
    public const uint DoNotActivate = 0x0010;                     // SWP_NOACTIVATE

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr windowHandle, IntPtr insertAfter, int x, int y, int width, int height, uint flags);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(IntPtr windowHandle);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr windowHandle, out uint processId);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AttachThreadInput(uint threadToAttach, uint threadToAttachTo, [MarshalAs(UnmanagedType.Bool)] bool attach);

    [StructLayout(LayoutKind.Sequential)]
    public struct NativePoint
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(out NativePoint point);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyIcon(IntPtr iconHandle);
    public static void ForceToForeground(IntPtr windowHandle)
    {
        IntPtr currentForeground = GetForegroundWindow();
        if (currentForeground == windowHandle)
        {
            return;
        }

        uint foregroundThread = GetWindowThreadProcessId(currentForeground, out _);
        uint ourThread = GetCurrentThreadId();
        bool attached = foregroundThread != 0 && foregroundThread != ourThread
                        && AttachThreadInput(ourThread, foregroundThread, true);

        SetForegroundWindow(windowHandle);

        if (attached)
        {
            AttachThreadInput(ourThread, foregroundThread, false);
        }
    }
    public const int CursorIsShowing = 0x0001;    // CURSOR_SHOWING
    public const uint DrawIconNormal = 0x0003;    // DI_NORMAL

    [StructLayout(LayoutKind.Sequential)]
    public struct CursorInfo
    {
        public int Size;
        public int Flags;
        public IntPtr CursorHandle;
        public NativePoint ScreenPosition;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IconInfo
    {
        [MarshalAs(UnmanagedType.Bool)]
        public bool IsIcon;
        public int HotspotX;
        public int HotspotY;
        public IntPtr MaskBitmap;
        public IntPtr ColorBitmap;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorInfo(ref CursorInfo cursorInfo);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetIconInfo(IntPtr iconHandle, out IconInfo iconInfo);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DrawIconEx(IntPtr deviceContext, int x, int y, IntPtr iconHandle,
                                         int width, int height, uint frameIndex, IntPtr backgroundBrush, uint flags);
    public const uint CopySourceToDestination = 0x00CC0020;   // SRCCOPY
    public const uint IncludeLayeredWindows = 0x40000000;     // CAPTUREBLT

    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfoHeader
    {
        public int Size;
        public int Width;
        public int Height;          // negative = top-down rows
        public short Planes;
        public short BitCount;
        public int Compression;
        public int SizeImage;
        public int XPixelsPerMeter;
        public int YPixelsPerMeter;
        public int ColorsUsed;
        public int ColorsImportant;
    }

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr windowHandle);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr windowHandle, IntPtr deviceContext);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleDC(IntPtr deviceContext);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteDC(IntPtr deviceContext);

    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern IntPtr CreateDIBSection(IntPtr deviceContext, ref BitmapInfoHeader bitmapInfo, uint usage,
                                                 out IntPtr pixelData, IntPtr sectionHandle, uint offset);

    [DllImport("gdi32.dll")]
    public static extern IntPtr SelectObject(IntPtr deviceContext, IntPtr gdiObject);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteObject(IntPtr gdiObject);

    [DllImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool BitBlt(IntPtr destination, int destX, int destY, int width, int height,
                                     IntPtr source, int sourceX, int sourceY, uint rasterOperation);
}
