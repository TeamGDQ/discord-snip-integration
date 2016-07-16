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
using Discord;

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

        private static Locale locale;
        private static string musicDir;
        private static bool quitTriggered;
        private static Settings settings;
        private static Thread threadWorker;
        private string _currentHash;
        private Process _snipProc;
        private string _token;
        private Profile _user;
        private DiscordClient client;
        public static Locale Locale => locale;
        internal static Settings Settings => settings;
        private static string currentSong => $"{musicDir}\\{SNIPTXT}";

        [STAThread]
        public static void Main ( string [ ] args )
        {
            Console.Title = "";
            LoadCritical ( );
            DisplayHeader ( );
            RunUpdates ( );
            Console.Title = AppFull;
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

        private void CheckTrack ( )
        {
            string newHash = GetFileHashString ( currentSong );
            if ( _currentHash != newHash )
            {
                _currentHash = newHash;
                SetTrack ( );
            }
        }

        private void Client_Ready ( object sender, EventArgs e )
        {
            Console.WriteLine ( "Ready" );
        }

        private void Initiate ( )
        {
            try
            {
                // INIT PLUGIN

                InitSnip ( );

                _token = DiscordTokenHelper.GetToken ( );

                client = new DiscordClient ( );

                client.Ready += Client_Ready;

                client.Connect ( _token );

                while ( client.State == ConnectionState.Connecting )
                {
                    Console.Write ( '.' );
                    Thread.Sleep ( SLEEP );
                }

                while ( client.State == ConnectionState.Connected )
                {
                    try
                    {
                        Console.Clear ( );
                        if ( _user == null || _user != client.CurrentUser )
                            _user = client.CurrentUser;
                        Game g = _user.CurrentGame ?? new Game ( "No Game", GameType.Default, null );
                        Console.WriteLine ( $"{locale.ConnectedAsString} {_user.Name}" );
                        Console.WriteLine ( $"ID: {_user.Id}" );
                        Console.WriteLine ( $"{locale.CurrentStateString}: {client.State}" );
                        Console.WriteLine ( $"{locale.CurrentGameStatusString}: {( g.Name )} " );
                        Console.WriteLine ( $"{locale.CurrentUserStatusString}: {_user.Status.Value}" );

                        CheckTrack ( );
                        Thread.Sleep ( REDRAW );
                    }
                    catch ( System.Runtime.Serialization.SerializationException )
                    {
                        /** Invalid JSON String **/
                    }
                }
                threadWorker.Abort ( );
            }
            catch ( Exception ex )
            {
                Console.WriteLine ( ex );

                Console.WriteLine ( ex.Message );
            }
            Console.Write ( locale.PressAnyKeyString );
            Console.ReadKey ( );
            quitTriggered = true;
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

        private void SetTrack ( )
        {
            _user.Client.SetGame ( ReadFile ( currentSong ) );
            _user.Client.Servers.All ( ( x ) =>
            {
                x.Client.SetGame ( _user.Client.CurrentGame );

                return true;
            } );
        }
    }
}