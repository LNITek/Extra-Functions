using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows;
using ExtraFunctions.Extras;
using ExtraFunctions.ExComponents;

namespace ExtraFunctions.ExInput
{
    /// <summary>
    /// Create A Numeric Value Input Prompt
    /// </summary>
    public class NumberPrompt : ExInputBase, IExInput
    {
        readonly NumericUpDown edtInput = new NumericUpDown() { Margin = new Thickness(10, 0, 10, 0) };

        /// <summary>
        /// Create A Numeric Value Input Prompt
        /// </summary>
        /// <param name="Parent">The Parent In Wich The Prompt Must Center In</param>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Value">Set A Default Value</param>
        /// <param name="MinValue">Set A Min Value</param>
        /// <param name="MaxValue">Set A Max Value </param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public NumberPrompt(Window Parent, string Title, string PromptText, double Value = 0, 
            double MinValue = 0, double MaxValue = 100, BasicButton[] Buttons = default)
        {
            Main.Owner = Parent;
            this.Title = Title;
            this.Message = PromptText;
            this.Value = Value.ToString();
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);

            edtInput.MinValue = MinValue;
            edtInput.MaxValue = MaxValue;
            edtInput.SetBinding(TextBox.TextProperty, new Binding(nameof(TextBox.Text)) 
            { Source = this, Mode = BindingMode.TwoWay});
            ExComponent = edtInput;
            SetSlotB();
        }

        /// <summary>
        /// Create A Standard Text Input Prompt
        /// </summary>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Value">Set A Default Value</param>
        /// <param name="MinValue">Set A Min Value</param>
        /// <param name="MaxValue">Set A Max Value </param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public NumberPrompt(string Title, string PromptText, double Value = 0, double MinValue = 0, double MaxValue = 100, 
            BasicButton[] Buttons = default)
        {
            this.Title = Title;
            this.Message = PromptText;
            this.Value = Value.ToString();
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);

            edtInput.MinValue = MinValue;
            edtInput.MaxValue = MaxValue;
            edtInput.SetBinding(TextBox.TextProperty, new Binding(nameof(TextBox.Text))
            { Source = this, Mode = BindingMode.TwoWay });
            ExComponent = edtInput;
            SetSlotB();
        }
    }
}
