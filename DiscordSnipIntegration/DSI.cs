using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;

namespace DiscordSnipIntegration
{
    internal class DeusX {

        private object _lockState;
        private string _token;
        private Profile _me;
        private DiscordClient _client;
        private FileSystemWatcher _fsx;
        
        private bool _connected;
        private bool _announcedGame;
        private bool _bleedInProgress;
        private bool _externalSnip;

        private readonly GlassBar _win;

        private const string GameName = "$Game";
        private const string SongName = "$Song";
        private const string StreamName = "$Stream";
        private const string GameWithSong = "$Game while listening to $Song";
        private const string StreamWithSong = "$Stream while listening to $Song";

        private string _lastGame;
        private Thread _fswatchThread;
        private Thread _dsiThread;

        public DeusX (GlassBar frame)
        {
            _win = frame;
            Task.Run ( new Action ( Initiate ) );
        }

        private static bool InitSnip( ) => Task.Run( new Func< bool >( Winter.SpotifyNowPlaying.SnipInit ) ).Result;
        public bool IsConnected( ) => _client?.State == ConnectionState.Connected;

        private void SetTrack ( string gameName, string songName = "", bool listening = false, bool playing = false, bool streaming = false, string url = "" )
        {
            _lastGame = gameName;
            if ( !_announcedGame ) {
                Trace.__verbose( $"Current Game: {gameName}" );
                _announcedGame = true;
            }
            // Three different approaches, Define these in Locale?
            // Streaming $Stream while listening to $Song
            // Playing $Game while listening to $Song
            // Playing $Song

            // TODO: Go through this fucking block
            string x;
            if ( string.IsNullOrEmpty ( gameName ) || gameName == Global.LastSong )
                gameName = "";
            // possible crash?
            int len = gameName.IndexOf ( gameName.Replace ( GameName, "" ), StringComparison.Ordinal ) + 1;
            len = len <= 1 ? gameName.Length : len;

            string g = gameName.Substring ( 0, len );

            if ( streaming )
            {
                x = listening ? StreamWithSong.Replace ( StreamName, url ).Replace ( SongName, songName ) : StreamName.Replace ( StreamName, url );
            }
            else if ( playing )
            {
                x = listening ? GameWithSong.Replace ( GameName, gameName ).Replace ( SongName, songName ) : GameName.Replace ( GameName, gameName );
            }
            else
            {
                x = listening ? SongName.Replace ( SongName, songName ) : GameName.Replace ( GameName, gameName );
            }

            var game = new Game ( x, streaming ? GameType.Twitch : GameType.Default, url );

            if ( _lastGame == game.Name ) return;
            _announcedGame = false;
            Trace.Info ( $"Setting Game Status to {x}\r\n\t[g]: {g}" );
            if ( _connected )
                _client.SetGame ( game );
            _win.SetNowPlaying ( x );
            _lastGame = game.Name;
        }

        private void Initiate ( )
        {
            try
            {
                // INIT PLUGIN

                if(!InitSnip ( )) {
                    Process p =
                        Process
                            .GetProcesses(  )
                            .FirstOrDefault(x => x.ProcessName.Equals( $"{Global.SNIP}", StringComparison.InvariantCulture ) );
                    if ( p != null ) {
                        // We got the running process
                        Trace.PrintLine( "CAPTURE", $"We got the Process! {p.ProcessName}" );
                    }
                    Trace.__verbose("Creating new FileSystemWatcher()");
                    _fswatchThread = new Thread( ( ) => {
                        Trace.__verbose( "Thread Started(FileSystemWatcher());" );
                        _fsx = new FileSystemWatcher( Program.Settings.SnipLocation ) {
                            NotifyFilter = NotifyFilters.LastWrite,
                            Filter = "*.txt"
                        };
                        _fsx.Changed += FsxOnChanged;
                        _fsx.Error += FsxOnError;
                        _fsx.EnableRaisingEvents = true;
                        while ( _fswatchThread.IsAlive ) {
                            Thread.Sleep( 1 );
                            // Loop Forever until the thread dies
                        }
                    } );
                    _fswatchThread.Start( );
                    _externalSnip = true;
                }

                _token = DiscordTokenHelper.GetToken ( );
                _lockState = _token.Clone( );

                _client = new DiscordClient ();
                _client.Ready += ( s, e ) =>
                {
                    _me = _client.CurrentUser;
                    Trace.Info( $"State: {_client.State}" );
                    while ( _client.State != ConnectionState.Connected ) {
                        Trace.Write(".");
                        Thread.Sleep(Global.REDRAW);
                    }
                    _connected = _client.State == ConnectionState.Connected;
                    
                    _win.SetStatus ( GlassConnectionStatus.Online );
                    Trace.Success ( "We are connected" );
                    _fsx.EndInit( );

                    _dsiThread = new Thread( Run );
                    _dsiThread.Start( );
                };

                _client.MessageReceived += ( s, e ) => {
                    // Trace.PrintLine( $"MESSAGE [{e.Server.Name}::{e.Channel.Name}->{e.Message.User.Name}]",
                    //     $"{e.Message.Text}" );
                };
                
                var t = new Thread ( Connect );
                t.Start ( );
                
                // while(t.IsAlive)
                // {
                    _win.SetStatus ( GlassConnectionStatus.Connecting );
                    SetTrack ( Locale.LoadedLocale.ConnectingString );
                    Thread.Sleep ( 500 );
                // }
            }
            catch ( Exception ex )
            {
                Trace.Error ( ex.ToString ( ) );
            }
        }

