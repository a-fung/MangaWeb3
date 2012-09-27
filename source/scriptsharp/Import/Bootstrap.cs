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

    [ScriptName("typeahead")]
    public abstract jQueryObject Typeahead();

    [ScriptName("typeahead")]
    public abstract jQueryObject Typeahead(object options);
}


[Imported]
[IgnoreNamespace]
public abstract class BootstrapTypeahead
{
    [ScriptName("source")]
    public object[] Source;
}