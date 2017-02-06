/**
   Copyright (C) 2016 Adonis S. Deliannis

   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordSnipIntegration {
    using static Global;

    internal static class Program {
        /**
            Originally used into getting auth tokens,
            but the client would still get kicked off,
            which is why I made the TokenHelper.

            GET /?code=token HTTP/1.1 regex
            https://regex101.com/r/qZ6kL3/1
        */

        /*
            Anything in [] is not from my code
            Anything in () is critical
            Implement {
                [Streaming StreamName] while listening to music
                [Playing Game] while listening to music
                (The reason why I will not remove the snip.txt is\
                    because we need it for OBS or whatever streaming program you have\
                    but it would be easier to continue to read the current stream in memory\
                    than to keep polling every redraw cycle.)
            }
        */

        private static Settings settings;
        private static GlassBar bar;
        internal static Settings Settings => settings;

        [STAThread]
        public static void Main( string[] args ) {
            Trace.AllocConsole( );

            Trace.SetTitle( AppFull );
            LoadCritical( );
            DisplayHeader( );
            RunUpdates( );

            Trace.Info( "Launching new GlassBar()" );
            bar = new GlassBar( );
            bar.ShowDialog( );
            Trace.FreeConsole( );
        }

        private static void DisplayHeader( ) {
            Trace.WriteLine( AppFull );
            Trace.WriteLine( AppCopy );

            PrintWarningByCurrentRepo( );

            Trace.WriteLine( );
        }

        private static void LoadCritical( ) {
            settings = Settings.Load( );
            Locale.LoadLocales( settings.PreferredLocale );

            if ( Locale.localeGenerated ) {
                settings.PreferredLocale = Locale.LoadedLocale.LocaleName;
                settings.AutoSave( );
            }

            if ( Locale.LoadedLocale == null ) {
                throw new NullReferenceException( Locale.LocaleFail );
            }

            if ( settings.AcceptEula ) return;
            if ( new Eula( ).ShowDialog( ) != System.Windows.Forms.DialogResult.OK ) return;
            settings.AcceptEula = true;
            settings.AutoSave( );
        }

        private static void RunUpdates( ) {
            using ( var tc = new TcpClient( ) ) {
                try {
                    string newVer;
                    string pth;
                    string dPth;
                    using ( var w = new WebClient( ) ) {
                        w.CachePolicy =
                            new System.Net.Cache.RequestCachePolicy( System.Net.Cache.RequestCacheLevel.NoCacheNoStore );
                        try {
                            pth = PROJECTURL;
                            dPth = $"{pth}/LATEST";
                            Trace.__verbose( $"File: {dPth}" );
                            newVer = w.DownloadString( dPth );

                        } catch (Exception) {
                            pth = PROJECTURL_ALT;
                            dPth = $"{pth}/LATEST";
                            Trace.__verbose ( $"File: {dPth}" );
                            newVer = w.DownloadString( dPth );
                        }
                    }

                    ToasterVersion nv = ToasterVersion.Parse( newVer );

                    if ( nv <= Global.Version ) return;
                    Trace.WriteLine( Locale.LoadedLocale.UpdateAvailableString );
                    Trace.PrintLine("FILE_TO_DOWNLOAD", $"{pth}/{nv.Filename}");
                    Process.Start( $"{pth}/{nv.Filename}" );
                    Trace.WriteLine( Locale.LoadedLocale.PressAnyKeyString );
                    Trace.ReadKey( true );
                }
                catch ( Exception ex ) {
                    Trace.Error( ex.Message );
                }
            }
        }
    }
}