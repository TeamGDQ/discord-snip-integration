
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
using System.IO;

using System.Reflection;
using Newtonsoft.Json;

namespace DiscordSnipIntegration
{
    public class Locale
    {
        public const string LocaleFail = "Unable to load given locale. Please find a compatible locale from the installation.\r\nIf you do not see your language, send an email to the developer of this program at shadowcompany@blizzeta.net";


        public static readonly string LocalePath = $"{Path.GetDirectoryName ( Assembly.GetExecutingAssembly ( ).Location )}\\.locale";

        // Default Language
        public static readonly string usLocale = $"{LocalePath}\\en-US.locale";

        private static bool lg = false;

        private string ln;


        [JsonProperty ( PropertyName = "Language", Required = Required.Always )]
        public string LanguageString { get; private set; }

        [JsonProperty ( PropertyName = "Application Description", Required = Required.Always )]
        public string ApplicationDescriptionString { get; private set; }

        [JsonProperty ( PropertyName = "Authentication Failure", Required = Required.DisallowNull )]
        public string AuthFailString { get; private set; }

        [JsonProperty ( PropertyName = "Cannot find Snip", Required = Required.DisallowNull )]
        public string CannotFindSnipString { get; private set; }

        [JsonProperty ( PropertyName = "You are connected as", Required = Required.DisallowNull )]
        public string ConnectedAsString { get; private set; }

        [JsonProperty ( PropertyName = "Current State", Required = Required.DisallowNull )]
        public string CurrentStateString { get; private set; }
        
        [JsonProperty ( PropertyName = "Current User Status", Required = Required.DisallowNull )]
        public string CurrentUserStatusString { get; private set; }

        [JsonProperty ( PropertyName = "Connected", Required = Required.DisallowNull )]
        public string ConnectedString { get; private set; }

        [JsonProperty ( PropertyName = "Connecting", Required = Required.DisallowNull )]
        public string ConnectingString { get; private set; }

        [JsonProperty ( PropertyName = "DiscordConnectedEvent", Required = Required.DisallowNull )]
        public string DiscordConnectedString { get; private set; }

        [JsonProperty ( PropertyName = "New Message From", Required = Required.DisallowNull )]
        public string NewMessageFromString { get; private set; }

        [JsonProperty ( PropertyName = "Now Playing", Required = Required.DisallowNull )]
        public string NowPlayingString { get; private set; }

        [JsonProperty ( PropertyName = "Now Streaming", Required = Required.DisallowNull )]
        public string NowStreamingString { get; private set; }
        
        [JsonProperty ( PropertyName = "Press any key to quit", Required = Required.DisallowNull )]
        public string PressAnyKeyString { get; private set; }

        [JsonProperty ( PropertyName = "Update Available", Required = Required.DisallowNull )]
        public string UpdateAvailableString { get; private set; }

        [JsonProperty ( PropertyName = "Development Stage", Required = Required.DisallowNull )]
        public string DevelString { get; private set; }

        [JsonProperty ( PropertyName = "Scaring Stage", Required = Required.DisallowNull )]
        public string ScaringDevString { get; private set; }

        [JsonProperty ( PropertyName = "Missing License", Required = Required.Always )]
        public string MissingLicense { get; private set; }

        // Streaming $Stream while listening to $Song *
        // Playing $Game while listening to $Song *
        // Streaming $Stream *
        // Playing $Game *
        // Playing $Song *

        [JsonProperty ( PropertyName = "Playing Game", Required = Required.DisallowNull )]
        public string CurrentGameStatusString { get; private set; }

        [JsonProperty ( PropertyName = "Playing Game While Listening", Required = Required.DisallowNull )]
        public string CurrentGameWhileListeningString { get; private set; }

        [JsonProperty ( PropertyName = "Streaming While Listening", Required = Required.DisallowNull )]
        public string CurrentStreamWhileListeningString { get; private set; }

        [JsonProperty ( PropertyName = "Streaming", Required = Required.DisallowNull )]
        public string CurrentStreamStatusString { get; private set; }

        [JsonProperty ( PropertyName = "Playing Song", Required = Required.DisallowNull )]
        public string CurrentSongStatusString { get; private set; }


        internal static bool localeGenerated => lg;

        private static Locale ll;
        private static Locale dl;

        [JsonIgnore ( )]
        public string LocaleName => ln;

        [JsonIgnore ( )]
        public static Locale LoadedLocale => ll;

        [JsonIgnore ( )]
        public static Locale EnUSLocale => dl;

        internal static void CreateGenericLocale ( )
        {
            Locale l = new Locale ( );
            l.LanguageString = "English (United States)";
            l.ApplicationDescriptionString = "Detects Music coming from Snip (Snip Required). This Application supports whatever Snip supports";
            l.AuthFailString = "Failed to authenticate";
            l.CannotFindSnipString = "We cannot find Snip...";
            l.ConnectedAsString = "Connected as";
            l.ConnectedString = "Successfully connected to Guild";
            l.ConnectingString = "Connecting to Guild";

            // These elements need to have the first word chopped off when literally updating current game status.
            // Better yet, We need to load the en-US locale when updating the game.
            l.CurrentGameStatusString = "Playing $Game";
            l.CurrentGameWhileListeningString = "Playing $Game while listening to $Song";
            l.CurrentSongStatusString = "Playing $Song";
            l.CurrentStreamStatusString = "Streaming $Stream";
            l.CurrentStreamWhileListeningString = "Streaming $Stream while listening to $Song";

            l.CurrentStateString = "Current State";
            l.CurrentUserStatusString = "Current User Status";
            l.DiscordConnectedString = "You have successfully connected to Discord";
            l.NewMessageFromString = "New message from";
            l.NowPlayingString = "Now Playing";
            l.NowStreamingString = "Now Streaming";
            l.PressAnyKeyString = "Press any key to quit";
            l.UpdateAvailableString = "Update Available";
            l.DevelString = "This program is in its development stages, Whether it's Alpha, Beta, or Development, Please expect a lot of bugs. Thank you.";
            l.ScaringDevString = "This program is in its development stages, however, this version is deemed more stable. Expect far less bugs.";
            l.MissingLicense = "License is missing! Please obtain a license from https://www.gnu.org/licenses/gpl-3.0.txt, and put it in a File called License.txt in the Application's Directory.";
            File.WriteAllText ( usLocale, JsonConvert.SerializeObject ( l, Formatting.Indented ) );
            l = null;
            lg = true;
        }

        internal static void LoadLocales (string preferredlocale)
        {
            ll = Load ( preferredlocale );
        }

        internal static Locale Load ( string preferredlocale )
        {
            Locale l = null;
            if ( !Directory.Exists ( LocalePath ) )
            {
                Directory.CreateDirectory ( LocalePath );
            }

            if ( string.IsNullOrEmpty ( preferredlocale ) )
            {
                CreateGenericLocale ( );
                return Load ( Path.GetFileNameWithoutExtension ( usLocale ) );
            }

            string file = $"{LocalePath}\\{preferredlocale}.locale";
            if ( !File.Exists ( file ) )
            {
                throw new FileNotFoundException ( "Locale does not exist!", file );
            }

            StreamReader reader = new StreamReader ( file );
            l = JsonConvert.DeserializeObject<Locale> ( reader.ReadToEnd ( ) );

            l.ln = preferredlocale;

            return l;
        }

        // More strings?
    }
}