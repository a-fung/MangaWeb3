using System;

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

        public static int ElementsPerPage = 5;
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
