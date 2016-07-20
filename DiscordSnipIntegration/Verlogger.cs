using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DiscordSnipIntegration
{
    internal class NativeMethods
    {
        public const int FOREGROUND_BLUE = 0x01;
        public const int FOREGROUND_GREEN = 0x02;
        public const int FOREGROUND_INTENSITY = 0x08;
        public const int FOREGROUND_RED = 0x04;
        public const int MAX = FOREGROUND_INTENSITY | FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE;

        public const int STD_OUTPUT_HANDLE = -11;

        public enum ShowWindowCommands
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0,

            /// <summary>
            /// Activates and displays a window. If the window is minimized or
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window
            /// for the first time.
            /// </summary>
            Normal = 1,

            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,

            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3, // is this the right value?

                          /// <summary>
                          /// Activates the window and displays it as a maximized window.
                          /// </summary>
            ShowMaximized = 3,

            /// <summary>
            /// Displays a window in its most recent size and position. This value
            /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except
            /// the window is not activated.
            /// </summary>
            ShowNoActivate = 4,

            /// <summary>
            /// Activates the window and displays it in its current size and position.
            /// </summary>
            Show = 5,

            /// <summary>
            /// Minimizes the specified window and activates the next top-level
            /// window in the Z order.
            /// </summary>
            Minimize = 6,

            /// <summary>
            /// Displays the window as a minimized window. This value is similar to
            /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the
            /// window is not activated.
            /// </summary>
            ShowMinNoActive = 7,

            /// <summary>
            /// Displays the window in its current size and position. This value is
            /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the
            /// window is not activated.
            /// </summary>
            ShowNA = 8,

            /// <summary>
            /// Activates and displays the window. If the window is minimized or
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9,

            /// <summary>
            /// Sets the show state based on the SW_* value specified in the
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.
            /// </summary>
            ShowDefault = 10,

            /// <summary>
            ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
            /// that owns the window is not responding. This flag should only be
            /// used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }

        public static IntPtr ConsoleHandle { get; private set; }

        [Conditional ( "DEBUG" ), DllImport ( "Kernel32.dll" )]
        public static extern void AllocConsole ( );

        [Conditional ( "DEBUG" ), DllImport ( "Kernel32.dll" )]
        public static extern void FreeConsole ( );

        [DllImport ( "kernel32.dll", SetLastError = true )]
        public static extern bool GetConsoleScreenBufferInfoEx (
           IntPtr hConsoleOutput,
           ref CONSOLE_SCREEN_BUFFER_INFOEX ConsoleScreenBufferInfo
           );

        [DllImport ( "user32.dll" )] // import lockwindow to remove flashing
        public static extern bool LockWindowUpdate ( IntPtr hWndLock );

        [DllImport ( "user32.dll", CharSet = CharSet.Unicode )]
        public static extern int MessageBox ( IntPtr hWnd, string caption, string title, int flags );

        public static void SetConsoleHandle ( )
        {
            ConsoleHandle = GetStdHandle ( STD_OUTPUT_HANDLE );
        }

        [DllImport ( "kernel32.dll", SetLastError = true )]
        public static extern bool SetConsoleScreenBufferInfoEx (
           IntPtr ConsoleOutput,
           ref CONSOLE_SCREEN_BUFFER_INFOEX ConsoleScreenBufferInfoEx
           );

        [DllImport ( "kernel32.dll" )]
        public static extern bool SetConsoleTextAttribute ( IntPtr hConsoleOutput,
            ushort wAttributes );

        [DllImport ( "user32.dll" )]
        public static extern bool ShowWindowAsync ( IntPtr hWnd, ShowWindowCommands nCmdShow );

        [DllImport ( "kernel32.dll", SetLastError = true )]
        private static extern IntPtr GetStdHandle ( int nStdHandle );

        [StructLayout ( LayoutKind.Sequential )]
        public struct COLORREF
        {
            public uint ColorDWORD;

            public COLORREF ( Color color )
            {
                ColorDWORD = color.R + ( ( ( uint ) color.G ) << 8 ) + ( ( ( uint ) color.B ) << 16 );
            }

            public Color GetColor ( )
            {
                return Color.FromArgb ( ( int ) ( 0x000000FFU & ColorDWORD ),
                   ( int ) ( 0x0000FF00U & ColorDWORD ) >> 8, ( int ) ( 0x00FF0000U & ColorDWORD ) >> 16 );
            }

            public void SetColor ( Color color )
            {
                ColorDWORD = color.R + ( ( ( uint ) color.G ) << 8 ) + ( ( ( uint ) color.B ) << 16 );
            }
        }

        [StructLayout ( LayoutKind.Sequential )]
        public struct CONSOLE_SCREEN_BUFFER_INFOEX
        {
            public uint cbSize;
            public COORD dwSize;
            public COORD dwCursorPosition;
            public short wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;

            public ushort wPopupAttributes;
            public bool bFullscreenSupported;

            [MarshalAs ( UnmanagedType.ByValArray, SizeConst = 16 )]
            public COLORREF [ ] ColorTable;

            public static CONSOLE_SCREEN_BUFFER_INFOEX Create ( )
            {
                return new CONSOLE_SCREEN_BUFFER_INFOEX { cbSize = 96 };
            }
        }

        [StructLayout ( LayoutKind.Sequential )]
        public struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout ( LayoutKind.Sequential )]
        public struct RGB
        {
            private byte byRed, byGreen, byBlue, RESERVED;

            public RGB ( Color colorIn )
            {
                byRed = colorIn.R;
                byGreen = colorIn.G;
                byBlue = colorIn.B;
                RESERVED = 0;
            }

            public RGB ( byte R, byte G, byte B )
            {
                byRed = R;
                byGreen = G;
                byBlue = B;
                RESERVED = 0;
            }

            public static implicit operator Color ( RGB rgb )
            {
                return Color.FromArgb ( rgb.byRed, rgb.byGreen, rgb.byBlue );
            }

            public int ToInt32 ( )
            {
                byte [ ] RGBCOLORS = new byte [ 4 ];
                RGBCOLORS [ 0 ] = byRed;
                RGBCOLORS [ 1 ] = byGreen;
                RGBCOLORS [ 2 ] = byBlue;
                RGBCOLORS [ 3 ] = RESERVED;
                return BitConverter.ToInt32 ( RGBCOLORS, 0 );
            }
        }

        public struct SMALL_RECT
        {
            public short Bottom;
            public short Left;
            public short Right;
            public short Top;
        }
    }

    internal class Trace
    {
        [Conditional ( "DEBUG" )]
        public static void AllocConsole ( )
        {
            NativeMethods.AllocConsole ( );
        }

        [Conditional ( "DEBUG" )]
        public static void Error ( string msg )
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteLine ( $"[ERROR: {DateTime.Now:HH:mm:ss}] {msg}" );
            Console.ResetColor ( );
        }

        [Conditional ( "DEBUG" )]
        public static void FreeConsole ( )
        {
            NativeMethods.FreeConsole ( );
        }

        [Conditional ( "DEBUG" )]
        public static void Info ( string msg )
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteLine ( $"[INFO: {DateTime.Now:HH:mm:ss}] {msg}" );
            Console.ResetColor ( );
        }

        [Conditional ( "DEBUG" )]
        public static void Maximize ( )
        {
            NativeMethods.ShowWindowAsync ( NativeMethods.ConsoleHandle, NativeMethods.ShowWindowCommands.Maximize );
        }

        public static ConsoleKeyInfo ReadKey ( )
        {
            return ReadKey ( false );
        }
        public static ConsoleKeyInfo ReadKey ( bool intercept )
        {
#if DEBUG
            return Console.ReadKey ( intercept );
#else
            return new ConsoleKeyInfo( );
#endif
        }

        [Conditional ( "DEBUG" )]
        public static void SetBiggie ( )
        {
            Console.WindowWidth += 50;
            Console.WindowHeight += 30;
        }

        [Conditional ( "DEBUG" )]
        public static void SetTitle ( string title )
        {
            Console.Title = title;
        }

        [Conditional ( "DEBUG" )]
        public static void Success ( string msg )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteLine ( $"[SUCCESS: {DateTime.Now:HH:mm:ss}] {msg}" );
            Console.ResetColor ( );
        }

        [Conditional ( "DEBUG" )]
        public static void Warning ( string msg )
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteLine ( $"[WARNING: {DateTime.Now:HH:mm:ss}] {msg}" );
            Console.ResetColor ( );
        }

        [Conditional ( "DEBUG" )]
        public static void Write ( string msg )
        {
            Console.Write ( msg );
        }

        [Conditional ( "DEBUG" )]
        public static void WriteLine ( string msg )
        {
            Console.WriteLine ( msg );
        }

        public static void WriteLine ( )
        {
            WriteLine ( "" );
        }

        [Conditional ( "DEBUG" )]
        internal static void __verbose ( string msg )
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteLine ( $"[VERBOSE: {DateTime.Now:HH:mm:ss}] {msg}" );
            Console.ResetColor ( );
        }
    }
}