        private void FsxOnError( object sender, ErrorEventArgs e ) {
            Trace.Error( e.GetException( ).Message );
        }

        private void FsxOnChanged( object sender, FileSystemEventArgs fsea ) {
            if ( fsea.ChangeType != WatcherChangeTypes.Changed ) return;
            string fch = fsea.Name;
            if ( fch != Global.SNIPTXT ) return;

            Trace.PrintLine("FILE_CHANGED", $"{fsea.FullPath}");
            try {
                string data = File.ReadAllText( fsea.FullPath );
                Global.LastSong = data;
            } catch (Exception) {
                Thread.Sleep( 100 );
                FsxOnChanged(sender, fsea);
            }
        }

        private void Run ( ) {
            Action bleedBeat = ( ) => {
                if ( _bleedInProgress ) return;
                lock ( _lockState ) {
                    Trace.Warning( "Bleed is in Progress... Sending a Heart Beat" );
                    _bleedInProgress = true;
                    _client.GatewaySocket?.SendHeartbeat( );
                    Thread.Sleep(10000);
                    _bleedInProgress = false;
                    Trace.Success("Bleed Complete");
                }

            };

            // Trace.Warning ( $"Are we connected? {( _connected ? "We are successfully connected, yet We couldn't rely on our own bool, yet the library's" : "Nope, We are not connected" )}" );
            while ( _connected )
            {
                string title = !_externalSnip ? Global.CurrentSong : Global.LastSong;

                // Console.Clear ( ); Change the way the Console interacts, or just create a stupid GUI....
                // Console.WriteLine ( $"{Program.Locale.ConnectedAsString} {_me.Username}" );
                _win.SetUser ( _me.Name );
                try
                {
                    Game g = _me.CurrentGame ?? new Game ( );
                    
                    SetTrack (
                        g.Name,
                        title,
                        !string.IsNullOrEmpty ( title ),
                        !string.IsNullOrEmpty ( g.Name ) && !g.Name.Contains ( title ),
                        g.Type == GameType.Twitch,
                        g.Url
                        );
                }
                catch ( Exception ex ) {
                    Trace.Error( "BOOBOO" );
                    Trace.Error ( ex.Message );
                }

                // Should we put the Heartbeat method here?
                // But that means every second we send a beat out, when we should do this every 10 seconds
                // Task.Run( bleedBeat );
                
                Thread.Sleep ( 1000 );
            }
            _win.SetNowPlaying ( Locale.LoadedLocale.AuthFailString );
            _win.SetStatus ( GlassConnectionStatus.Offline );
        }

        private void Connect ( )
        {
            try {
                _client.ExecuteAndWait( async ( ) => await _client.Connect( _token, TokenType.User ) );
            } catch (Exception ex) {
                Trace.Error( ex.ToString() );
                #if DEBUG
                Trace.ReadKey( );
                #endif
            }
            
        }
    }
}
