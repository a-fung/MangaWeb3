// CKFinder.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace Finder
{
    [Imported]
    [IgnoreNamespace]
    public class CKFinder
    {
        [ScriptName("config")]
        public static CKFinderConfig Config;

        [ScriptName("basePath")]
        public string BasePath;

        [ScriptName("height")]
        public int Height;

        [ScriptName("api")]
        public CKFinderAPI Api;

        [ScriptName("callback")]
        public Action<CKFinderAPI> Callback;

        [ScriptName("appendTo")]
        public void AppendTo(string elementId) { }

        [ScriptName("selectActionFunction")]
        public Action<string> SelectActionFunction;
    }

    [Imported]
    [IgnoreNamespace]
    public class CKFinderConfig
    {
        [ScriptName("language")]
        public string Language;

        [ScriptName("connectorInfo")]
        public string ConnectorInfo;
    }

    [Imported]
    [IgnoreNamespace]
    public class CKFinderAPI
    {
        [ScriptName("disableFileContextMenuOption")]
        public void DisableFileContextMenuOption(string option, bool addedUsingApi) { }

        [ScriptName("disableFolderContextMenuOption")]
        public void DisableFolderContextMenuOption(string option, bool addedUsingApi) { }

        [ScriptName("destroy")]
        public void Destroy() { }

        [ScriptName("destroy")]
        public void Destroy(Action callback) { }

        [ScriptName("getSelectedFile")]
        public CKFinderFile GetSelectedFile() { return null; }

        [ScriptName("getSelectedFolder")]
        public CKFinderFile GetSelectedFolder() { return null; }
    }

    [Imported]
    [IgnoreNamespace]
    public class CKFinderFile
    {
        [ScriptName("getUrl")]
        public string GetUrl() { return null; }
    }

    [Imported]
    [IgnoreNamespace]
    public class CKFinderFolder
    {
        [ScriptName("getUrl")]
        public string GetUrl() { return null; }
    }

    [Imported]
    [IgnoreNamespace]
    [ScriptName("window")]
    public class CKFinderWindow
    {
        [ScriptName("CKFinder")]
        public static object CKFinder;
    }
}