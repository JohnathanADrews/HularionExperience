using HularionExperience.Embedded.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HularionExperience.Embedded.Views
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class Browser : UserControl
    {
        public Browser()
        {
            InitializeComponent();

            CefBrowser.IsBrowserInitializedChanged += CefBrowser_IsBrowserInitializedChanged;

        }

        private void CefBrowser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = ((BrowserViewModel)DataContext);
            context.CefBrowser = CefBrowser;
            if ((bool)e.NewValue) { context.Start(); }
        }
    }
}
