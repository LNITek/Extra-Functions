using System;
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
        Button BTN = new() { Height = 20, Width = 75, Margin = new Thickness(5) };

        /// <summary>
        /// Buttons Purpose / Function On The Promt
        /// </summary>
        public ButtonResult BTNResult { get; private set; }

        /// <summary>
        /// Returns The Button Component
        /// </summary>
        public Button Button
        {
            get
            {
                return BTN;
            }
        }

        /// <summary>
        /// A Complite Custom Button
        /// </summary>
        /// <param name="Text">Button Text</param>
        /// <param name="Result">Buttons Purpose</param>
        public BasicButton(string Text, ButtonResult Result)
        {
            BTN.Content = Text;
            BTNResult = Result;
            Format();
        }

        /// <summary>
        /// A Set Of Preset Custom Buttons
        /// </summary>
        /// <param name="ButtonResult">The Buttons Purpose Into A Complite Custom Buttton</param>
        public BasicButton(ButtonResult ButtonResult)
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
            Format();
        }

        private void Format()
        {
            switch (BTNResult)
            {
                case ButtonResult.Accept:
                case ButtonResult.Retry: BTN.IsDefault = true; BTN.Click += AcceptClick; break;
                case ButtonResult.Cancel: BTN.IsCancel = true; break;
            }
        }

        private void AcceptClick(object sender, EventArgs e)
        {
            var w = (Window)((Grid)((StackPanel)((Button)sender).Parent).Parent).Parent;
            w.DialogResult = true;
            w.Close();
        }
    }
}
