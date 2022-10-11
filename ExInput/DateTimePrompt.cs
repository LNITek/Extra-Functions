using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows;
using ExtraFunctions.Extras;

namespace ExtraFunctions.ExInput
{
    /// <summary>
    /// Create A Date Input Prompt
    /// </summary>
    public class DateTimePrompt : ExInputBase, IExInput
    {
        readonly DatePicker dtpInput = new DatePicker() { Margin = new Thickness(10, 0, 10, 0) };

        /// <summary>
        /// Create A Date Input Prompt
        /// </summary>
        /// <param name="Parent">The Parent In Wich The Prompt Must Center In</param>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Value">Set A Default Value For The Date Picker</param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public DateTimePrompt(IWin32Window Parent, string Title, string PromptText, DateTime Value = default, 
            BasicButton[] Buttons = default)
        {
            Main.Owner = (Window)Parent;
            this.Title = Title;
            this.Message = PromptText;
            if (Value == default) Value = DateTime.Now;
            else this.Value = Value.ToShortDateString();
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);

            dtpInput.SetBinding(DatePicker.TextProperty, new Binding(nameof(this.Value)) 
            { Source = this, Mode = BindingMode.TwoWay });
            dtpInput.DisplayDate = Value;
            ExComponent = dtpInput;
            SetSlotB();
        }

        /// <summary>
        /// Create A Date Input Prompt
        /// </summary>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Value">Set A Default Value For The Date Picker</param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public DateTimePrompt(string Title, string PromptText, DateTime Value = default, BasicButton[] Buttons = default)
        {
            this.Title = Title;
            this.Message = PromptText;
            if (Value == default) Value = DateTime.Now;
            else this.Value = Value.ToShortDateString();
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);

            dtpInput.SetBinding(DatePicker.TextProperty, new Binding(nameof(this.Value))
            { Source = this, Mode = BindingMode.TwoWay });
            dtpInput.DisplayDate = Value;
            ExComponent = dtpInput;
            SetSlotB();
        }
    }
}
