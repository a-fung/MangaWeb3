using System;
using System.Html;
using jQueryApi;

namespace afung.MangaWeb3.Client
{
    /// <summary>
    /// Contain environment variables
    /// </summary>
    public class Environment
    {
        /// <summary>
        /// The server type.
        /// Should be set from servertype.js
        /// </summary>
        public static ServerType ServerType;

        public static int ElementsPerPage = 20;

        public static int MangaListItemPerRow = 4;

        public static int ModalElementsPerPage = 7;

        public static int MangaPagesToPreload = 1;

        public static int MangaPagesUnloadDistance = 3;

        public static double PixelRatio
        {
            get
            {
                if (Number.IsFinite(ExtendedWindow.DevicePixelRatio))
                {
                    return ExtendedWindow.DevicePixelRatio;
                }

                return 1.0;
            }
        }

        public static bool IsiOS
        {
            get
            {
                return Window.Navigator.UserAgent.IndexOf("iPhone") != -1 || Window.Navigator.UserAgent.IndexOf("iPad") != -1 || Window.Navigator.UserAgent.IndexOf("iPod") != -1;
            }
        }


        public static bool IsKindle
        {
            get
            {
                return Window.Navigator.UserAgent.IndexOf("Kindle") != -1;
            }
        }

        public static bool IsIE9OrLower
        {
            get
            {
                int index = Window.Navigator.UserAgent.IndexOf("MSIE");
                if (index == -1)
                {
                    return false;
                }

                string versionString = Window.Navigator.UserAgent.Substr(index + 4);
                int version = int.Parse(versionString, 10);

                if (version > 9)
                {
                    return false;
                }
                else if (version == 7)
                {
                    // compatibility mode?
                    index = Window.Navigator.UserAgent.IndexOf("Trident/");
                    versionString = Window.Navigator.UserAgent.Substr(index + 8);
                    version = int.Parse(versionString, 10);

                    return version <= 5;
                }

                return true;
            }
        }
    }

    /// <summary>
    /// Server Type enum
    /// </summary>
    public enum ServerType
    {
        /// <summary>
        /// The server is running ASP.NET
        /// </summary>
        AspNet = 0,

        /// <summary>
        /// The server is running PHP
        /// </summary>
        Php = 1
    }
}
