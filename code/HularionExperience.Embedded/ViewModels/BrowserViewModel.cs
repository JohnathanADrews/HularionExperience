#region License
/*
MIT License

Copyright (c) 2023-2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using CefSharp;
using HularionCore.Pattern.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Embedded.ViewModels
{
    public class BrowserViewModel : ViewModel
    {

        public CefSharp.Wpf.ChromiumWebBrowser CefBrowser { get; set; }

        private EmbeddedHularionExperienceApplication application;

        public BrowserViewModel(EmbeddedHularionExperienceApplication application)
        {
            this.application = application;
            application.ScreenController.ReloadHandler.Handler = new Action(() =>
            {
                CefBrowser.Dispatcher.Invoke(() =>
                {
                    ReloadBrowser();
                });
            });
            application.ScreenController.ShowDevToolsHandler.Handler = new Action(() =>
            {
                CefBrowser.Dispatcher.Invoke(() =>
                {
                    CefBrowser.ShowDevTools();
                });
            });
        }

        public void Start()
        {
            Load();
        }

        public void ReloadBrowser()
        {
            application.HXApplication.Refresh();
            Load();
        }

        private void Load()
        {
            CefBrowser.WebBrowser.JavascriptObjectRepository.UnRegisterAll();
            foreach (var registration in application.RegistrationObjects)
            {
                CefBrowser.WebBrowser.JavascriptObjectRepository.Register(registration.Name, registration.Value);
            }
            foreach (var handler in application.ResourceHandlers)
            {
                handler.Stream.Position = 0;
                CefBrowser.WebBrowser.RegisterResourceHandler(handler.Url, handler.Stream, mimeType: handler.MimeType);
            }
            CefBrowser.LoadHtml(application.HXApplication.IndexPage, application.Url);

            application.HXApplication.RegisterRouteSender(message =>
            {
                CefBrowser.WebBrowser.ExecuteScriptAsync(String.Format("hularionReceiver({0});", message));
            });
            CefBrowser.ShowDevTools();
        }


    }
}
