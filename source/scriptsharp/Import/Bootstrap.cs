// Bootstrap.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace jQueryApi
{
    [Imported]
    [IgnoreNamespace]
    public abstract class jQueryBootstrap : jQueryObject
    {
        [ScriptName("modal")]
        public abstract jQueryObject Modal();

        [ScriptName("modal")]
        public abstract jQueryObject Modal(object options);

        [ScriptName("typeahead")]
        public abstract jQueryObject Typeahead();

        [ScriptName("typeahead")]
        public abstract jQueryObject Typeahead(object options);

        [ScriptName("tooltip")]
        public abstract jQueryObject Tooltip(object options);
    }

    [Imported]
    [IgnoreNamespace]
    public abstract class BootstrapTypeahead
    {
        [ScriptName("source")]
        public object[] Source;
    }

    [Imported]
    [IgnoreNamespace]
    [ScriptName("$.support")]
    public class BootstrapTransition
    {
        [ScriptName("transition")]
        public static bool Support;

        [ScriptName("transition.end")]
        public static string TransitionEventEndName;
    }
}