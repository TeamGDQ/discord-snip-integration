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
using DiscordSharp;
using DiscordSharp.Objects;

namespace DiscordSnipIntegration
{
    using static Global;

    internal class Program
    {
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

        private static Locale locale;
        private static string musicDir;
        private static bool quitTriggered;
        private static Settings settings;
        private static Thread threadWorker;
        private Process _snipProc;
        private string _token;
        private DiscordMember _me;
        private DiscordClient _client;
        public static Locale Locale => locale;
        internal static Settings Settings => settings;
        private static string currentSong => $"{musicDir}\\{SNIPTXT}";
        private bool _connected;

        [STAThread]
        public static void Main ( string [ ] args )
        {
            Console.Title = AppFull;
            LoadCritical ( );
            DisplayHeader ( );
            RunUpdates ( );
            threadWorker = new Thread ( new ThreadStart ( new Program ( ).Initiate ) );
            threadWorker.Start ( );
            while ( !quitTriggered )
            {
            }
        }

        private static void DisplayHeader ( )
        {
            Console.WriteLine ( AppFull );
            Console.WriteLine ( AppCopy );

            PrintWarningByCurrentRepo ( );

            Console.WriteLine ( );
        }

        private static void LoadCritical ( )
        {
            settings = Settings.Load ( );
            locale = Locale.Load ( settings.PreferredLocale );

            if ( Locale.localeGenerated )
            {
                settings.PreferredLocale = locale.LocaleName;
                settings.AutoSave ( );
            }

            if ( locale == null )
            {
                throw new NullReferenceException ( Locale.LocaleFail );
            }

            if ( !settings.AcceptEula )
            {
                if ( ( new Eula ( ) ).ShowDialog ( ) == System.Windows.Forms.DialogResult.OK )
                {
                    settings.AcceptEula = true;
                    settings.AutoSave ( );
                }
            }
        }

        private static void RunUpdates ( )
        {
            using ( TcpClient tc = new TcpClient ( ) )
            {
                string newVer = string.Empty;
                using ( var w = new WebClient ( ) )
                {
                    w.CachePolicy = new System.Net.Cache.RequestCachePolicy ( System.Net.Cache.RequestCacheLevel.NoCacheNoStore );
                    newVer = w.DownloadString ( $"{PROJECTURL}/LATEST" );
                }

                ToasterVersion nv = ToasterVersion.Parse ( newVer );

                if ( nv > version )
                {
                    Console.WriteLine ( locale.UpdateAvailableString );
                    Process.Start ( $"{PROJECTURL}/{nv.Filename}" );
                    Console.WriteLine ( locale.PressAnyKeyString );
                    Console.ReadKey ( true );
                }
            }
        }
        
        private void Initiate ( )
        {
                Console.CursorVisible = false;
            try
            {
                // INIT PLUGIN

                InitSnip ( );

                _token = DiscordTokenHelper.GetToken ( );

                _client = new DiscordClient ( _token );
                _client.Connected += ( s, e ) =>
                {
                    _me = _client.Me;
                    _connected = true;
                    _client.UpdateCurrentGame ( "DSI Initiated" );
                    
                };

                _client.PresenceUpdated += ( s, e ) =>
                {
                    Console.WriteLine ($"Username: {e.User.Username}");
                    Console.WriteLine ($"Game: {e.Game}");
                    Console.WriteLine ($"Status: {e.Status}");
                    Thread.Sleep ( 3000 );
                };

                _client.SendLoginRequest ( );
                ( new Thread ( new ThreadStart( _client.Connect ) ) ).Start ( );
                
                while (!_connected)
                {
                    Console.Write ( '.' );
                    Thread.Sleep ( SLEEP );
                }

                while ( _connected )
                {
                    string title = CurrentSong;
                    
                    // Console.Clear ( ); Change the way the Console interacts, or just create a stupid GUI....
                    Console.WriteLine ( $"{locale.ConnectedAsString} {_me.Username}" );
                    Console.WriteLine ( $"ID: {_me.ID}" );
                    // Console.WriteLine ( $"Current Server: {_me.Parent.Name}" );
                    Console.WriteLine ( $"{locale.CurrentGameStatusString}: {( _me.CurrentGame )} " );
                    Console.WriteLine ( $"{locale.CurrentUserStatusString}: {_me.Status}" );
                    Thread.Sleep ( REDRAW );

                }
                threadWorker.Abort ( );
            }
            catch ( Exception ex )
            {
                Console.WriteLine ( ex.Message );
            }
            Console.CursorVisible = true;
            Console.Write ( locale.PressAnyKeyString );
            Console.ReadKey ( );
            quitTriggered = true;
            if ( _snipProc != null )
                _snipProc.Kill ( );
        }

        private void InitSnip ( )
        {
            _snipProc = Process.GetProcessesByName ( SNIP ).FirstOrDefault ( );
            if ( _snipProc == null )
            {
                Task.Run ( new Action ( ( ) => Winter.SpotifyNowPlaying.SnipInit ( ) ) );
                musicDir = StartupPath;
                return;
            }
            musicDir = Path.GetDirectoryName ( _snipProc.MainModule.FileName );
        }

        private string ReadFile ( string path )
        {
            string res = string.Empty;
            using ( StreamReader reader = new StreamReader ( path ) )
            {
                res = reader.ReadToEnd ( );
            }
            return res;
        }

        private void SetTrack (string name )
        {
            _client.UpdateCurrentGame ( name );
        }
    }
}