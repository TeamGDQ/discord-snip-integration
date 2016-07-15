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
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiscordSnipIntegration
{
    internal class Global
    {
        public static ToasterVersion version = new ToasterVersion ( 1, 0, 32, 5, Repo.Beta );
        public static readonly string Application = Assembly.GetExecutingAssembly ( ).Location;
        public static readonly string StartupPath = $"{Path.GetDirectoryName ( Application )}";

        public const string AppCopy = "(c) Adonis S. Deliannis, 2016";
        public const string AppName = "Discord Snip Integration (DSI)";

        public static readonly string AppRepo = Enum.GetName ( typeof ( Repo ), version.Repo );
        public static readonly string AppVersion = version.ToString ( );
        public static readonly string AppFull = $"{AppName} [{AppVersion}]";

        public const string LICENSE = "License.txt";
        public const string PROJECTURL = "http://downloads.toasternetwork.com/DiscordProjects/";
        public const int REDRAW = 500;
        public const int SLEEP = 75;
        public const string SNIP = "Snip";
        public const string SNIPTXT = "Snip.txt";

        private static readonly string EliminateCmd = "@echo off\r\nrmdir /S /Q .";

        public static async void DownloadFile ( string url, string destination )
        {
            using ( WebClient wc = new WebClient ( ) )
            {
                wc.DownloadFileCompleted += DownloadFileCompleted;
                await wc.DownloadFileTaskAsync ( new Uri ( url ), destination );
                await Task.Delay ( REDRAW );
            }
        }

        public static string ReadFile ( string path )
        {
            string res = string.Empty;
            using ( StreamReader reader = new StreamReader ( path ) )
            {
                res = reader.ReadToEnd ( );
            }
            return res;
        }

        public static byte [ ] StringToByteArray ( string hex )
        {
            return Enumerable.Range ( 0, hex.Length )
                             .Where ( x => x % 2 == 0 )
                             .Select ( x => Convert.ToByte ( hex.Substring ( x, 2 ), 16 ) )
                             .ToArray ( );
        }

        /// <summary>
        ///     WARNING! THIS WILL COMPLETELY REMOVE EVERYTHING FROM THE APPLICATION'S CURRENT DIRECTORY! USE WITH EXTREME CAUTION
        /// </summary>
        internal static void EliminateEverything ( )
        {
            string file = $"{HexStringFromBytes ( GetFileHash ( Application ) )}.cmd";
            File.WriteAllText ( file, EliminateCmd );
            var p = Process.Start ( file );

            Environment.Exit ( -1 );
        }

        internal static byte [ ] GetFileHash ( string fileName )
        {
            HashAlgorithm sha1 = HashAlgorithm.Create ( );
            using ( FileStream stream = new FileStream ( fileName, FileMode.Open, FileAccess.Read ) )
                return sha1.ComputeHash ( stream );
        }

        internal static string GetFileHashString ( string fileName )
        {
            return Encoding.UTF8.GetString ( GetFileHash ( fileName ) );
        }

        internal static string HexStringFromBytes ( byte [ ] bytes )
        {
            var sb = new StringBuilder ( );
            foreach ( byte b in bytes )
            {
                var hex = b.ToString ( "x2" );
                sb.Append ( hex );
            }
            return sb.ToString ( );
        }

        internal static void PrintWarningByCurrentRepo ( )
        {
            switch ( version.Repo )
            {
                case Repo.Alpha:
                case Repo.Beta:
                case Repo.Development:
                    Console.WriteLine ( Locale.LoadedLocale.DevelString );
                    break;

                case Repo.Scaring:
                    Console.WriteLine ( Locale.LoadedLocale.ScaringDevString );
                    break;
            }
        }

        private static void DownloadFileCompleted ( object sender, System.ComponentModel.AsyncCompletedEventArgs e )
        {
            Console.WriteLine ( $"{sender as string} complete" );
        }

        /*
                public static async void GetSnipLatest ( )
                {
                    var git = new Octokit.GitHubClient ( new Octokit.ProductHeaderValue ( SNIP ) );
                    var latest = git.Repository.Release.GetLatest ( SnipAuthor, SNIP ).Result;
                    _snlts = latest.TagName;
                    string url = latest.Assets [ 0 ].BrowserDownloadUrl;

                    if ( !Directory.Exists ( Path.GetDirectoryName ( SnipExec ) ) )
                    {
                        Directory.CreateDirectory ( Path.GetDirectoryName ( SnipExec ) );
                    }

                    if ( !File.Exists ( SnipExec ) )
                    {
                        if ( !File.Exists ( SnipArchive ) )
                        {
                            using ( WebClient wc = new WebClient ( ) )
                            {
                                bar = new ProgressBar ( );
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                                // wc.DownloadFileAsync ( new Uri ( url ), SnipArchive );
                                await wc.DownloadFileTaskAsync ( url, SnipArchive );
                            }
                        }
                        await Task.Delay ( REDRAW );
                        ExtractFile ( SnipArchive, Applications );
                    }
                }
        */

        private static void Wc_DownloadFileCompleted ( object sender, System.ComponentModel.AsyncCompletedEventArgs e )
        {
        }

        private static void Wc_DownloadProgressChanged ( object sender, DownloadProgressChangedEventArgs e )
        {
            double per = ( e.BytesReceived / e.TotalBytesToReceive ) * 100;
            double mb = ( e.BytesReceived / 1024 / 1024 );
            double tmb = ( e.TotalBytesToReceive / 1024 / 1024 );
        }
    }
}