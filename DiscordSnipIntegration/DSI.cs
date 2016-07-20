﻿using System;
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
    class DeusX
    {
        private string _token;
        private Profile _me;
        private DiscordClient _client;
        
        private bool _connected;
        private GlassBar _win;

        private const string GameName = "$Game";
        private const string SongName = "$Song";
        private const string StreamName = "$Stream";
        private const string GameWithSong = "$Game while listening to $Song";
        private const string StreamWithSong = "$Stream while listening to $Song";

        private string _lastGame;
        
        public DeusX (GlassBar frame)
        {
            _win = frame;
            Task.Run ( new Action ( Initiate ) );
        }

        private void InitSnip ( )
        {
            Task.Run ( new Action ( ( ) => Winter.SpotifyNowPlaying.SnipInit ( ) ) );
        }
        
        private void SetTrack ( string gameName, string songName = "", bool listening = false, bool playing = false, bool streaming = false, string url = "" )
        {
            _lastGame = gameName;
            Trace.__verbose ( $"Current Game: {gameName}" );
            // Three different approaches, Define these in Locale?
            // Streaming $Stream while listening to $Song
            // Playing $Game while listening to $Song
            // Playing $Song

            string x = string.Empty;
            if ( string.IsNullOrEmpty ( gameName ) || gameName == Global.LastSong )
                gameName = "";
            // possible crash?
            int len = gameName.IndexOf ( gameName.Replace ( GameName, "" ) ) + 1;
            len = len <= 1 ? gameName.Length : len;

            string g = gameName.Substring ( 0, len );

            if ( streaming )
            {
                if ( listening )
                {
                    x = StreamWithSong.Replace ( StreamName, url ).Replace ( SongName, songName );
                }
                else
                {
                    x = StreamName.Replace ( StreamName, url );
                }
            }
            else if ( playing )
            {
                if ( listening )
                {
                    x = GameWithSong.Replace ( GameName, gameName ).Replace ( SongName, songName );
                }
                else
                {
                    x = GameName.Replace ( GameName, gameName );
                }
            }
            else
            {
                if ( listening )
                {
                    x = SongName.Replace ( SongName, songName );
                }
                else
                {
                    x = GameName.Replace ( GameName, gameName );
                }
            }

            Game game = new Game ( x, streaming ? GameType.Twitch : GameType.Default, url );


            if ( _lastGame != game.Name )
            {
                Trace.Info ( $"Setting Game Status to {x}" );
                if ( _client.State == ConnectionState.Connected )
                    _client.SetGame ( game );
                _win.SetNowPlaying ( x );
                _lastGame = game.Name;
            }
        }

        private void Initiate ( )
        {
            try
            {
                // INIT PLUGIN

                InitSnip ( );

                _token = DiscordTokenHelper.GetToken ( );

                _client = new DiscordClient ();
                _client.Ready += ( s, e ) =>
                {
                    _me = _client.CurrentUser;
                    _connected = true;
                    _win.SetStatus ( GlassConnectionStatus.Online );
                    Trace.Success ( "We are connected" );
                    Run ( );
                };
                
                
                Thread t = ( new Thread ( new ThreadStart ( Connect ) ) );
                t.Start ( );
                
                while(t.IsAlive)
                {
                    _win.SetStatus ( GlassConnectionStatus.Connecting );
                    SetTrack ( Locale.LoadedLocale.ConnectingString );
                    Thread.Sleep ( 500 );
                }
            }
            catch ( Exception ex )
            {
                Trace.Error ( ex.ToString ( ) );
            }
        }

        private void Run ( )
        {
            Trace.Warning ( $"Are we connected? {( _connected ? "We are successfully connected, yet We couldn't rely on our own bool, yet the library's" : "Nope, We are not connected" )}" );
            while ( _connected )
            {
                string title = Global.CurrentSong;

                // Console.Clear ( ); Change the way the Console interacts, or just create a stupid GUI....
                // Console.WriteLine ( $"{Program.Locale.ConnectedAsString} {_me.Username}" );
                _win.SetUser ( _me.Name );
                try
                {
                    Game g = ( _me.CurrentGame != null && _me.CurrentGame.HasValue ) ? _me.CurrentGame.Value : new Game ( );
                    
                    SetTrack (
                        g.Name,
                        title,
                        !string.IsNullOrEmpty ( title ),
                        !string.IsNullOrEmpty ( g.Name ) && !g.Name.Contains ( title ),
                        g.Type == GameType.Twitch ? true : false,
                        g.Url
                        );
                }
                catch ( Exception ex )
                {
                    Trace.Error ( ex.Message );
                }
                Thread.Sleep ( 1000 );
            }
            _win.SetNowPlaying ( Locale.LoadedLocale.AuthFailString );
            _win.SetStatus ( GlassConnectionStatus.Offline );
        }

        private void Connect ( )
        {
            _client.Connect ( _token );
        }
    }
}
