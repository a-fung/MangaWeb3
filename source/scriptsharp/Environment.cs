using System;

namespace afung.MangaWeb3.Client
{
    public class Environment
    {
        public static ServerType ServerType;
    }

    public enum ServerType
    {
        AspNet = 0,
        Php = 1
    }
}
