using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ExtraFunctions.ExComponents
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

        private DigitBox edtValue;
        private Button btnUp;
        private Button btnDown;
        private Image imgUp;
        private Image imgDown;

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
            btnUp.SetBinding(IsEnabledProperty, new Binding(nameof(UpEnabled)) { Source = this });
            btnDown.SetBinding(IsEnabledProperty, new Binding(nameof(DownEnabled)) { Source = this });

            imgUp.Source = Imaging.CreateBitmapSourceFromHBitmap( ExComponents.ExComponentsRes.Arrow_Up.GetHbitmap(), 
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));
            imgDown.Source = Imaging.CreateBitmapSourceFromHBitmap(ExComponents.ExComponentsRes.Arrow_Down.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));

            btnUp.Click += UpClick;
            btnDown.Click += DownClick;
            edtValue.KeyUp += EnterValue;

            UpEnabled = Value < MaxValue;
            DownEnabled = Value > MinValue;
            if (Value > MaxValue) Value = MaxValue;
            if (Value < MinValue) Value = MinValue;
            imgUp.Opacity = UpEnabled ? 1 : .25;
            imgDown.Opacity = DownEnabled ? 1 : .25;

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Gets or sets the value of the Number Edit.
        /// </summary>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(NumericUpDown), 
                new PropertyMetadata(.0, ValuePropertyChanged));

        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumericUpDown NumEdit)
            {
                NumEdit.RaiseEvent(new RoutedEventArgs(ValueChangedEvent, NumEdit));
                NumEdit.OnPropertyChanged("Value");
                NumEdit.UpEnabled = (double)e.NewValue < NumEdit.MaxValue;
                NumEdit.DownEnabled = (double)e.NewValue > NumEdit.MinValue;
                if ((double)e.NewValue > NumEdit.MaxValue) NumEdit.Value = NumEdit.MaxValue;
                if ((double)e.NewValue < NumEdit.MinValue) NumEdit.Value = NumEdit.MinValue;
                if (NumEdit.imgUp == null || NumEdit.imgDown == null) return;
                NumEdit.imgUp.Opacity = NumEdit.UpEnabled ? 1 : .25;
                NumEdit.imgDown.Opacity = NumEdit.DownEnabled ? 1 : .25;
            }
        }
        
        /// <summary>
        /// Minimum Value
        /// </summary>
        public static DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(NumericUpDown), new PropertyMetadata(.0));
        /// <summary>
        /// Maximum Value
        /// </summary>
        public static DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(NumericUpDown), new PropertyMetadata(100.0));

        /// <summary>
        /// The Amount To Increase And Decrease With
        /// </summary>  
        public static DependencyProperty IncrementsProperty =
            DependencyProperty.Register(nameof(Increments), typeof(double), typeof(NumericUpDown), new PropertyMetadata(1.0));

        /// <summary>
        /// Whether The Increase Button Is Enabled
        /// </summary>
        public static DependencyProperty UpProperty =
            DependencyProperty.Register(nameof(UpEnabled), typeof(bool), typeof(NumericUpDown), new PropertyMetadata(true));
        /// <summary>
        /// Whether The Decrease Button Is Enabled
        /// </summary>
        public static DependencyProperty DownProperty =
            DependencyProperty.Register(nameof(DownEnabled), typeof(bool), typeof(NumericUpDown), new PropertyMetadata(false));

        /// <summary>
        /// The Alignment Of The Value
        /// </summary>
        public static DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(nameof(TextAlignment), typeof(TextAlignment), typeof(NumericUpDown),
                new PropertyMetadata(TextAlignment.Left));

        /// <summary>
        /// Event For When The Value Changes
        /// </summary>
        public static RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(nameof(ValueChanged), 
            RoutingStrategy.Bubble, typeof(EventHandler), typeof(NumericUpDown));

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
        [Description("Display Value"), Category("Common")]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value > MaxValue ? MaxValue : value < MinValue ? MinValue : value); }
        }
        /// <summary>
        /// Minimum Value
        /// </summary>
        [Description("Minimum Value"), Category("Common")]
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); OnPropertyChanged("MinValue"); }
        }
        /// <summary>
        /// Maximum Value
        /// </summary>
        [Description("Maximum Value"), Category("Common")]
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); OnPropertyChanged("MaxValue"); }
        }
        /// <summary>
        /// The Amount To Increase And Decrease With
        /// </summary>    
        [Description("Increase And Decrease The Value By"), Category("Common")]
        public double Increments
        {
            get { return (double)GetValue(IncrementsProperty); }
            set { SetValue(IncrementsProperty, value); OnPropertyChanged("Increments"); }
        }

        /// <summary>
        /// The Alignment Of The Value
        /// </summary>
        [Description("Alignment Of The Text"), Category("Text")]
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); OnPropertyChanged(nameof(TextAlignment)); }
        }

        /// <summary>
        /// Whether The Increase Button Is Enabled
        /// </summary>
        [Description("Increase Button Enabled"), Category("Automation")]
        public bool UpEnabled
        {
            get { return (bool)GetValue(UpProperty); }
            set { SetValue(UpProperty, value); OnPropertyChanged(nameof(UpEnabled)); }
        }
        /// <summary>
        /// Whether The Decrease Button Is Enabled
        /// </summary>
        [Description("Decrease Button Enabled"), Category("Automation")]
        public bool DownEnabled
        {
            get { return (bool)GetValue(DownProperty); }
            set { SetValue(DownProperty, value); OnPropertyChanged(nameof(DownEnabled)); }
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

        private void EnterValue(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Value = Convert.ToDouble(edtValue.Text);
        }
    }
}
