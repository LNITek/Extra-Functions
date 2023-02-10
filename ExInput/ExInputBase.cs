using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

namespace ExtraFunctions.ExInput
{
    /// <summary>
    /// A Base To Create A Extra Input Prompts
    /// </summary>
    public class ExInputBase : IExInput
    {
        /// <summary>
        /// Title Of The Prompt
        /// </summary>
        public string Title { get; set; } = "Extra Inputs";
        /// <summary>
        /// Prompt Message
        /// </summary>
        public string Message { get; set; } = "Hallo World!";
        /// <summary>
        /// List Of Custom Buttons
        /// </summary> 
        public List<BasicButton> lstButtons { get; set; } = new List<BasicButton>();
        
        /// <summary>
        /// A Returning Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Main Window
        /// </summary>
        public Window Main { get; set; } = new Window()
        {
            Background = Brushes.White,
            ShowInTaskbar = false,
            Width = 400,
            MinHeight = 100,
            ResizeMode = ResizeMode.NoResize,
            SizeToContent = SizeToContent.Height,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        /// <summary>
        /// Stack Panel That Contains The Buttons
        /// </summary>
        public DockPanel pnlBTN { get; set; } = new DockPanel()
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f0f0f0")),
            Margin = new Thickness(0, 10, 0, 0),
            VerticalAlignment = VerticalAlignment.Bottom,
        };
        /// <summary>
        /// The Message Component
        /// </summary>
        public TextBlock lblMessage { get; set; } = new TextBlock()
        {
            Margin = new Thickness(10),
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Top
        };
        /// <summary>
        /// Extra Component
        /// </summary>
        public UIElement ExComponent { get; set; } = new UIElement();
        /// <summary>
        /// A Grid For Structure
        /// </summary>
        public Grid GridX { get; set; } = new Grid();

        /// <summary>
        /// Shows The Prompt
        /// </summary>
        /// <returns>DialogResult Of The Button</returns>
        public bool Show()
        {
            Main.Title = Title;
            lblMessage.Text = Message;
            Grid.SetColumn(lblMessage, 1);

            pnlBTN.Children.Clear();
            lstButtons.Reverse();
            lstButtons.ForEach(x => { pnlBTN.Children.Add(x.Button); });
            Grid.SetColumnSpan(pnlBTN, 2);
            Grid.SetRow(pnlBTN, 2);

            GridX.Children.Clear();
            GridX.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            GridX.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            GridX.RowDefinitions.Add(new RowDefinition());
            GridX.RowDefinitions.Add(new RowDefinition());
            GridX.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            GridX.Children.Add(ExComponent);
            GridX.Children.Add(lblMessage);
            GridX.Children.Add(pnlBTN);

            Main.Content = GridX;

            return Main.ShowDialog() ?? false;
        }

        /// <summary>
        /// Show(): Creates A Large Empty Space On The Left Side.
        /// Set ExComponent To Slot A
        /// </summary>
        public void SetSlotA()
        {
            Grid.SetColumn(ExComponent, 0);
            Grid.SetRow(ExComponent, 0);
            Grid.SetRowSpan(ExComponent, 2);
        }
        /// <summary>
        /// Show(): Creates A Wide Empty Space Between The Message Prompt And The Buttons.
        /// Set ExComponent To Slot B
        /// </summary>
        public void SetSlotB()
        {
            Grid.SetColumn(ExComponent, 1);
            Grid.SetRow(ExComponent, 1);
        }
    }
}
