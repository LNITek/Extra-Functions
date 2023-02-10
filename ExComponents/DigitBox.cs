using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            PreviewKeyDown += new KeyEventHandler(OnKeyDown);

            AddHandler(PreviewMouseLeftButtonDownEvent,
              new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            AddHandler(GotKeyboardFocusEvent,
              new RoutedEventHandler(SelectAllText), true);
            AddHandler(MouseDoubleClickEvent,
              new RoutedEventHandler(SelectAllText), true);
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

        private bool IsOtherKey(Key inKey) =>
            inKey == Key.Delete || inKey == Key.Back || inKey == Key.Decimal || inKey == Key.OemMinus || inKey == Key.Subtract 
            || inKey == Key.Tab || inKey == Key.Up || inKey == Key.Down;

        private string LeaveOnlyNumbers(string inString)
        {
            string tmp = inString;
            foreach (char c in inString.ToCharArray())
            {
                if (!Regex.IsMatch(c.ToString(), @"^[0-9.-]*$"))
                {
                    tmp = tmp.Replace(c.ToString(), "");
                }
            }
            return tmp;
        }

        private protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && double.TryParse(Text, out double Val) && !IsReadOnly)
                Text = (Val + 1).ToString();
            if (e.Key == Key.Down && double.TryParse(Text, out Val) && !IsReadOnly)
                Text = (Val - 1).ToString();
            e.Handled = !IsNumberKey(e.Key) && !IsOtherKey(e.Key);
        }

        private protected void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var s = SelectionStart;
            base.Text = LeaveOnlyNumbers(Text);
            Select(s, 0);
        }

        private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focussed, give it the focus and
                    // stop further processing of this click event.
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox textBox)
                textBox.SelectAll();
        }
    }
}
