using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ExtraFunctions.Components
{
    /// <summary>
    /// A Numeric Text Box With Add And Subtract Buttons
    /// </summary>
    public class NumericUpDown : Control, INotifyPropertyChanged
    {
        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        #pragma warning disable CS8618
        private DigitBox edtValue;
        private Button btnUp;
        private Button btnDown;
        private Image imgUp;
        private Image imgDown;
        #pragma warning restore CS8618

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            edtValue = (DigitBox)Template.FindName("PART_Value", this);
            btnUp = (Button)Template.FindName("PART_Up", this);
            btnDown = (Button)Template.FindName("PART_Down", this);
            imgUp = (Image)Template.FindName("PART_UpIcon", this);
            imgDown = (Image)Template.FindName("PART_DownIcon", this);

            edtValue.SetBinding(TextBox.TextProperty, new Binding(nameof(Value)) { Source = this, Mode = BindingMode.TwoWay });
            edtValue.SetBinding(TextBox.TextAlignmentProperty, new Binding(nameof(TextAlignment)) { Source = this });
            btnUp.SetBinding(IsEnabledProperty, new Binding(nameof(UpEnabled)) { Source = this });
            btnDown.SetBinding(IsEnabledProperty, new Binding(nameof(DownEnabled)) { Source = this });

            imgUp.Source = Imaging.CreateBitmapSourceFromHBitmap( ExComponents.ExComponentsRes.Arrow_Up.GetHbitmap(), 
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));
            imgDown.Source = Imaging.CreateBitmapSourceFromHBitmap(ExComponents.ExComponentsRes.Arrow_Down.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));

            btnUp.Click += UpClick;
            btnDown.Click += DownClick;

            UpEnabled = Value < MaxValue;
            DownEnabled = Value > MinValue;
            imgUp.Opacity = UpEnabled ? 1 : .25;
            imgDown.Opacity = DownEnabled ? 1 : .25;

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Gets or sets the value of the Number Edit.
        /// </summary>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumericUpDown), 
                new PropertyMetadata(.0, ValuePropertyChanged));

        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumericUpDown NumEdit)
            {
                NumEdit.RaiseEvent(new RoutedEventArgs(ValueChangedEvent, NumEdit));
                NumEdit.OnPropertyChanged("Value");
                NumEdit.UpEnabled = (double)e.NewValue < NumEdit.MaxValue;
                NumEdit.DownEnabled = (double)e.NewValue > NumEdit.MinValue;
                try
                {
                    NumEdit.imgUp.Opacity = NumEdit.UpEnabled ? 1 : .25;
                    NumEdit.imgDown.Opacity = NumEdit.DownEnabled ? 1 : .25;
                }
                catch { }
            }
        }
        
        /// <summary>
        /// Minimum Value
        /// </summary>
        public static DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata(.0));
        /// <summary>
        /// Maximum Value
        /// </summary>
        public static DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata(100.0));

        /// <summary>
        /// The Amount To Increase And Decrease With
        /// </summary>  
        public static DependencyProperty IncrementsProperty =
            DependencyProperty.Register("Increments", typeof(double), typeof(NumericUpDown), new PropertyMetadata(1.0));

        /// <summary>
        /// Whether The Increase Button Is Enabled
        /// </summary>
        public static DependencyProperty UpProperty =
            DependencyProperty.Register("UpEnabled", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(true));
        /// <summary>
        /// Whether The Decrease Button Is Enabled
        /// </summary>
        public static DependencyProperty DownProperty =
            DependencyProperty.Register("DownEnabled", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(false));

        /// <summary>
        /// The Alignment Of The Value
        /// </summary>
        public static DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(NumericUpDown),
                new PropertyMetadata(TextAlignment.Left));

        /// <summary>
        /// Event For When The Value Changes
        /// </summary>
        public static RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChaged", RoutingStrategy.Bubble,
            typeof(EventHandler), typeof(NumericUpDown));

        /// <summary>
        /// Event When The Value Changes
        /// </summary>
        public event EventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
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
        /// Gets or sets the value of the Number Edit.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value > MaxValue ? MaxValue : value < MinValue ? MinValue : value); }
        }
        /// <summary>
        /// Minimum Value
        /// </summary>
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); OnPropertyChanged("MinValue"); }
        }
        /// <summary>
        /// Maximum Value
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); OnPropertyChanged("MaxValue"); }
        }
        /// <summary>
        /// The Amount To Increase And Decrease With
        /// </summary>    
        public double Increments
        {
            get { return (double)GetValue(IncrementsProperty); }
            set { SetValue(IncrementsProperty, value); OnPropertyChanged("Increments"); }
        }

        /// <summary>
        /// Wether The Increase Button Is Enabled
        /// </summary>
        public bool UpEnabled
        {
            get { return (bool)GetValue(UpProperty); }
            set { SetValue(UpProperty, value); OnPropertyChanged("UpEnabled"); }
        }
        /// <summary>
        /// Wether The Decrease Button Is Enabled
        /// </summary>
        public bool DownEnabled
        {
            get { return (bool)GetValue(DownProperty); }
            set { SetValue(DownProperty, value); OnPropertyChanged("DownEnabled"); }
        }

        /// <summary>
        /// The Alignment Of The Value
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); OnPropertyChanged("TextAlignment"); }
        }

        /// <summary>
        /// Reset The Value To 0
        /// </summary>
        public void Clear() => Value = 0;
        /// <summary>
        /// Selects A Range Of Text
        /// </summary>
        /// <param name="start">Starting Index</param>
        /// <param name="length">Abount Of Char</param>
        public void Select(int start, int length) => edtValue.Select(start, length);

        /// <summary>
        /// Increase The Value By The Increment
        /// </summary>
        public void Increase() => Value += Increments;
        /// <summary>
        /// Decrease The Value By The Increment
        /// </summary>
        public void Decrease() => Value -= Increments;

        private void UpClick(object sender, EventArgs e) => Increase();
        private void DownClick(object sender, EventArgs e) => Decrease();
    }
}
