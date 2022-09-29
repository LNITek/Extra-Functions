using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// A Textbox For Numbers (Integer , Decimal , Etc)
    /// </summary>
    public class DigitBox : TextBox
    {
        /// <summary>
        /// A Textbox For Numbers (Integer , Decimal , Etc)
        /// </summary>
        public DigitBox()
        {
            TextChanged += new TextChangedEventHandler(OnTextChanged);
            KeyDown += new KeyEventHandler(OnKeyDown);
        }
        /// <summary>
        /// Text / Value Of The Component
        /// </summary>
        new public string Text
        {
            get { return base.Text; }
            set
            {
                var s = SelectionStart;
                base.Text = LeaveOnlyNumbers(value);
                Select(s, 0);
            }
        }

        private bool IsNumberKey(Key inKey)
        {
            if (inKey < Key.D0 || inKey > Key.D9)
            {
                if (inKey < Key.NumPad0 || inKey > Key.NumPad9)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsOtherKey(Key inKey)
        {
            return inKey == Key.Delete || inKey == Key.Back || inKey == Key.Decimal;
        }

        private string LeaveOnlyNumbers(string inString)
        {
            string tmp = inString;
            foreach (char c in inString.ToCharArray())
            {
                if (!Regex.IsMatch(c.ToString(), "^[0-9.]*$"))
                {
                    tmp = tmp.Replace(c.ToString(), "");
                }
            }
            return tmp;
        }

        private protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !IsNumberKey(e.Key) && !IsOtherKey(e.Key);
        }

        private protected void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var s = SelectionStart;
            base.Text = LeaveOnlyNumbers(Text);
            Select(s, 0);
        }
    }
}
