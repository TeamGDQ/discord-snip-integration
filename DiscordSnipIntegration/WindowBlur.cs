using System;
using System.Collections.Generic;
using Drawing = System.Drawing;
using Drawing2D = System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace DiscordSnipIntegration.WindowHelper
{
    public enum WindowSide
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 3,
        TopLeft = 4,
        TopRight = 5,
        Bottom = 6,
        BottomLeft = 7,
        BottomRight = 8,
    }

    public struct DWMCOLORIZATIONPARAMS
    {
        public uint ColorizationColor,
            ColorizationAfterglow,
            ColorizationColorBalance,
            ColorizationAfterglowBalance,
            ColorizationBlurBalance,
            ColorizationGlassReflectionIntensity,
            ColorizationOpaqueBlend;
    }

    [Flags]
    public enum CompositionAction : uint
    {
        /// <summary>
        /// To enable DWM composition
        /// </summary>
        DWM_EC_DISABLECOMPOSITION = 0,
        /// <summary>
        /// To disable composition.
        /// </summary>
        DWM_EC_ENABLECOMPOSITION = 1
    }

    [StructLayout ( LayoutKind.Sequential )]
    public struct Win32Point
    {
        public int X;
        public int Y;
    };

    [StructLayout ( LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4 )]
    public class MONITORINFOEX
    {
        public int cbSize = Marshal.SizeOf ( typeof ( MONITORINFOEX ) );
        public RECT rcMonitor = new RECT ( );
        public RECT rcWork = new RECT ( );
        public int dwFlags = 0;
        [MarshalAs ( UnmanagedType.ByValArray, SizeConst = 32 )]
        public char [ ] szDevice = new char [ 32 ];
    }

    [StructLayout ( LayoutKind.Sequential )]
    public struct POINTSTRUCT
    {
        public int x;
        public int y;
        public POINTSTRUCT ( int x, int y )
        {
            this.x = x;
            this.y = y;
        }
    }

    [StructLayout ( LayoutKind.Sequential )]
    public struct RECT
    {
        public double Left, Top, Right, Bottom;

        public RECT ( int left, int top, int right, int bottom )
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT ( double left, double top, double right, double bottom )
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public double X
        {
            get { return Left; }
            set { Right -= ( Left - value ); Left = value; }
        }

        public double Y
        {
            get { return Top; }
            set { Bottom -= ( Top - value ); Top = value; }
        }

        public double Height
        {
            get { return Bottom - Top; }
            set { Bottom = value + Top; }
        }

        public double Width
        {
            get { return Bottom - Top; }
            set { Bottom = value + Top; }
        }

        public Point Location
        {
            get { return new Point ( Left, Top ); }
            set { X = value.X; Y = value.Y; }
        }

        public Size Size
        {
            get { return new Size ( Width, Height ); }
            set { Width = value.Width; Height = value.Height; }
        }

        public static bool operator == ( RECT r1, RECT r2 )
        {
            return r1.Equals ( r2 );
        }

        public static bool operator != ( RECT r1, RECT r2 )
        {
            return !r1.Equals ( r2 );
        }

        public bool Equals ( RECT r )
        {
            return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
        }

        public override bool Equals ( object obj )
        {
            if ( obj is RECT )
                return Equals ( ( RECT ) obj );
            return false;
        }

        public override int GetHashCode ( )
        {
            return base.GetHashCode ( );
        }

        public override string ToString ( )
        {
            return string.Format ( System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", ( int ) Left, ( int ) Top, ( int ) Right, ( int ) Bottom );
        }
    }


    [StructLayout ( LayoutKind.Sequential )]
    public struct MARGINS
    {
        public int leftWidth;
        public int rightWidth;
        public int topHeight;
        public int bottomHeight;

        public MARGINS ( int left, int top, int bottom, int right )
        {
            leftWidth = left;
            rightWidth = right;
            topHeight = top;
            bottomHeight = bottom;
        }

        public MARGINS ( int margin )
        {
            leftWidth = margin;
            rightWidth = margin;
            topHeight = margin;
            bottomHeight = margin;
        }
    }

    [StructLayout ( LayoutKind.Sequential )]
    public struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    /// <summary>
    ///     The weird thing is that WCA_ACCENT_POLICY was presumed to be 19, but it's 10, according to http://www.brandonfa.lk/win8/win8_devrel_head_x86/uxtheme.h
    ///     But the value is supposed to be 19, which is WCA_CLOAKED......
    /// </summary>
    public enum WindowCompositionAttribute
    {
        // WCA_UNDEFINED = 0,
        // WCA_NCRENDERING_ENABLED = 1,
        // WCA_NCRENDERING_POLICY = 2,
        // WCA_TRANSITIONS_FORCEDISABLED = 3,
        // WCA_ALLOW_NCPAINT = 4,
        // WCA_CAPTION_BUTTON_BOUNDS = 5,
        // WCA_NONCLIENT_RTL_LAYOUT = 6,
        // WCA_FORCE_ICONIC_REPRESENTATION = 7,
        // WCA_FLIP3D_POLICY = 8,
        // WCA_EXTENDED_FRAME_BOUNDS = 9,
        // WCA_HAS_ICONIC_BITMAP = 10,
        // WCA_THEME_ATTRIBUTES = 11,
        // WCA_NCRENDERING_EXILED = 12,
        // WCA_NCADORNMENTINFO = 13,
        // WCA_EXCLUDED_FROM_LIVEPREVIEW = 14,
        // WCA_VIDEO_OVERLAY_ACTIVE = 15,
        // WCA_FORCE_ACTIVEWINDOW_APPEARANCE = 16,
        // WCA_DISALLOW_PEEK = 17,
        // WCA_CLOAK = 18,
        // WCA_CLOAKED = 19,
        // WCA_ACCENT_POLICY = 20,
        // WCA_ACCENT_TRANSFORM = 21,
        // WCA_LAST = 22,

        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }

    [Flags]
    public enum AccentFlags
    {
        // ...
        DrawLeftBorder = 0x20,
        DrawTopBorder = 0x40,
        DrawRightBorder = 0x80,
        DrawBottomBorder = 0x100,
        DrawAllBorders = ( DrawLeftBorder | DrawTopBorder | DrawRightBorder | DrawBottomBorder )
        // ...
    }

    public enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [StructLayout ( LayoutKind.Sequential )]
    public struct AccentPolicy
    {
        public AccentState AccentState;
        public AccentFlags AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout ( LayoutKind.Sequential )]
    public struct SIZE
    {
        public int cx;
        public int cy;
        public SIZE ( int cx, int cy )
        {
            this.cx = cx;
            this.cy = cy;
        }
    }

    [StructLayout ( LayoutKind.Sequential )]
    public struct PROPPAGEINFO
    {
        public uint cb;
        public IntPtr pszTitle;
        public SIZE size;
        public IntPtr pszDocString;
        public IntPtr pszHelpFile;
        public uint dwHelpContext;

        public static PROPPAGEINFO CreateInstance ( string title, string docString, string helpfile )
        {
            PROPPAGEINFO ppi = new PROPPAGEINFO ( );
            ppi.pszTitle = Marshal.StringToCoTaskMemUni ( title );
            ppi.pszDocString = Marshal.StringToCoTaskMemUni ( docString );
            ppi.pszHelpFile = Marshal.StringToCoTaskMemUni ( helpfile );
            ppi.cb = ( uint ) Marshal.SizeOf ( ppi );
            return ppi;
        }
    }

    public delegate bool EnumPropsDelegate ( IntPtr hWnd, string lpString, IntPtr data );

    public static class WindowBlurExtensions
    {
        [DllImport ( "user32.dll" )]
        public static extern int SetWindowCompositionAttribute ( IntPtr hwnd, ref WindowCompositionAttributeData data );

        [DllImport ( "user32.dll" )]
        public static extern int EnumProps ( IntPtr hWnd, EnumPropsDelegate lpEnumFunc );

        [DllImport ( "dwmapi.dll", EntryPoint = "#127" )]
        public static extern void DwmGetColorizationParameters ( ref DWMCOLORIZATIONPARAMS dwmColorizationParams );

        [DllImport ( "kernel32.dll" )]
        private static extern void AllocConsole ( );

        [DllImport ( "kernel32.dll" )]
        private static extern void FreeConsole ( );

        [DllImport ( "kernel32.dll", CharSet = CharSet.Auto )]
        static extern int lstrlen ( string lpString );

        [DllImport ( "kernel32.dll", CharSet = CharSet.Auto )]
        static extern IntPtr lstrcpy ( [Out] StringBuilder lpString1, string lpString2 );

        [DllImport ( "user32.dll" )]
        static extern int SetWindowRgn ( IntPtr hWnd, IntPtr hRgn, bool bRedraw );

        public const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x320;

        public static void ShowWindowProperties ( this Window window )
        {
            var windowHelper = new WindowInteropHelper ( window );
            var hWnd = windowHelper.Handle;
            AllocConsole ( );
            EnumProps ( hWnd, new EnumPropsDelegate ( ( h, lpstr, data ) =>
            {
                StringBuilder sb = new StringBuilder ( lstrlen ( lpstr ) );
                string fgt = Marshal.PtrToStringAnsi ( data );
                lstrcpy ( sb, fgt );

                Console.WriteLine ( $"Property: {lpstr}; Data: {sb.ToString ( )}" );
                return true;
            } ) );
            Console.WriteLine ( "Press any key to Free the console..." );
            Console.ReadLine ( );
            FreeConsole ( );
        }

        public static Color GetWindowColorizationColor ( bool opaque )
        {
            DWMCOLORIZATIONPARAMS pms = new DWMCOLORIZATIONPARAMS ( );

            DwmGetColorizationParameters ( ref pms );

            return Color.FromArgb (
                ( byte ) ( opaque ? 255 : pms.ColorizationColor >> 24 ),
        ( byte ) ( pms.ColorizationColor >> 16 ),
        ( byte ) ( pms.ColorizationColor >> 8 ),
        ( byte ) pms.ColorizationColor
    );
        }

        public static void EnableBlur ( this Window window )
        {
            if ( SystemParameters.HighContrast )
            {
                return; // Blur is not useful in high contrast mode
            }

            SetAccentPolicy ( window, AccentState.ACCENT_ENABLE_BLURBEHIND );
        }

        public static void DisableBlur ( this Window window )
        {
            SetAccentPolicy ( window, AccentState.ACCENT_DISABLED );
        }


        private static void SetAccentPolicy ( Window window, AccentState accentState )
        {
            var windowHelper = new WindowInteropHelper ( window );
            var hwnd = windowHelper.Handle;

            var hwndSource = HwndSource.FromHwnd ( hwnd );

            window.Background = Brushes.Transparent;
            hwndSource.CompositionTarget.BackgroundColor = System.Windows.Media.Colors.Transparent;

            //var sizeFactor = hwndSource.CompositionTarget.TransformToDevice.Transform ( new Vector ( 1.0, 1.0 ) );

            //using ( var path = new Drawing2D.GraphicsPath ( ) )
            //{
            //path.AddEllipse ( 0, 0, ( int ) ( window.ActualWidth * sizeFactor.X ), ( int ) ( window.ActualHeight * sizeFactor.Y ) );


            //var newRgn = new Drawing.Region ( );
            //using ( var graphics = Drawing.Graphics.FromHwnd ( hwnd ) )
            //{
            //var nRgn = newRgn.GetHrgn ( graphics );
            //SetWindowRgn ( hwnd, nRgn, true );

            var accent = new AccentPolicy ( );
            accent.AccentState = accentState;
            accent.AccentFlags = 0;

            var accentStructSize = Marshal.SizeOf ( accent );

            var accentPtr = Marshal.AllocHGlobal ( accentStructSize );
            Marshal.StructureToPtr ( accent, accentPtr, false );

            var data = new WindowCompositionAttributeData ( );
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute ( windowHelper.Handle, ref data );

            Marshal.FreeHGlobal ( accentPtr );
            //        newRgn.ReleaseHrgn ( nRgn );
            //    }
            //}
        }
    }

    public class WindowBlurHelper
    {
        [DllImport ( "user32.dll" )]
        internal static extern int SetWindowCompositionAttribute ( IntPtr hwnd, ref WindowCompositionAttributeData data );

        [StructLayout ( LayoutKind.Sequential )]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout ( LayoutKind.Sequential )]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        private IntPtr hWnd;
        public IntPtr Handle => hWnd;

        public WindowBlurHelper ( IntPtr hWnd )
        {
            this.hWnd = hWnd;
        }

        internal void EnableBlur ( )
        {
            var accent = new AccentPolicy ( );
            var accentStructSize = Marshal.SizeOf ( accent );
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentPtr = Marshal.AllocHGlobal ( accentStructSize );
            Marshal.StructureToPtr ( accent, accentPtr, false );

            var data = new WindowCompositionAttributeData ( );
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute ( hWnd, ref data );

            Marshal.FreeHGlobal ( accentPtr );
        }
    }

}