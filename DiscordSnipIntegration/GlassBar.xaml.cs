using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using BreakerDev.DwmApi;
using BreakerDev.WPF_Interop;

namespace DiscordSnipIntegration
{
    using WindowHelper;

    public enum GlassConnectionStatus
    {
        Offline = 0,
        Connecting,
        Online,
        Idle
    }

    internal delegate void TUpdateHandler <T> ( T obj );

    /// <summary>
    /// Interaction logic for GlassBar.xaml
    /// </summary>
    public partial class GlassBar : Window
    {
        private readonly Thickness _bottom = new Thickness ( -1, -18, -1, -1 );

        private readonly RadialGradientBrush _chromeStroke = new RadialGradientBrush ( new GradientStopCollection {
            new GradientStop(Colors.Black, 0.85),
            new GradientStop(Color.FromArgb(0xFF, 0xC5, 0xC5, 0xC5), 1)
        } );

        private readonly Thickness _floating = new Thickness ( 0, -18, 0, 0 );

        private readonly RadialGradientBrush _idle = new RadialGradientBrush ( new GradientStopCollection {
            new GradientStop(Color.FromArgb(0x7F, 0x00, 0x00, 0x00), 1),
            new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xDC, 0x00), 0.15)
        } );

        private readonly RadialGradientBrush _offline = new RadialGradientBrush ( new GradientStopCollection {
            new GradientStop(Color.FromArgb(0x7F, 0x00, 0x00, 0x00), 1),
            new GradientStop(Colors.Red, 0.15)
        } );

        private readonly RadialGradientBrush _online = new RadialGradientBrush ( new GradientStopCollection {
            new GradientStop(Color.FromArgb(0x7F, 0x00, 0x00, 0x00), 1),
            new GradientStop(Color.FromArgb(0xFF, 0x47, 0xD8, 0x30), 0.15)
        } );

        private readonly RadialGradientBrush _connecting = new RadialGradientBrush ( new GradientStopCollection {
            new GradientStop(Color.FromArgb(0x7F, 0x00, 0x00, 0x00), 1),
            new GradientStop(Color.FromArgb(0xFF, 0xDE, 0xA2, 0x00), 0.15)
        } );


        private readonly Thickness _top = new Thickness ( -1, -25, -1, 0 );

        private DeusX _deux;

        private ABEdge _edge;
        private GlassConnectionStatus _lastStatus;

        private int resistance = 4;

        public GlassBar ( )
        {
            InitializeComponent ( );
            _edge = ABEdge.Top;
            connectionStatus.Stroke = _chromeStroke; 
            SetStatus ( GlassConnectionStatus.Offline );
        }

        internal void SetStatus ( GlassConnectionStatus gcs )
        {
            if(!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke ( new TUpdateHandler<GlassConnectionStatus> ( SetStatus ), gcs );
                return;
            }
            if ( gcs == _lastStatus )
                return;
            System.Diagnostics.Debug.WriteLine ( $"Setting Connection status to {gcs}", "Connection Status" );
            RadialGradientBrush rgb = null;
            switch ( gcs )
            {
                    
                case GlassConnectionStatus.Connecting:
                    rgb = _connecting;
                    break;
                case GlassConnectionStatus.Online:
                    rgb = _online;
                    break;
                case GlassConnectionStatus.Idle:
                    rgb = _idle;
                    break;
                case GlassConnectionStatus.Offline:
                default:
                    rgb = _offline;
                    break;
            }
            
            connectionStatus.Fill = rgb;
            _lastStatus = gcs;
        }

        [DllImport ( "dwmapi.dll" )]
        private static extern void DwmEnableBlurBehindWindow ( IntPtr hwnd, ref Dwm.DWM_BLURBEHIND blurBehind );

        [DllImport ( "dwmapi.dll" )]
        private static extern int DwmExtendFrameIntoClientArea ( IntPtr hwnd, ref Dwm.MARGINS margins );

        /**
            private Thickness _top      = new Thickness(-1,-25,-1,0);  Set on Top
            private Thickness _bottom   = new Thickness(-1,-24,-1,-1); Set on Bottom
            private Thickness _floating = new Thickness(0,-24,0,0);    Floating

        */
        private void accent_MouseDown ( object sender, MouseButtonEventArgs e )
        {
            if ( Program.Settings.BarPosition == BarPosition.Floating )
            {
                try
                {
                    DragMove ( );
                }
                catch ( Exception ) // Only when we "try" to move the window...
                {
                    // This is stupidly a null idea
                }
            }
        }

        private void closeBtn_Click ( object sender, RoutedEventArgs e )
        {
            Environment.Exit ( 0 );
        }

        private void dockBtn_Click ( object sender, RoutedEventArgs e )
        {
            // MessageBox.Show ( "Please edit your decision of Floating, Top, or Bottom, for \"Bar Position\"" );
            int scPos = ( int ) ( ( WpfScreen.GetScreenFrom ( this ).WorkingArea.Height / 2 ) );
            switch ( Program.Settings.BarPosition )
            {
                case BarPosition.Floating:
                    if ( Top < scPos )
                        Program.Settings.BarPosition = BarPosition.Top;
                    else
                        Program.Settings.BarPosition = BarPosition.Bottom;
                    break;

                case BarPosition.Top:
                case BarPosition.Bottom:
                default:
                    Program.Settings.BarPosition = BarPosition.Floating;
                    break;
            }
            SetWindowParams ( );
        }

        private void SetWindowParams ( )
        {
            // WPF.AppBarFunctions.SetAppBar ( this, WPF.ABEdge.None );
            // double o = Height;
            // Height = 30;
            switch ( Program.Settings.BarPosition )
            {
                case BarPosition.Floating:
                    _edge = ABEdge.None;
                    accent.Margin = _floating;
                    ShowInTaskbar = true;
                    break;
                case BarPosition.Bottom:
                    _edge = ABEdge.Bottom;
                    accent.Margin = _bottom;
                    ShowInTaskbar = false;
                    break;
                case BarPosition.Top:
                default:
                    _edge = ABEdge.Top;
                    accent.Margin = _top;
                    ShowInTaskbar = false;
                    break;
            }
            AppBarFunctions.SetAppBar ( this, _edge );
            Program.Settings.AutoSave ( );
            // Thread.Sleep ( 300 );
            // Height = o;
        }

        internal void SetUser(string username)
        {
            if ( !Dispatcher.CheckAccess ( ) )
            {
                Dispatcher.Invoke ( new TUpdateHandler<string> ( SetUser ), username );
                return;
            }
            if(this.username.Text != username)
                this.username.Text = username;
        }

        internal void SetNowPlaying(string nowPlaying)
        {
            if ( !Dispatcher.CheckAccess ( ) )
            {
                Dispatcher.Invoke ( new TUpdateHandler<string> ( SetNowPlaying ), nowPlaying );
                return;
            }
            if(songPlaying.Text != nowPlaying)
                songPlaying.Text = nowPlaying;
        }

        private void SwitchState ( )
        {
            switch ( WindowState )
            {
                case WindowState.Normal:
                    {
                        WindowState = WindowState.Maximized;
                        break;
                    }
                case WindowState.Maximized:
                    {
                        WindowState = WindowState.Normal;
                        break;
                    }
            }
        }

        private void SysEventUserPrefsChanged ( object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e )
        {
            if ( e.Category == Microsoft.Win32.UserPreferenceCategory.General )
            {
                uint bkg = 0;
                bool opaqueBlend;
                Dwm.DwmGetColorizationColor ( out bkg, out opaqueBlend );
                accent.BorderBrush = Unpack ( bkg );
            }
        }

        private Brush Unpack ( uint bkg )
        {
            byte a, r, g, b;
            a = ( byte ) ( bkg >> 24 );
            r = ( byte ) ( bkg >> 16 );
            g = ( byte ) ( bkg >> 8 );
            b = ( byte ) ( bkg );

            return new SolidColorBrush ( Color.FromArgb ( a, r, g, b ) );
        }

        private void Window_Loaded ( object sender, RoutedEventArgs e )
        {
            _deux = new DeusX ( this );
            SetWindowParams ( );
            Microsoft.Win32.SystemEvents.UserPreferenceChanged += SysEventUserPrefsChanged;
            IntPtr hWnd = new WindowInteropHelper ( this ).Handle;
            HwndSource src = HwndSource.FromHwnd ( hWnd );
            uint bkg = 0;
            bool opaqueBlend;
            Dwm.DwmGetColorizationColor ( out bkg, out opaqueBlend );
            accent.BorderBrush = Unpack ( bkg );

            WindowBlurHelper wbh = new WindowBlurHelper ( hWnd );

            Dwm.MARGINS pMargins = new Dwm.MARGINS ( -1, -1, -1, -1 );
            src.CompositionTarget.BackgroundColor = Color.FromArgb ( 0, 0, 0, 0 );

            bool isComposite = false;
            Dwm.DwmIsCompositionEnabled ( out isComposite );

            if ( !isComposite )
                Dwm.DwmEnableComposition ( true );

            DwmExtendFrameIntoClientArea ( hWnd, ref pMargins );
            wbh.EnableBlur ( );

            // DwmEnableBlurBehindWindow ( hWnd, ref dbb );
        }
    }

    internal static class Extensions
    {
        public static byte [ ] ToByteArray ( this System.Drawing.Image imageIn )
        {
            MemoryStream ms = new MemoryStream ( );
            imageIn.Save ( ms, imageIn.RawFormat );
            return ms.ToArray ( );
        }
    }
}