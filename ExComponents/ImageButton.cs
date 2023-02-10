using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        Image imgIcon;

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            PreviewMouseLeftButtonUp += (s, e) => RaiseEvent(new RoutedEventArgs(ClickEvent, this));
            KeyDown += EnterKeyClick;

            imgIcon = (Image)Template.FindName("PART_Icon", this);
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

            switch (ImageAlignment)
            {
                case Extras.ImageAlignment.Center:
                    Grid.SetColumn(imgIcon, 1);
                    Grid.SetRow(imgIcon, 1);
                    break;
                case Extras.ImageAlignment.Top:
                    Grid.SetColumn(imgIcon, 1);
                    Grid.SetRow(imgIcon, 0);
                    break;
                case Extras.ImageAlignment.Bottom:
                    Grid.SetColumn(imgIcon, 1);
                    Grid.SetRow(imgIcon, 2);
                    break;
                case Extras.ImageAlignment.Left:
                    Grid.SetColumn(imgIcon, 0);
                    Grid.SetRow(imgIcon, 1);
                    break;
                case Extras.ImageAlignment.Right:
                    Grid.SetColumn(imgIcon, 2);
                    Grid.SetRow(imgIcon, 1);
                    break;
            }

            base.OnApplyTemplate();
            void EnterKeyClick(object sender, KeyEventArgs e)
            {
                if (e.Key != Key.Return) return;
                RaiseEvent(new RoutedEventArgs(ClickEvent, this));
            }
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
        public static DependencyProperty ImageAlignmentProperty =
            DependencyProperty.Register(nameof(ImageAlignment), typeof(Extras.ImageAlignment), typeof(ImageButton),
                new PropertyMetadata(Extras.ImageAlignment.Left, ImageAlignmentPropertyChanged));

        private static void ImageAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageButton btnImage && btnImage.imgIcon != null)
            {
                switch (btnImage.ImageAlignment)
                {
                    case Extras.ImageAlignment.Center:
                        Grid.SetColumn(btnImage.imgIcon, 1);
                        Grid.SetRow(btnImage.imgIcon, 1);
                        break;
                    case Extras.ImageAlignment.Top:
                        Grid.SetColumn(btnImage.imgIcon, 1);
                        Grid.SetRow(btnImage.imgIcon, 0);
                        break;
                    case Extras.ImageAlignment.Bottom:
                        Grid.SetColumn(btnImage.imgIcon, 1);
                        Grid.SetRow(btnImage.imgIcon, 2);
                        break;
                    case Extras.ImageAlignment.Left:
                        Grid.SetColumn(btnImage.imgIcon, 0);
                        Grid.SetRow(btnImage.imgIcon, 1);
                        break;
                    case Extras.ImageAlignment.Right:
                        Grid.SetColumn(btnImage.imgIcon, 2);
                        Grid.SetRow(btnImage.imgIcon, 1);
                        break;
                }
                btnImage.OnPropertyChanged(nameof(ImageAlignment));
            }
        }

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
            typeof(RoutedEventHandler), typeof(ImageButton));

        /// <summary>
        /// On Click Event
        /// </summary>
        public event RoutedEventHandler Click
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
        [Description("Alignment Of The Image"), Category("Layout")]
        public Extras.ImageAlignment ImageAlignment
        {
            get { return (Extras.ImageAlignment)GetValue(ImageAlignmentProperty); }
            set { SetValue(ImageAlignmentProperty, value); }
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
