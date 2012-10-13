using System;
using System.Collections.Generic;
using System.Html;
using afung.MangaWeb3.Client.Module;
using jQueryApi;

namespace afung.MangaWeb3.Client
{
    /// <summary>
    /// Class Application
    /// </summary>
    public class Application
    {
        /// <summary>
        /// The HTML for showing fatal error message
        /// </summary>
        private const string FatalErrorMessageHtml = "<div class=\"container\"><div class=\"row\"><div class=\"span12\"><div class=\"alert alert-error\"><h4>Fatal Error</h4><span class=\"fatalerrormsg\"></span></div></div></div></div>";

        /// <summary>
        /// The class for showing fatal error message inside the above HTML
        /// </summary>
        private const string FatalErrorMessageClass = ".fatalerrormsg";

        /// <summary>
        /// Start the application
        /// </summary>
        public Application()
        {
            // Work around for iPhone 5 add to home screen letterbox bug
            if (Window.Screen.Width == 320 && Window.Screen.Height == 568)
            {
                jQuery.Select("meta[name=\"viewport\"]").Attribute("content", "width=320.01, initial-scale=1, maximum-scale=1, minimum-scale=1, user-scalable=0");
            }

            // Load default language first
            Action<Exception> defaultLanguageLoadFailed = delegate(Exception error)
            {
                ShowFatalError(String.Format("Unabled to load default language file ({0}). Please check that you have the correct file on your server", error));
            };

            Action defaultLanguageLoadFinished = delegate()
            {
                // Then load user language
                Action<Exception> userLanguageLoadFailed = delegate(Exception error)
                {
                    Settings.UserLanguage = Strings.DefaultLanguage;
                    StartStage2();
                };

                Action userLanguageLoadFinished = delegate()
                {
                    StartStage2();
                };

                try
                {
                    Strings.LoadUserLanguage(userLanguageLoadFinished, userLanguageLoadFailed);
                }
                catch (Exception error)
                {
                    userLanguageLoadFailed(error);
                }
            };

            try
            {
                Strings.LoadDefaultLanguage(defaultLanguageLoadFinished, defaultLanguageLoadFailed);
            }
            catch (Exception error)
            {
                defaultLanguageLoadFailed(error);
            }
        }

        /// <summary>
        /// Stage 2 of booting up the application
        /// </summary>
        protected virtual void StartStage2()
        {
            // Load default language first
            Action<Exception> templateLoadFailed = delegate(Exception error)
            {
                ShowFatalError(error.ToString());
            };

            Action templateLoadFinished = delegate()
            {
                LoadFirstModule();
            };

            Template.LoadTemplateFile(templateLoadFinished, templateLoadFailed);
        }

        protected virtual void LoadFirstModule()
        {
            MangasModule.Instance.Show(null);
        }

        /// <summary>
        /// Show fatal error
        /// </summary>
        /// <param name="message">the message to be shown</param>
        public static void ShowFatalError(string message)
        {
            jQueryObject errorMessageObject = jQuery.FromHtml(FatalErrorMessageHtml);
            jQuery.Select("body").Prepend(errorMessageObject);
            jQuery.Select(FatalErrorMessageClass, errorMessageObject).Text(message);
        }

        public static void Refresh()
        {
            Window.Location.Href = Window.Location.Pathname.Unescape();
        }
    }
}
