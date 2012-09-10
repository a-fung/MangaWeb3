// Bootstrap.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using jQueryApi;

[Imported]
[IgnoreNamespace]
public abstract class jQueryBootstrap : jQueryObject
{
    [ScriptName("modal")]
    public abstract jQueryObject Modal();

    [ScriptName("modal")]
    public abstract jQueryObject Modal(object options);
}
