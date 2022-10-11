using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows;
using ExtraFunctions.Extras;

namespace ExtraFunctions.ExInput
{
    /// <summary>
    /// Create A Drop Down Text Input Prompt
    /// </summary>
    public class ComboPrompt : ExInputBase, IExInput
    {
        readonly ComboBox cmbInput = new ComboBox() { Text = "", Margin = new Thickness(10, 0, 10, 0) };

        /// <summary>
        /// Create A Drop Down Text Input Prompt
        /// </summary>
        /// <param name="Parent">The Parent In Wich The Prompt Must Center In</param>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Values">The Values / Items Inside The Drop Down</param>
        /// <param name="StartIndex">Select A Default Value From Values \r\n
        /// (-1 : No Defualt Value)</param>
        /// <param name="TextInput">True To Allow User To Enter A Value, False To Only Use Selected Values</param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public ComboPrompt(IWin32Window Parent, string Title, string PromptText, string[] Values, int StartIndex = 0,
            bool TextInput = true, BasicButton[] Buttons = default)
        {
            Main.Owner = (Window)Parent;
            this.Title = Title;
            Message = PromptText;
            if (StartIndex >= Values.Length) StartIndex = Values.Length - 1;
            if (StartIndex < 0) StartIndex = 0; else Value = Values[StartIndex];
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);

            cmbInput.IsEditable = TextInput;
            cmbInput.ItemsSource = Values;
            cmbInput.SetBinding(ComboBox.TextProperty, new Binding(nameof(Value)) { Source = this, Mode = BindingMode.TwoWay });
            cmbInput.SelectedIndex = StartIndex;
            ExComponent = cmbInput;
            SetSlotB();
        }

        /// <summary>
        /// Create A Drop Down Text Input
        /// </summary>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Values">The Values / Items Inside The Drop Down</param>
        /// <param name="StartIndex">Select A Default Value From Values \r\n
        /// (-1 : No Defualt Value)</param>
        /// <param name="TextInput">True To Allow User To Enter A Value, False To Only Use Selected Values</param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public ComboPrompt(string Title, string PromptText, string[] Values, int StartIndex = 0,
            bool TextInput = true, BasicButton[] Buttons = default)
        {
            this.Title = Title;
            Message = PromptText;
            if (StartIndex >= Values.Length) StartIndex = Values.Length - 1;
            if (StartIndex < 0) StartIndex = 0; else Value = Values[StartIndex];
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);

            cmbInput.IsEditable = TextInput;
            cmbInput.ItemsSource = Values;
            cmbInput.SetBinding(ComboBox.TextProperty, new Binding(nameof(Value)) { Source = this, Mode = BindingMode.TwoWay });
            cmbInput.SelectedIndex = StartIndex;
            ExComponent = cmbInput;
            SetSlotB();
        }
    }
}
