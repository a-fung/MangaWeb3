using System;
using System.Collections;
using System.Collections.Generic;
using System.Html;
using jQueryApi;

namespace afung.MangaWeb3.Client
{
    /// <summary>
    /// The Strings class which contains logic related to strings including language
    /// </summary>
    public class Strings
    {
        /// <summary>
        /// The default language is English (United States)
        /// </summary>
        public const string DefaultLanguage = "en-us";

        /// <summary>
        /// List of supported languages
        /// </summary>
        public static readonly Dictionary<string, string> Languages = new Dictionary<string, string>(
            "en-us", "English (United States)",
            "zh-hk", "Chinese (Hong Kong)");

        /// <summary>
        /// Format of language file path
        /// </summary>
        private const string LanguageFilePathFormat = "lang/{0}.html";

        /// <summary>
        /// Current Language
        /// </summary>
        private static string CurrentLanguage = DefaultLanguage;

        /// <summary>
        /// Dictionary to store loaded language data
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> loadedLanguageData = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Load default language
        /// </summary>
        /// <param name="successCallback">Success Callback</param>
        /// <param name="failureCallback">Failure Callback</param>
        public static void LoadDefaultLanguage(Action successCallback, Action<Exception> failureCallback)
        {
            LoadLanguageFile(DefaultLanguage, successCallback, failureCallback);
        }

        /// <summary>
        /// Load user language
        /// </summary>
        /// <param name="successCallback">Success Callback</param>
        /// <param name="failureCallback">Failure Callback</param>
        public static void LoadUserLanguage(Action successCallback, Action<Exception> failureCallback)
        {
            // TODO: add option to change user language
            LoadLanguageFile(CurrentLanguage, successCallback, failureCallback);
        }

        /// <summary>
        /// Load a language file
        /// </summary>
        /// <param name="language">The language to be loaded</param>
        /// <param name="successCallback">Success Callback</param>
        /// <param name="failureCallback">Failure Callback</param>
        public static void LoadLanguageFile(string language, Action successCallback, Action<Exception> failureCallback)
        {
            if (loadedLanguageData.ContainsKey(language))
            {
                successCallback();
                return;
            }

            AjaxErrorCallback onError = delegate(jQueryXmlHttpRequest request, string textStatus, Exception error)
            {
                failureCallback(error);
            };

            AjaxRequestCallback onFinish = delegate(object data, string textStatus, jQueryXmlHttpRequest request)
            {
                loadedLanguageData[language] = new Dictionary<string, string>();

                jQuery.FromHtml((string)data).Each(delegate(int index, Element element)
                {
                    jQueryObject currentObject = jQuery.FromElement(element);
                    if (String.Compare(element.TagName, "p", true) == 0)
                    {
                        string stringId = currentObject.GetAttribute("id");
                        if (!String.IsNullOrEmpty(stringId))
                        {
                            loadedLanguageData[language][stringId] = currentObject.GetHtml();
                        }
                    }
                });

                successCallback();
            };

            jQueryAjaxOptions options = new jQueryAjaxOptions();
            options.Type = "GET";
            options.DataType = "html";
            options.Error = onError;
            options.Success = onFinish;

            jQuery.Ajax(String.Format(LanguageFilePathFormat, language), options);
        }

        /// <summary>
        /// Get the string from string ID
        /// </summary>
        /// <param name="stringId">String ID</param>
        /// <returns>The string to display</returns>
        public static string GetHtml(string stringId)
        {
            if (CurrentLanguage != DefaultLanguage && loadedLanguageData.ContainsKey(CurrentLanguage) && loadedLanguageData[CurrentLanguage].ContainsKey(stringId))
            {
                return loadedLanguageData[CurrentLanguage][stringId];
            }
            else if (loadedLanguageData.ContainsKey(DefaultLanguage) && loadedLanguageData[DefaultLanguage].ContainsKey(stringId))
            {
                return loadedLanguageData[DefaultLanguage][stringId];
            }

            return "String Not Defined";
        }

        public static string Get(string stringId)
        {
            return jQuery.FromHtml("<span>" + GetHtml(stringId) + "</span>").GetText();
        }
    }
}
