using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows;
using ExtraFunctions.Extras;

namespace ExtraFunctions.ExInput
{
    /// <summary>
    /// Create A Standard Text Input Prompt
    /// </summary>
    public class TextPrompt : ExInputBase, IExInput
    {
        readonly TextBox edtInput = new TextBox() { Text = "", Margin = new Thickness(10, 0, 10, 0) };

        /// <summary>
        /// Create A Standard Text Input Prompt
        /// </summary>
        /// <param name="Parent">The Parent In Wich The Prompt Must Center In</param>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Value">Set A Default Value In The Text Box</param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public TextPrompt(Window Parent, string Title, string PromptText, string Value = "", BasicButton[] Buttons = default)
        {
            Main.Owner = Parent;
            this.Title = Title;
            this.Message = PromptText;
            this.Value = Value;
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);

            edtInput.SetBinding(TextBox.TextProperty, new Binding(nameof(Value)) { Source = this, Mode = BindingMode.TwoWay });
            if (!string.IsNullOrWhiteSpace(Value))
                edtInput.Text = Value;
            ExComponent = edtInput;
            SetSlotB();
        }
            
        /// <summary>
        /// Create A Standard Text Input Prompt
        /// </summary>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Value">Set A User Default Value In The Text Box</param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public TextPrompt(string Title, string PromptText, string Value = "", BasicButton[] Buttons = default)
        {
            this.Title = Title;
            this.Message = PromptText;
            this.Value = Value;
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);

            edtInput.SetBinding(TextBox.TextProperty, new Binding(nameof(Value)) { Source = this, Mode = BindingMode.TwoWay });
            if (!string.IsNullOrWhiteSpace(Value))
                edtInput.Text = Value;
            ExComponent = edtInput;
            SetSlotB();
        }
    }
}
