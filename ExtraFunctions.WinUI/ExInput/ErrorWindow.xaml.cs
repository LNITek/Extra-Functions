using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using WinUIEx;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Windowing;
using Microsoft.UI;

namespace ExtraFunctions.WinUI.ExInput
{
    /// <summary>
    /// Show a window dialog for error code, messages. Use for error handaling. 
    /// </summary>
    public sealed partial class ErrorWindow : WindowEx
    {
        /// <summary>
        /// Show a error window with a owner.
        /// </summary>
        /// <remarks>
        /// If you don't have/want to have a owner use <see cref="ErrorWindow"/> contructor instead.
        /// </remarks>
        /// <param name="Owner">This windows pairent window</param>
        /// <param name="Title">The text in the titel bar</param>
        /// <param name="Message">The message to display</param>
        public static void Show(WindowId Owner, string Title, string Message)
        {
            var win = new ErrorWindow(Title, Message);

            var app = AppWindow.Create(win.Presenter, Owner, win.DispatcherQueue);
            app.Show(true);
        }

        /// <summary>
        /// Creates a Error dialog as a window.
        /// </summary>
        /// <param name="Title">The text in the titel bar</param>
        /// <param name="Message">The message to display</param>
        public ErrorWindow(string Title, string Message)
        {
            InitializeComponent();

            this.Title = Title;
            lblMSG.Text = Message;

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            lblMSG.Loaded += (s, e) =>
            {
                Height = lblMSG.ActualHeight + 120;
                this.CenterOnScreen();
            };
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Copy(object sender, RoutedEventArgs e = null)
        {
            var package = new DataPackage();
            package.SetText(lblMSG.Text);
            Clipboard.SetContent(package);
        }
    }
}
