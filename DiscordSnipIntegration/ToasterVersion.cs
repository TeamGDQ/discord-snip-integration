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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordSnipIntegration
{
    public enum Repo
    {
        Development = 0x01,
        Alpha = 0x02,
        Snapshot = 0x04,
        Scaring = 0x08,
        Beta = 0x10,
        Release = 0x20,
    }

    public struct ToasterVersion
    {
        [JsonProperty(PropertyName ="Major", Required = Required.Always)]
        public int Major { get; private set; }

        [JsonProperty ( PropertyName = "Minor", Required = Required.Always )]
        public int Minor { get; private set; }

        [JsonProperty ( PropertyName = "Build", Required = Required.Always )]
        public int Build { get; private set; }

        [JsonProperty ( PropertyName = "Revision", Required = Required.Always )]
        public int Revision { get; private set; }

        [JsonProperty ( PropertyName = "Repository", Required = Required.Always )]
        public Repo Repo { get; private set; }
        
        [JsonProperty(PropertyName = "Update Package", Required = Required.AllowNull)]
        public string Filename { get; private set; }

        public ToasterVersion(int major, int minor, int build, int revision, Repo repo)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
            Repo = repo;
            Filename = null;
        }
 
        public static ToasterVersion Parse ( string tver )
        {
            try
            {
                return JsonConvert.DeserializeObject<ToasterVersion> ( tver );
            }
            catch ( JsonException )
            {
                return new ToasterVersion ( );
            }
        }

        public string CreateUpdateString()
        {
            return JsonConvert.SerializeObject ( this, Formatting.None );
        }

        public static bool operator > ( ToasterVersion a, ToasterVersion b )
        {
            if ( a.version ( ) > b.version ( ) || a.Repo > b.Repo )
            {
                return true;
            }
            return false;
        }

        public static bool operator < ( ToasterVersion a, ToasterVersion b )
        {
            if ( a.version ( ) < b.version ( )  || a.Repo < b.Repo)
            {
                return true;
            }
            return false;
        }

        public static bool operator >= (ToasterVersion a, ToasterVersion b)
        {
            if ( a.version ( ) >= b.version ( )  || a.Repo >= b.Repo)
            {
                return true;
            }
            return false;
        }

        public static bool operator <= (ToasterVersion a, ToasterVersion b)
        {
            if(a.version() <= b.version() ||  a.Repo <= b.Repo)
            {
                return true;
            }
            return false;
        }

        public static bool operator == ( ToasterVersion a, ToasterVersion b )
        {
            if ( a.version ( ) == b.version ( ) && a.Repo == b.Repo)
            {
                return true;
            }
            return false;
        }

        public static bool operator != ( ToasterVersion a, ToasterVersion b )
        {
            if ( a.version ( ) != b.version ( ) && a.Repo != b.Repo )
            {
                return true;
            }
            return false;
        }

        private Version version ()
        {
            return new Version ( Major, Minor, Build, Revision );
        }

        public override bool Equals ( object obj )
        {
            return base.Equals ( obj );
        }

        public override int GetHashCode ( )
        {
            return base.GetHashCode ( );
        }

        public override string ToString ( )
        {
            return $"{Major}.{Minor}.{Build}{( Revision > 0 ? $".{Revision}" : "" )} ({Enum.GetName ( typeof ( Repo ), Repo )})";
        }
    }
}
