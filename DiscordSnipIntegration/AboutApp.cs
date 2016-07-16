using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordSnipIntegration
{
    public static class About
    {
        #region Assembly Attribute Accessors

        public static string Title
        {
            get
            {
                object [ ] attributes = Assembly.GetExecutingAssembly ( ).GetCustomAttributes ( typeof ( AssemblyTitleAttribute ), false );
                if ( attributes.Length > 0 )
                {
                    AssemblyTitleAttribute titleAttribute = ( AssemblyTitleAttribute ) attributes [ 0 ];
                    if ( titleAttribute.Title != "" )
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension ( Assembly.GetExecutingAssembly ( ).CodeBase );
            }
        }

        public static string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly ( ).GetName ( ).Version.ToString ( );
            }
        }

        public static string Description
        {
            get
            {
                object [ ] attributes = Assembly.GetExecutingAssembly ( ).GetCustomAttributes ( typeof ( AssemblyDescriptionAttribute ), false );
                if ( attributes.Length == 0 )
                {
                    return "";
                }
                return ( ( AssemblyDescriptionAttribute ) attributes [ 0 ] ).Description;
            }
        }

        public static string Product
        {
            get
            {
                object [ ] attributes = Assembly.GetExecutingAssembly ( ).GetCustomAttributes ( typeof ( AssemblyProductAttribute ), false );
                if ( attributes.Length == 0 )
                {
                    return "";
                }
                return ( ( AssemblyProductAttribute ) attributes [ 0 ] ).Product;
            }
        }

        public static string Copyright
        {
            get
            {
                object [ ] attributes = Assembly.GetExecutingAssembly ( ).GetCustomAttributes ( typeof ( AssemblyCopyrightAttribute ), false );
                if ( attributes.Length == 0 )
                {
                    return "";
                }
                return ( ( AssemblyCopyrightAttribute ) attributes [ 0 ] ).Copyright;
            }
        }

        public static string Company
        {
            get
            {
                object [ ] attributes = Assembly.GetExecutingAssembly ( ).GetCustomAttributes ( typeof ( AssemblyCompanyAttribute ), false );
                if ( attributes.Length == 0 )
                {
                    return "";
                }
                return ( ( AssemblyCompanyAttribute ) attributes [ 0 ] ).Company;
            }
        }
        #endregion
    }
}
