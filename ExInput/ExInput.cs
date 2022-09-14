using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using ExtraFunctions.Extras;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Size = System.Windows.Size;

namespace ExtraFunctions.ExInput
{
    /// <summary>
    /// Extra Inputs Ranging From Text To Button Inputs
    /// </summary>
    public class ExInput
    {
        string Title { get; set; } = "Extra Inputs";
        string Message { get; set; } = "Hallo World!";
        InputMode Type { get; set; } = InputMode.Prompt;
        Icons Icon { get; set; } = Icons.None;
        static List<BasicButton> lstButtons { get; set; } = new List<BasicButton>();
        int ValueIndex { get; set; } = -1;

        /// <summary>
        /// The Returning Text Value
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// All Values Used For Combo Prompt
        /// </summary>
        public List<string> Values { get; set; }

        Window Main = new()
        {
            Background = Brushes.White,
            ShowInTaskbar = false,
            Width = 400,
            MinHeight = 150,
            ResizeMode = ResizeMode.NoResize,
            SizeToContent = SizeToContent.Height,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        StackPanel pnlBTN = new()
        {
            Margin = new Thickness(5),
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        TextBlock lblMessage = new()
        {
            Margin = new Thickness(10),
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Top
        };
        Image imgIcon = new()
        {
            Height = 100,
            Width = 100,
            Margin = new Thickness(5),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment= HorizontalAlignment.Center,
        };
        TextBox edtInput = new() { Text = "", Margin = new Thickness(10, 0, 10, 0) };
        ComboBox cmbInput = new() { Text = "", Margin = new Thickness(10, 0, 10, 0) };
        Grid GridX = new();

        /// <summary>
        /// Create A Standard Text Input 
        /// </summary>
        /// <param name="Parent">The Parent In Wich The Prompt Must Center In (Nullable)</param>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Value">Set A User Default Value In The Text Box</param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public ExInput(IWin32Window Parent, string Title, string PromptText, string Value = "", BasicButton[]? Buttons = default)
        {
            Values = new List<string>() { "" };
            lstButtons.Clear();
            Main.Owner = (Window)Parent;
            Main.Title = Title;
            lblMessage.Text = PromptText;
            this.Value = Value;
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);
            Type = InputMode.Text;
        }

        /// <summary>
        /// Create A Drop Down Text Input
        /// </summary>
        /// <param name="Parent">The Parent In Wich The Prompt Must Center In (Nullable)</param>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Values">The Values / Items Inside The Drop Down</param>
        /// <param name="StartIndex">Select A Default Value From Values \r\n
        /// (-2 : Removes Text Input, -1 : No Defualt Value)</param>
        /// <param name="TextInput">True To Allow User To Enter A Value, False To Only Use Selected Values</param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public ExInput(IWin32Window Parent, string Title, string PromptText, string[] Values, int StartIndex = 0,
            bool TextInput = true, BasicButton[]? Buttons = default)
        {
            this.Values = new List<string>() { "" };
            lstButtons.Clear();

            cmbInput.IsEditable = TextInput;
            Main.Owner = (Window)Parent;
            Main.Title = Title;
            lblMessage.Text = PromptText;
            if (StartIndex < 0) StartIndex = 0;
            if (StartIndex >= Values.Length) StartIndex = Values.Length - 1;
            Value = Values[StartIndex];
            ValueIndex = StartIndex;
            this.Values = Values.ToList();
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);
            Type = InputMode.Combo;
        }

        /// <summary>
        /// Create A Popup With A Icon And Custom Buttons
        /// </summary>
        /// <param name="Parent">The Parent In Wich The Prompt Must Center In (Nullable)</param>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Icon">The Icon To Display</param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public ExInput(IWin32Window Parent, string Title, string PromptText, Icons Icon, BasicButton[]? Buttons = default)
        {
            Values = new List<string>() { "" };
            lstButtons.Clear();

            Main.Owner = (Window)Parent;
            Main.Title = Title;
            lblMessage.Text = PromptText;
            this.Icon = Icon;
            this.Value = "";
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);
            Type = InputMode.Prompt;
        }

        /// <summary>
        /// Shows The Prompt
        /// </summary>
        /// <returns>DialogResult Of The Button</returns>
        public bool Show()
        {
            pnlBTN.Children.Clear();
            lstButtons.ForEach(x => { pnlBTN.Children.Add(x.Button); });
            Grid.SetColumn(pnlBTN, 1);
            Grid.SetRow(pnlBTN, 2);

            Grid.SetColumn(lblMessage, 1);

            GridX.Children.Clear();
            GridX.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            GridX.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) });
            GridX.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            GridX.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            GridX.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            GridX.Children.Add(ModeFormater());
            GridX.Children.Add(lblMessage);
            GridX.Children.Add(pnlBTN);

            Main.Content = GridX;

            return Main.ShowDialog() ?? false;
        }

        private UIElement ModeFormater()
        {
            switch (Type)
            {
                case InputMode.Text:
                    {
                        edtInput.SetBinding(TextBox.TextProperty, new Binding("Value") { Source = this, Mode = BindingMode.TwoWay });
                        if (!string.IsNullOrWhiteSpace(Value))
                            edtInput.Text = Value;
                        Grid.SetColumn(edtInput, 1);
                        Grid.SetRow(edtInput, 1);
                        return edtInput;
                    }
                case InputMode.Combo:
                    {
                        cmbInput.ItemsSource = Values;
                        cmbInput.SetBinding(ComboBox.TextProperty, new Binding("Value") { Source = this, Mode = BindingMode.TwoWay });
                        if (ValueIndex < 0)
                            throw new Exception($"Invalid Index. ({ValueIndex})");
                        if (ValueIndex >= cmbInput.Items.Count)
                            ValueIndex = cmbInput.Items.Count - 1;
                        if (ValueIndex >= 0)
                            cmbInput.SelectedIndex = ValueIndex;
                        Grid.SetColumn(cmbInput, 1);
                        Grid.SetRow(cmbInput, 1);
                        cmbInput.SelectedIndex = ValueIndex;
                        return cmbInput;
                    }
                case InputMode.Prompt:
                    {
                        var Icon = LoadIcon();
                        if (Icon != null)
                            imgIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(
                                Icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                                BitmapSizeOptions.FromWidthAndHeight(100, 100));
                        else
                            imgIcon.Width = 0;
                        Grid.SetRowSpan(cmbInput, 5);
                        return imgIcon;
                    }
                default:
                    throw new Exception($"No Mode Selected Or Not Yet Seported. ({Type})");
            }
        }

        private Bitmap? LoadIcon()
        {
            switch (Icon)
            {
                case Icons.Info: return ExInputRes.Info_Icon;
                case Icons.Notify: return ExInputRes.Notifi_Icon;
                case Icons.Question: return ExInputRes.Question_Icon;
                case Icons.Warning: return ExInputRes.Warning_Icon;
                case Icons.Error: return ExInputRes.Error_Icon;
                case Icons.None: return null;
                default: return ExInputRes.None_Icon;
            }
        }
    }
}
