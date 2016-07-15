
﻿/**
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
    public class Settings
    {
        public static readonly string SettingsPath = $"{Path.GetDirectoryName ( Assembly.GetExecutingAssembly ( ).Location )}";
        public static readonly string SettingsFile = $"{SettingsPath}\\Settings.conf";

        [JsonProperty ( PropertyName = "Accept Eula" )]
        public bool AcceptEula { get; internal set; }

        [JsonProperty ( PropertyName = "Autosave" )]
        public bool Autosave { get; internal set; }

        [JsonProperty ( PropertyName = "Preferred Locale" )]
        public string PreferredLocale { get; internal set; }


        public static Settings Load ( )
        {
            if ( !File.Exists ( SettingsFile ) )
            {
                ( new Settings
                {
                    AcceptEula = false,
                    Autosave = true

                } ).Save ( );
            }
            StreamReader reader = new StreamReader ( SettingsFile );
            Settings s = JsonConvert.DeserializeObject<Settings> ( reader.ReadToEnd ( ) );
            reader.Close ( );
            reader = null;
            return s;
        }

        public void AutoSave ( )
        {
            if ( Autosave )
            {
                Save ( );
            }
        }

        public void Save ( )
        {
            File.WriteAllText ( SettingsFile, JsonConvert.SerializeObject ( this, Formatting.Indented ) );
        }
    }
}