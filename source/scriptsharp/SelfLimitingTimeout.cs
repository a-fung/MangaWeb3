// SelfLimitingTimeout.cs
//

using System;
using System.Collections.Generic;
using System.Html;

namespace afung.MangaWeb3.Client
{
    public class SelfLimitingTimeout
    {
        private int id;
        public SelfLimitingTimeout()
        {
            id = -1;
        }

        public void Start(Action callback, int timeout)
        {
            if (id == -1)
            {
                id = Window.SetTimeout(
                    delegate
                    {
                        id = -1;
                        callback();
                    },
                    timeout);
            }
        }
    }
}
