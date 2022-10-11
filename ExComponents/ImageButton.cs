using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// A Simple Markup Button That Supports An Image
    /// </summary>
    public class ImageButton : Control, INotifyPropertyChanged
    {
        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            PreviewMouseLeftButtonUp += (s, e) => RaiseEvent(new RoutedEventArgs(ClickEvent, this));

            var pnlDock = (Border)Template.FindName("pnlBack", this);
            pnlDock.MouseEnter += (sender, e) =>
            {
                if (sender == null) return;
                pnlDock.Background = new SolidColorBrush(Color.FromRgb(230, 240, 250));
                pnlDock.BorderBrush = new SolidColorBrush(Color.FromRgb(160, 200, 240));
            };
            pnlDock.MouseLeave += (sender, e) =>
            {
                pnlDock.Background = Brushes.Transparent;
                pnlDock.BorderBrush = Brushes.Transparent;
            };

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Get Or Set The Text To Display Next To The Image
        /// </summary>
        public static DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ImageButton));
        /// <summary>
        /// Get Or Set The Image Source
        /// </summary>
        public static DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(ImageButton));

        /// <summary>
        /// Get Or Set Width Of The Image
        /// </summary>
        public static DependencyProperty ImageWidthProperty =
            DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(ImageButton),
                new PropertyMetadata(-1.0));
        /// <summary>
        /// Get Or Set Hight Of The Image
        /// </summary>
        public static DependencyProperty ImageHightProperty =
            DependencyProperty.Register(nameof(ImageHight), typeof(double), typeof(ImageButton),
                new PropertyMetadata(-1.0));

        /// <summary>
        /// Get Or Set The Image Alighment
        /// </summary>
        public static DependencyProperty IconAlignmentProperty =
            DependencyProperty.Register(nameof(IconAlignment), typeof(Dock), typeof(ImageButton),
                new PropertyMetadata(Dock.Left));
        /// <summary>
        /// Get Or Set The General Alignment
        /// </summary>
        public static DependencyProperty ContentAlignmentProperty =
            DependencyProperty.Register(nameof(ContentAlignment), typeof(HorizontalAlignment), typeof(ImageButton),
                new PropertyMetadata(HorizontalAlignment.Left));
        /// <summary>
        /// Get Or Set The Alignment Of The Text
        /// </summary>
        public static DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(nameof(TextAlignment), typeof(HorizontalAlignment), typeof(ImageButton),
                new PropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// On Click Event
        /// </summary>
        public static RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble,
            typeof(EventHandler), typeof(ImageButton));

        /// <summary>
        /// On Click Event
        /// </summary>
        public event EventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// Event When A Property Changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Exicutes Property Change Event 
        /// </summary>
        protected void OnPropertyChanged(string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// Get Or Set The Text To Display Next To The Image
        /// </summary>
        [Description("Display Text"), Category("Common")]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        /// Get Or Set The Image Source
        /// </summary>
        [Description("Display Image"), Category("Common")]
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); OnPropertyChanged(nameof(Source)); }
        }

        /// <summary>
        /// Get Or Set Width Of The Image
        /// </summary>
        [Description("Image Width (-1 = Auto)"), Category("Layout")]
        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); OnPropertyChanged(nameof(ImageWidth)); }
        }
        /// <summary>
        /// Get Or Set Hight Of The Image
        /// </summary>
        [Description("Image Hight (-1 = Auto)"), Category("Layout")]
        public double ImageHight
        {
            get { return (double)GetValue(ImageHightProperty); }
            set { SetValue(ImageHightProperty, value); OnPropertyChanged(nameof(ImageHight)); }
        }

        /// <summary>
        /// Get Or Set The Image Alighment
        /// </summary>
        [Description("Alignment Of The Icon"), Category("Layout")]
        public Dock IconAlignment
        {
            get { return (Dock)GetValue(IconAlignmentProperty); }
            set { SetValue(IconAlignmentProperty, value); OnPropertyChanged(nameof(IconAlignment)); }
        }
        /// <summary>
        /// Get Or Set The General Alignment
        /// </summary>
        [Description("Alignment Of Text And Icon"), Category("Layout")]
        public HorizontalAlignment ContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(ContentAlignmentProperty); }
            set { SetValue(ContentAlignmentProperty, value); OnPropertyChanged(nameof(ContentAlignment)); }
        }
        /// <summary>
        /// Get Or Set The Alignment Of The Text
        /// </summary>
        /// <summary>
        /// The Alignment Of The Value
        /// </summary>
        [Description("Alignment Of The Text"), Category("Layout")]
        public HorizontalAlignment TextAlignment
        {
            get { return (HorizontalAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); OnPropertyChanged(nameof(TextAlignment)); }
        }
    }
}
