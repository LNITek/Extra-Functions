using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ExtraFunctions.Extras;

namespace ExtraFunctions.ExInput
{

    /// <summary>
    /// Basic Way To Create Custom Buttons
    /// </summary>
    public class BasicButton
    {
        readonly Button BTN = new Button() 
        { HorizontalAlignment = HorizontalAlignment.Right, Height = 20, Width = 75, Margin = new Thickness(5) };

        /// <summary>
        /// Buttons Purpose / Function On The Promt
        /// </summary>
        public ButtonResult BTNResult { get; private set; }

        /// <summary>
        /// Returns The Button Component
        /// </summary>
        public Button Button { get { return BTN; } }

        /// <summary>
        /// A Complite Custom Button
        /// </summary>
        /// <param name="Text">Button Text</param>
        /// <param name="ButtonResult">Buttons Purpose</param>
        /// <param name="ClickEvent">On Click Event</param>
        public BasicButton(string Text, ButtonResult ButtonResult, RoutedEventHandler ClickEvent = null)
        {
            BTN.Content = Text;
            BTNResult = ButtonResult;
            if (ClickEvent != null) BTN.Click += ClickEvent;
            Format();
        }

        /// <summary>
        /// A Set Of Preset Custom Buttons
        /// </summary>
        /// <param name="ButtonResult">The Buttons Purpose Into A Complite Custom Buttton</param>
        /// <param name="ClickEvent">On Click Event</param>
        public BasicButton(ButtonResult ButtonResult, RoutedEventHandler ClickEvent = null)
        {
            string Text;

            switch (ButtonResult)
            {
                case ButtonResult.None: Text = "None"; break;
                case ButtonResult.Cancel: Text = "Cancel";  break;
                case ButtonResult.Accept: Text = "Ok";  break;
                case ButtonResult.Retry: Text = "Retry"; break;
                default: Text = "Button"; break;
            }

            BTN.Content = Text;
            BTNResult = ButtonResult;
            if (ClickEvent != null) BTN.Click += ClickEvent;
            Format();
        }

        private void Format()
        {
            DockPanel.SetDock(BTN, Dock.Right);
            switch (BTNResult)
            {
                case ButtonResult.Accept:
                case ButtonResult.Retry: BTN.IsDefault = true; BTN.Click += AcceptClick; break;
                case ButtonResult.Cancel when !BTN.IsDefault: BTN.IsCancel = true; break;
            }
        }

        /// <summary>
        /// RoutedEventHandler For Accept Button.
        /// ButtonResult.Accept And ButtonResult.Retry Will Exicute This Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        public void AcceptClick(object sender, EventArgs e)
        {
            FrameworkElement Main = (FrameworkElement)sender;
            while (!(Main is Window))
            {
                if (Main == null) throw new Exception("The Parent Of The Button Is Not A Window");
                Main = (FrameworkElement)Main.Parent;
            }
            var Win = Main as Window;
            Win.DialogResult = true;
            Win.Close();
        }

        /// <summary>
        /// RoutedEventHandler For Cancel Button.
        /// ButtonResult.CancelS Will Exicute This Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        public void CancelClick(object sender, EventArgs e)
        {
            FrameworkElement Main = (FrameworkElement)sender;
            while (!(Main is Window))
            {
                if (Main == null) throw new Exception("The Parent Of The Button Is Not A Window");
                Main = (FrameworkElement)Main.Parent;
            }
            var Win = Main as Window;
            Win.DialogResult = false;
            Win.Close();
        }
    }
}
