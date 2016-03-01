using System;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace VKPlugin
{
    internal class BrowserWindow
    {
        private Window window = new Window();
        private StackPanel dialogSP = new StackPanel();
        private WebBrowser browser = new WebBrowser();        
        private double windowWidth = 650;
        private double windowHeight = 500;
        private VKAudio vkAudio = new VKAudio();        

        private BrowserWindow()
        {
            window.Width = windowWidth;
            window.Height = windowHeight;
            window.ResizeMode = ResizeMode.NoResize;
            try
            {
                window.Icon = new BitmapImage(new Uri(Environment.CurrentDirectory + @"\Plugins\VKPlugin\Images\faviconnew.ico"));
            }
            catch { }
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;            
            window.Title = "Пожалуйста, подождите...";
            browser.Width = 640;
            browser.Height = 520;
            browser.Margin = new Thickness(0, -60, 0, 0);
            browser.LoadCompleted += Browser_LoadCompleted;
            HideScriptErrors(browser, true);
            dialogSP.Orientation = Orientation.Vertical;
            dialogSP.Children.Add(browser);
            window.Content = dialogSP;
        }        

        internal BrowserWindow(VKAudio vk) : this()
        {
            vkAudio = vk;
        }

        private void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null)
                return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide);
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }

        internal bool? Show()
        {
            return window.ShowDialog();
        }

        internal void Navigate(string uri)
        {
            browser.Navigate(uri);
        }

        private void Browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            window.Title = "Вход | ВКонтакте";
            vkAudio.GetAccessData(browser.Source.ToString());
            if (vkAudio.HasAccessData)
                window.Close();
        }        
    }
}
