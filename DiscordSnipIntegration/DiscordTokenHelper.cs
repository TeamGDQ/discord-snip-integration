using System;

using System.Data.SQLite;

using System.IO;



namespace DiscordSnipIntegration
{
    public class DiscordTokenHelper
    {

        private const string Key = "key";
        private const string Token = "token";
        private const string Value = "value";
        private static readonly string AppData = Environment.GetFolderPath ( Environment.SpecialFolder.ApplicationData );
        private static readonly string Command = $"SELECT {Key}, {Value} FROM ItemTable";
        private static readonly string DiscordLocalStoragePath = Path.Combine ( AppData, "discord", "Local Storage" );
        private static readonly string LocalStorageFile = "https_discordapp.com_0.localstorage";
        private static readonly string Database = Path.Combine ( DiscordLocalStoragePath, LocalStorageFile );
        private static readonly string Statement = $"Data Source={Database};Version=3;";
        private static readonly char [ ] Trim = { '\"' };

        public static string GetToken ( )
        {
            using ( var conn = new SQLiteConnection ( Statement ) )
            {

                using ( var cmd = conn.CreateCommand ( ) )
                {
                    conn.Open ( );

                    cmd.CommandText = Command;


                    using ( var reader = cmd.ExecuteReader ( ) )
                    {
                        while ( reader.Read ( ) )
                        {

                            string key = reader.GetString ( reader.GetOrdinal ( Key ) );
                            string value = reader.GetString ( reader.GetOrdinal ( Value ) );
                            if ( key == Token )
                            {
                                return value.Trim ( Trim );
                                
                            }
                        }
                    }
                }

            }
            return null;

        }
    }
}