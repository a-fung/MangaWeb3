// SelfClearingTimeout.cs
//

using System;
using System.Collections.Generic;
using System.Html;

namespace afung.MangaWeb3.Client
{
    public class SelfClearingTimeout
    {
        private int id;
        public SelfClearingTimeout()
        {
        }

        public void Start(Action callback, int timeout)
        {
            Clear();
            id = Window.SetTimeout(callback, timeout);
        }

        public void Clear()
        {
            Window.ClearTimeout(id);
        }
    }
}
