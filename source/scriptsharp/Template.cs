// Template.cs
//

using System;
using System.Collections.Generic;
using jQueryApi;
using System.Html;

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
                });

        /// <summary>
        /// The loaded template data
        /// </summary>
        private static Dictionary<string, Dictionary<string, jQueryObject>> loadedTemplateData = new Dictionary<string, Dictionary<string, jQueryObject>>();

        public static void LoadTemplateFile(Action successCallback, Action<Exception> failureCallback)
        {
            int templateIndex = 0;
            string currentTemplateFile = string.Empty;
            jQueryObject tempDiv = jQuery.FromElement(Document.CreateElement(HtmlConstants.TagDiv)).AppendTo(jQuery.Select(HtmlConstants.TagBody)).AddClass(HtmlConstants.ClassTemp);
            Action loadNextTemplate = delegate { };

            AjaxErrorCallback onError = delegate(jQueryXmlHttpRequest request, string textStatus, Exception error)
            {
                failureCallback(error);
            };

            AjaxRequestCallback onFinish = delegate(object data, string textStatus, jQueryXmlHttpRequest request)
            {
                jQueryObject templateDiv = jQuery.FromElement(Document.CreateElement(HtmlConstants.TagDiv)).AppendTo(tempDiv).Append(jQuery.FromHtml((string)data));

                loadedTemplateData[currentTemplateFile] = new Dictionary<string, jQueryObject>();
                foreach (string templateId in TemplateIds[currentTemplateFile])
                {
                    jQueryObject selectedTemplate = jQuery.Select("#" + templateId, templateDiv);
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
                    tempDiv.Remove();
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
    }
}
