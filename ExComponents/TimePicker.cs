using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// Pick A Time An I'll Be There.
    /// </summary>
    public class TimePicker : Control, INotifyPropertyChanged
    {
        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
        }

        enum Focused
        {
            None,
            Hour,
            Min,
            Sec,
        }

        Focused FocusedComponent = Focused.None;
        DigitBox edtHour;
        DigitBox edtMin;
        DigitBox edtSec;
        Button btnUp;
        Button btnDown;
        Image imgUp;
        Image imgDown;

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            edtHour = (DigitBox)Template.FindName("PART_Hour", this);
            edtMin = (DigitBox)Template.FindName("PART_Min", this);
            edtSec = (DigitBox)Template.FindName("PART_Sec", this);
            btnUp = (Button)Template.FindName("PART_Up", this);
            btnDown = (Button)Template.FindName("PART_Down", this);
            imgUp = (Image)Template.FindName("PART_UpIcon", this);
            imgDown = (Image)Template.FindName("PART_DownIcon", this);

            imgUp.Source = Imaging.CreateBitmapSourceFromHBitmap(ExComponentsRes.Arrow_Up.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));
            imgDown.Source = Imaging.CreateBitmapSourceFromHBitmap(ExComponentsRes.Arrow_Down.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));

            edtHour.SetBinding(TextBox.TextProperty, new Binding(nameof(Hour)) { Source = this, Mode = BindingMode.TwoWay });
            edtMin.SetBinding(TextBox.TextProperty, new Binding(nameof(Minute)) { Source = this, Mode = BindingMode.TwoWay });
            edtSec.SetBinding(TextBox.TextProperty, new Binding(nameof(Seconds)) { Source = this, Mode = BindingMode.TwoWay });

            edtHour.GotFocus += SetFocus;
            edtMin.GotFocus += SetFocus;
            edtSec.GotFocus += SetFocus;
            btnUp.Click += AddTime;
            btnDown.Click += SubTime;

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Gets or sets the value of the Number Edit.
        /// </summary>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(TimeSpan), typeof(TimePicker),
                new PropertyMetadata(new TimeSpan(0, 0, 0), ValuePropertyChanged));

        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimePicker TimeEdit)
            {
                TimeEdit.RaiseEvent(new RoutedEventArgs(ValueChangedEvent, TimeEdit));
                TimeEdit.OnPropertyChanged(nameof(Value));
                TimeEdit.OnPropertyChanged(nameof(Hour));
                TimeEdit.OnPropertyChanged(nameof(Minute));
                TimeEdit.OnPropertyChanged(nameof(Seconds));
            }
        }

        /// <summary>
        /// is Read Only
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(TimePicker), new PropertyMetadata(false));

        /// <summary>
        /// Event For When The Value Changes
        /// </summary>
        public static RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(nameof(ValueChanged),
            RoutingStrategy.Bubble, typeof(EventHandler), typeof(TimePicker));

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
        /// Gets or sets the time value.
        /// </summary>
        [Description("Display Value"), Category("Common")]
        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Is Read Only
        /// </summary>
        [Description("Gets or sets a value that indicates whether the user can edit the value property."), Category("Miscellaneous")]
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); OnPropertyChanged(nameof(IsReadOnly)); }
        }

        private void SetFocus(object sender, RoutedEventArgs e)
        {
            switch(((DigitBox)sender).Name)
            {
                case "PART_Hour": FocusedComponent = Focused.Hour; break;
                case "PART_Min": FocusedComponent = Focused.Min; break;
                case "PART_Sec": FocusedComponent = Focused.Sec; break;
                default: FocusedComponent = Focused.None; break;
            }
        }

        private void AddTime(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly) return;
            switch (FocusedComponent)
            {
                case Focused.Hour: Hour = (int.Parse(Hour) + 1).ToString(); break;
                case Focused.Min: Minute = (int.Parse(Minute) + 1).ToString(); break;
                case Focused.Sec: Seconds = (int.Parse(Seconds) + 1).ToString(); break;
            }
        }

        private void SubTime(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly) return;
            switch (FocusedComponent)
            {
                case Focused.Hour: Hour = (int.Parse(Hour) - 1).ToString(); break;
                case Focused.Min: Minute = (int.Parse(Minute) - 1).ToString(); break;
                case Focused.Sec: Seconds = (int.Parse(Seconds) - 1).ToString(); break;
            }
        }

        /// <summary>
        /// Hour Value
        /// </summary>
        public string Hour
        {
            get
            {
                string H = Value.Hours.ToString();
                switch(H.Length)
                {
                    case 0: return "00";
                    case 1: return "0" + H;
                    case 2: return H;
                    default: return H.Substring(2);
                }
            }
            set
            {
                if (int.TryParse(value, out int H))
                {
                    if(H < 0) H =0;
                    Value = new TimeSpan(H, Value.Minutes, Value.Seconds);
                }
                else throw new Exception(value + " : Is Not A Valid Intager");
                OnPropertyChanged(nameof(Hour));
            }
        }
        /// <summary>
        /// Minute Value
        /// </summary>
        public string Minute
        {
            get
            {
                string M = Value.Minutes.ToString();
                switch (M.Length)
                {
                    case 0: return "00";
                    case 1: return "0" + M;
                    case 2: return M;
                    default: return M.Substring(2);
                }
            }
            set
            {
                if (int.TryParse(value, out int M))
                {
                    if (M < 0) M = 0;
                    Value = new TimeSpan(Value.Hours, M, Value.Seconds);
                }
                else throw new Exception(value + " : Is Not A Valid Intager");
                OnPropertyChanged(nameof(Minute));
            }
        }
        /// <summary>
        /// Second Value
        /// </summary>
        public string Seconds
        {
            get
            {
                string S = Value.Seconds.ToString();
                switch (S.Length)
                {
                    case 0: return "00";
                    case 1: return "0" + S;
                    case 2: return S;
                    default: return S.Substring(2);
                }
            }
            set
            {
                if (int.TryParse(value, out int S))
                {
                    if (S < 0) S = 0;
                    Value = new TimeSpan(Value.Hours, Value.Minutes, S);
                }
                else throw new Exception(value + " : Is Not A Valid Intager");
                OnPropertyChanged(nameof(Seconds));
            }
        }
    }
}
