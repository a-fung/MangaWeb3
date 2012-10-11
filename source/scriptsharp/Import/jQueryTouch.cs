// jQueryTouch.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace jQueryApi
{
    [Imported]
    [IgnoreNamespace]
    public class jQueryObjectTouch : jQueryObject
    {
        [ScriptName("touchInit")]
        public jQueryObjectTouch TouchInitialize(Dictionary<string, object> options) { return this; }
    }

    [Imported]
    [IgnoreNamespace]
    public class jQueryTouchEvent : jQueryEvent
    {
        [ScriptName("originalType")]
        public string OriginalType;

        [ScriptName("touches")]
        public jQueryTouchObject[] Touches;
    }

    [Imported]
    [IgnoreNamespace]
    public class jQueryTouchObject
    {
        [ScriptName("id")]
        public int Id;

        [ScriptName("clientX")]
        public int clientX;

        [ScriptName("clientY")]
        public int clientY;

        [ScriptName("pageX")]
        public int pageX;

        [ScriptName("pageY")]
        public int pageY;

        [ScriptName("screenX")]
        public int screenX;

        [ScriptName("screenY")]
        public int screenY;
    }
}
