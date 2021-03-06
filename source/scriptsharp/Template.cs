// Template.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace afung.MangaWeb3.Client
{
    /// <summary>
    /// Template class. Manage all template loading and parsing
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Format of template file path
        /// </summary>
        private const string TemplateFilePathFormat = "template/{0}.html";

        /// <summary>
        /// The template files to load
        /// </summary>
        public static string[] Templates = { "client" };

        /// <summary>
        /// The IDs in template files to parse
        /// The order of the IDs matters!
        /// </summary>
        public static Dictionary<string, string[]> TemplateIds =
            new Dictionary<string, string[]>(
                "client",
                new string[] {
                    "noguest-warning",
                    "noitem-well",
                    "loading-well",
                    "read-module",
                    "change-password-modal",
                    "settings-module",
                    "search-module",
                    "folders-trow",
                    "folders-table",
                    "folders-module",
                    "mangas-list-item-details-tag-link",
                    "mangas-list-item-details",
                    "mangas-list-item",
                    "mangas-list-row",
                    "mangas-breadcrumb-tag",
                    "mangas-breadcrumb-active-folder",
                    "mangas-breadcrumb-link",
                    "mangas-module",
                    "nav-logout-dropdown",
                    "nav-login-button",
                    "confirm-modal",
                    "widget-pagination-page",
                    "widget-pagination",
                    "login-modal",
                    "error-modal"
                });

        private static jQueryObject _tempDivObject = null;

        private static jQueryObject TempDivObject
        {
            get
            {
                if (_tempDivObject == null)
                {
                    _tempDivObject = jQuery.FromElement(Document.CreateElement("div")).AppendTo(jQuery.Select("body")).AddClass("temp");
                }
                return _tempDivObject;
            }
        }

        /// <summary>
        /// The loaded template data
        /// </summary>
        private static Dictionary<string, Dictionary<string, jQueryObject>> loadedTemplateData = new Dictionary<string, Dictionary<string, jQueryObject>>();

        public static void LoadTemplateFile(Action successCallback, Action<Exception> failureCallback)
        {
            int templateIndex = 0;
            string currentTemplateFile = string.Empty;
            Action loadNextTemplate = delegate { };

            AjaxErrorCallback onError = delegate(jQueryXmlHttpRequest request, string textStatus, Exception error)
            {
                failureCallback(new Exception(String.Format(Strings.Get("TemplateLoadError"), currentTemplateFile, error)));
            };

            AjaxRequestCallback onFinish = delegate(object data, string textStatus, jQueryXmlHttpRequest request)
            {
                jQueryObject templateDiv = jQuery.FromElement(Document.CreateElement("div")).AppendTo(TempDivObject).Append(jQuery.FromHtml((string)data));

                loadedTemplateData[currentTemplateFile] = new Dictionary<string, jQueryObject>();
                foreach (string templateId in TemplateIds[currentTemplateFile])
                {
                    jQueryObject selectedTemplate = jQuery.Select("#" + templateId, templateDiv);
                    if (selectedTemplate.Length != 1)
                    {
                        failureCallback(new Exception(String.Format(Strings.Get("TemplateParseError"), currentTemplateFile, templateId)));
                        return;
                    }

                    loadedTemplateData[currentTemplateFile][templateId] = selectedTemplate.Clone();
                    selectedTemplate.Remove();
                }

                templateDiv.Remove();

                templateIndex++;
                loadNextTemplate();
            };

            loadNextTemplate = delegate
            {
                if (templateIndex >= Templates.Length)
                {
                    successCallback();
                    return;
                }

                currentTemplateFile = Templates[templateIndex];

                jQueryAjaxOptions options = new jQueryAjaxOptions();
                options.Type = "GET";
                options.DataType = "html";
                options.Error = onError;
                options.Success = onFinish;

                jQuery.Ajax(String.Format(TemplateFilePathFormat, currentTemplateFile), options);
            };

            loadNextTemplate();
        }

        [AlternateSignature]
        public extern static jQueryObject Get(string template, string templateId);
        public static jQueryObject Get(string template, string templateId, bool removeId)
        {
            if (loadedTemplateData.ContainsKey(template) && loadedTemplateData[template].ContainsKey(templateId))
            {
                jQueryObject obj = loadedTemplateData[template][templateId];
                obj = obj.Clone().AppendTo(TempDivObject);

                jQuery.Select(".msg", obj).Each(delegate(int index, Element element)
                {
                    jQueryObject msgObj = jQuery.FromElement(element).RemoveClass("msg");
                    foreach (string className in msgObj.GetAttribute("class").Split(" "))
                    {
                        if (className.StartsWith("msg-"))
                        {
                            msgObj.Html(Strings.GetHtml(className.Substr(4))).RemoveClass(className);
                            break;
                        }
                    }
                });

                jQuery.Select(".plhdr", obj).Each(delegate(int index, Element element)
                {
                    jQueryObject msgObj = jQuery.FromElement(element).RemoveClass("plhdr");
                    foreach (string className in msgObj.GetAttribute("class").Split(" "))
                    {
                        if (className.StartsWith("plhdr-"))
                        {
                            msgObj.Attribute("placeholder", Strings.Get(className.Substr(6))).RemoveClass(className);
                            break;
                        }
                    }
                });

                jQuery.Select(".val", obj).Each(delegate(int index, Element element)
                {
                    jQueryObject msgObj = jQuery.FromElement(element).RemoveClass("val");
                    foreach (string className in msgObj.GetAttribute("class").Split(" "))
                    {
                        if (className.StartsWith("val-"))
                        {
                            msgObj.Value(Strings.Get(className.Substr(4))).RemoveClass(className);
                            break;
                        }
                    }
                });

                if (removeId)
                {
                    obj.RemoveAttr("id");
                }

                return obj.Detach();
            }

            throw new Exception(String.Format(Strings.Get("TemplateGetError"), template, templateId));
        }
    }
}
