using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace afung.MangaWeb3.Server.Provider
{
    interface IMangaProvider
    {
        bool TryOpen(string path);
    }
}
