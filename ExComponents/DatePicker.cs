using ExtraFunctions.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static ExtraFunctions.ExComponents.Week;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// New And Better Date Picker
    /// </summary>
    [System.Drawing.ToolboxBitmap(typeof(DatePicker),"DatePicker.bmp")]
    public class DatePicker : Control, INotifyPropertyChanged
    {
        static DatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatePicker), new FrameworkPropertyMetadata(typeof(DatePicker)));
        }

        TextBox edtDate;
        ImageButton btnSelector;

        Popup Popup;
        TextBlock lblDisplay;

        enum DisplayMode
        {
            Week,
            Month,
            Year,
            Decade,
        }

        DisplayMode Mode = DisplayMode.Week;
        List<Week> DayGrid { get; set; } = new List<Week>();

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            edtDate = (TextBox)Template.FindName("PART_Date", this);
            btnSelector = (ImageButton)Template.FindName("PART_Selector", this);

            Popup = (Popup)Template.FindName("PART_Popup", this);
            var SubMonth = (Border)Template.FindName("PART_Sub", this);
            var AddMonth = (Border)Template.FindName("PART_Add", this);
            lblDisplay = (TextBlock)Template.FindName("PART_DateDisplay", this);
            var GridDisplay = (Grid)Template.FindName("PART_Grid", this);
            var lblTodayDate = (TextBlock)Template.FindName("PART_TodayDate", this);

            btnSelector.Source = Imaging.CreateBitmapSourceFromHBitmap(ExComponentsRes.Calendar.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));

            Popup.StaysOpen = false;

            ExComponents.Month.SelectCell = SelectMonth;
            ExComponents.Year.SelectCell = SelectYear;

            GridDisplay.Children.Add(Week.WeekGrid);
            GridDisplay.Children.Add(ExComponents.Month.MonthGrid);
            GridDisplay.Children.Add(ExComponents.Year.YearGrid);

            edtDate.SetBinding(TextBox.TextProperty, new Binding(nameof(Value))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                Converter = new ExConverter.DateToStringConverter(),
            });

            lblDisplay.Text = ExFun.ToMonth(Value, true) + " " + Value.Year;
            lblTodayDate.MouseDown += (sender, e) => { Value = DateTime.Today; Popup.IsOpen = false; };
            SubMonth.MouseDown += Sub;
            void Sub(object sender = null,MouseButtonEventArgs e = null)
            {
                switch (Mode)
                {
                    case DisplayMode.Week: Value = Value.AddMonths(-1); break;
                    case DisplayMode.Month: Value = Value.AddYears(-1); break;
                    case DisplayMode.Year: Value = Value.AddYears(-10); break;
                }
                Update();
            }
            AddMonth.MouseDown += Add;
            void Add(object sender = null, MouseButtonEventArgs e = null)
            {
                switch (Mode)
                {
                    case DisplayMode.Week: Value = Value.AddMonths(1); break;
                    case DisplayMode.Month: Value = Value.AddYears(1); break;
                    case DisplayMode.Year: Value = Value.AddYears(10); break;
                }
                Update();
            }
            lblDisplay.MouseDown += (sender, e) =>
            {
                if (Mode == DisplayMode.Year) return;
                Mode = (DisplayMode)((int)Mode + 1);

                Update();
            };
            btnSelector.Click += (sender, e) => { if (!Popup.IsOpen) Reset(); Popup.IsOpen = !Popup.IsOpen; };

            SetDayGrid();
            Update();

            edtDate.AddHandler(PreviewMouseLeftButtonDownEvent,
              new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            edtDate.AddHandler(GotKeyboardFocusEvent,
              new RoutedEventHandler(SelectAllText), true);
            edtDate.AddHandler(MouseDoubleClickEvent,
              new RoutedEventHandler(SelectAllText), true);

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Gets or sets the value of the Number Edit.
        /// </summary>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(DateTime), typeof(DatePicker),
                new PropertyMetadata(DateTime.Today, ValuePropertyChanged));

        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatePicker DateEdit)
            {
                DateEdit.RaiseEvent(new RoutedEventArgs(ValueChangedEvent, DateEdit));
                DateEdit.OnPropertyChanged(nameof(Value));
                DateEdit.SetDayGrid();

                if (DateEdit.lblDisplay != null)
                    DateEdit.lblDisplay.Text = ExFun.ToMonth(DateEdit.Value, true) + " " + DateEdit.Value.Year;
            }
        }

        /// <summary>
        /// is Read Only
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(DatePicker), new PropertyMetadata(false));

        /// <summary>
        /// Event For When The Value Changes
        /// </summary>
        public static RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(nameof(ValueChanged),
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DatePicker));

        /// <summary>
        /// Event When The Value Changes
        /// </summary>
        public event RoutedEventHandler ValueChanged
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
        public DateTime Value
        {
            get { return (DateTime)GetValue(ValueProperty); }
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

        /// <summary>
        /// Year Value
        /// </summary>
        public int Year { get { return Value.Year; } set { Value = new DateTime(value, Value.Month, Value.Day); } }
        /// <summary>
        /// Month Value
        /// </summary>
        public int Month { get { return Value.Month; } set { Value = new DateTime(Value.Year, value, Value.Day); } }
        /// <summary>
        /// Day Value
        /// </summary>
        public int Day { get { return Value.Day; } set { Value = new DateTime(Value.Year, Value.Month, value); } }

        void Reset()
        {
            Mode = DisplayMode.Week;
            Update();
        }

        void Update()
        {
            DayGrid.ForEach(x => x.Select(Value.Day, Week.Tag.This));
            ExComponents.Month.Select(Value.Month);
            ExComponents.Year.Select(Value.Year);

            ExComponents.Year.Min = int.Parse(Value.Year.ToString().Substring(0, Value.Year.ToString().Length - 1) + "0");

            lblDisplay.Text = Mode == DisplayMode.Week ? ExFun.ToMonth(Value, true) + " " + Value.Year :
                                Mode == DisplayMode.Month ? Value.Year.ToString() :
                                ExComponents.Year.Min + "-" + (ExComponents.Year.Min + 9);

            WeekGrid.IsEnabled = Mode == DisplayMode.Week;
            ExComponents.Month.MonthGrid.IsEnabled = Mode == DisplayMode.Month;
            ExComponents.Year.YearGrid.IsEnabled = Mode == DisplayMode.Year;
        }

        void SetDayGrid()
        {
            if (Week.WeekGrid != null && DayGrid.Count > 0)
                DayGrid.ForEach(x => x.ToList().ForEach(X => Week.WeekGrid.Children.Remove(X)));
            DayGrid.Clear();
            var M = new DateTime(Value.Year, Value.Month, 1);
            if (M.DayOfWeek == DayOfWeek.Sunday) M = M.AddDays(-7);
            M = M.AddDays((int)M.DayOfWeek * -1);

            for (int J = 1; J < 7; J++)
            {
                var W = new Week() { SelectCell = SelectDay };
                for (int I = 0; I < 7; I++)
                {
                    var T = M == DateTime.Today ? Week.Tag.Today : M.Month == Value.Month ? Week.Tag.This :
                        (M.Month > Value.Month && !(M.Month == 12 && Value.Month == 1)) || (Value.Month == 12 && M.Month == 1) ?
                        Week.Tag.Next : Week.Tag.Pre;
                    W.Add(M.Day, M.DayOfWeek, J, T);
                    M = M.AddDays(1);
                }
                DayGrid.Add(W);
            }

            if (Week.WeekGrid != null)
                DayGrid.ForEach(x => x.ToList().ForEach(X => Week.WeekGrid.Children.Add(X)));

            OnPropertyChanged(nameof(DayGrid));
        }

        private void SelectDay(object sender, MouseButtonEventArgs e)
        {
            var lblCell = (TextBlock)sender;
            var I = int.Parse(lblCell.Text);
            var Tag = (Week.Tag)(lblCell.Tag ?? Week.Tag.This);
            var Ofset = 0;
            switch (Tag)
            {
                case Week.Tag.Pre: Ofset = -1; break;
                case Week.Tag.Next: Ofset = 1; break;
            }

            if (Value.Month + Ofset <= 0)
                Value = new DateTime(Value.Year - 1, 12, I);
            else
            if (Value.Month + Ofset > 12)
                Value = new DateTime(Value.Year + 1, 1, I);
            else
                Value = new DateTime(Value.Year, Value.Month + Ofset, I);

            Popup.IsOpen = false;
        }

        private void SelectMonth(object sender, MouseButtonEventArgs e)
        {
            var lblCell = (TextBlock)sender;
            var I = ExComponents.Month.Find(lblCell.Text);
            if(I < 0) return;

            Value = new DateTime(Value.Year, I, 1);

            Mode = DisplayMode.Week;
            Update();
        }

        private void SelectYear(object sender, MouseButtonEventArgs e)
        {
            var lblCell = (TextBlock)sender;
            var I = ExComponents.Year.Find(lblCell.Text);
            if (I < 0) return;

            Value = new DateTime(I, Value.Month, 1);

            Mode = DisplayMode.Month;
            Update();
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

    internal class Week
    {
        public static Grid WeekGrid = new Grid() { Margin = new Thickness(5, 0, 10, 0) };

        static Week()
        {
            WeekGrid.Children.Clear();
            List<TextBlock> Headers = new List<TextBlock>() {
                new TextBlock() { Text = "Sun", TextAlignment = TextAlignment.Center, },
                new TextBlock() { Text = "Mon", TextAlignment = TextAlignment.Center, },
                new TextBlock() { Text = "Tue", TextAlignment = TextAlignment.Center, },
                new TextBlock() { Text = "Wed", TextAlignment = TextAlignment.Center, },
                new TextBlock() { Text = "Thu", TextAlignment = TextAlignment.Center, },
                new TextBlock() { Text = "Fri", TextAlignment = TextAlignment.Center, },
                new TextBlock() { Text = "Sat", TextAlignment = TextAlignment.Center, },
            };

            var GridStyle = new Style(typeof(Grid));
            var GridTriger = new Trigger() { Property = UIElement.IsEnabledProperty, Value = false };
            var StartAni = new Storyboard();
            var Ani = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(.5)));
            Ani.Completed += (sender, e) => Panel.SetZIndex(WeekGrid, -1);
            Storyboard.SetTargetProperty(Ani, new PropertyPath(UIElement.OpacityProperty));
            StartAni.Children.Add(Ani);
            GridTriger.EnterActions.Add(new BeginStoryboard() { Storyboard = StartAni });
            var EndAni = new Storyboard();
            var Ani2 = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(.5)));
            Ani2.Completed += (sender, e) => Panel.SetZIndex(WeekGrid, 1);
            Storyboard.SetTargetProperty(Ani2, new PropertyPath(UIElement.OpacityProperty));
            EndAni.Children.Add(Ani2);
            GridTriger.ExitActions.Add(new BeginStoryboard() { Storyboard = EndAni });
            GridStyle.Triggers.Add(GridTriger);
            WeekGrid.Style = GridStyle;

            for (int I = 0; I < 7; I++)
            {
                WeekGrid.RowDefinitions.Add(new RowDefinition());
                WeekGrid.ColumnDefinitions.Add(new ColumnDefinition()); 
                Grid.SetColumn(Headers[I], I);
                WeekGrid.Children.Add(Headers[I]);
            }

            var GridBorder = new Border() { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0, 0, 0, 1) };
            Grid.SetColumnSpan(GridBorder, 7);
            WeekGrid.Children.Add(GridBorder);
        }

        public TextBlock Sun { get; private set; } = new TextBlock() { TextAlignment = TextAlignment.Right, };
        public TextBlock Mon { get; private set; } = new TextBlock() { TextAlignment = TextAlignment.Right, };
        public TextBlock Tue { get; private set; } = new TextBlock() { TextAlignment = TextAlignment.Right, };
        public TextBlock Wed { get; private set; } = new TextBlock() { TextAlignment = TextAlignment.Right, };
        public TextBlock Thu { get; private set; } = new TextBlock() { TextAlignment = TextAlignment.Right, };
        public TextBlock Fri { get; private set; } = new TextBlock() { TextAlignment = TextAlignment.Right, };
        public TextBlock Sat { get; private set; } = new TextBlock() { TextAlignment = TextAlignment.Right, };

        internal MouseButtonEventHandler SelectCell = (sender, e) => { };

        public enum Tag
        {
            Today,
            This,
            Next,
            Pre,
        }

        public void Add(int Day, DayOfWeek DayOfWeek, int Row, Tag Tag)
        {
            var Colour = Tag == Tag.This ? Brushes.Black : Brushes.Gray;
            if (Tag == Tag.Today) { Tag = Tag.This; Colour = Brushes.Blue; }

            switch (DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    Grid.SetColumn(Sun, 0);
                    Grid.SetRow(Sun, Row);
                    Sun.Text = Day.ToString();
                    Sun.Foreground = Colour;
                    Sun.Tag = Tag;
                    Sun.MouseLeftButtonDown += SelectCell;
                    break;
                case DayOfWeek.Monday:
                    Grid.SetColumn(Mon, 1);
                    Grid.SetRow(Mon, Row);
                    Mon.Text = Day.ToString();
                    Mon.Foreground = Colour;
                    Mon.Tag = Tag;
                    Mon.MouseLeftButtonDown += SelectCell;
                    break;
                case DayOfWeek.Tuesday:
                    Grid.SetColumn(Tue, 2);
                    Grid.SetRow(Tue, Row);
                    Tue.Text = Day.ToString();
                    Tue.Foreground = Colour;
                    Tue.Tag = Tag;
                    Tue.MouseLeftButtonDown += SelectCell;
                    break;
                case DayOfWeek.Wednesday:
                    Grid.SetColumn(Wed, 3);
                    Grid.SetRow(Wed, Row);
                    Wed.Text = Day.ToString();
                    Wed.Foreground = Colour;
                    Wed.Tag = Tag;
                    Wed.MouseLeftButtonDown += SelectCell;
                    break;
                case DayOfWeek.Thursday:
                    Grid.SetColumn(Thu, 4);
                    Grid.SetRow(Thu, Row);
                    Thu.Text = Day.ToString();
                    Thu.Foreground = Colour;
                    Thu.Tag = Tag;
                    Thu.MouseLeftButtonDown += SelectCell;
                    break;
                case DayOfWeek.Friday:
                    Grid.SetColumn(Fri, 5);
                    Grid.SetRow(Fri, Row);
                    Fri.Text = Day.ToString();
                    Fri.Foreground = Colour;
                    Fri.Tag = Tag;
                    Fri.MouseLeftButtonDown += SelectCell;
                    break;
                case DayOfWeek.Saturday:
                    Grid.SetColumn(Sat, 6);
                    Grid.SetRow(Sat, Row);
                    Sat.Text = Day.ToString();
                    Sat.Foreground = Colour;
                    Sat.Tag = Tag;
                    Sat.MouseLeftButtonDown += SelectCell;
                    break;
            }
        }

        public void Select(int Day, Tag Tag)
        {
            ToList().ForEach(x => x.Background = Brushes.White);
            (ToList().FirstOrDefault(x => x.Text == Day.ToString() && (Tag)(x.Tag ?? Tag.This) == Tag) ??
                new TextBlock()).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cce8ff"));
        }

        public List<TextBlock> ToList() => new List<TextBlock>() { Sun, Mon, Tue, Wed, Thu, Fri, Sat };
    }

    internal class Month
    {
        public static Grid MonthGrid = new Grid() { Margin = new Thickness(0, 15, 0, 0), IsEnabled = false };
        internal static MouseButtonEventHandler SelectCell { set { Values.ForEach(x => x.MouseDown += value); } }

        static List<TextBlock> Values = new List<TextBlock>() {
            new TextBlock() { Text = "Jan", TextAlignment = TextAlignment.Center, },
            new TextBlock() { Text = "Feb", TextAlignment = TextAlignment.Center, },
            new TextBlock() { Text = "Mar", TextAlignment = TextAlignment.Center, },
            new TextBlock() { Text = "Apr", TextAlignment = TextAlignment.Center, },
            new TextBlock() { Text = "May", TextAlignment = TextAlignment.Center, },
            new TextBlock() { Text = "Jun", TextAlignment = TextAlignment.Center, },
            new TextBlock() { Text = "Jul", TextAlignment = TextAlignment.Center, },
            new TextBlock() { Text = "Aug", TextAlignment = TextAlignment.Center, },
            new TextBlock() { Text = "Sep", TextAlignment = TextAlignment.Center, },
            new TextBlock() { Text = "Oct", TextAlignment = TextAlignment.Center, },
            new TextBlock() { Text = "Nov", TextAlignment = TextAlignment.Center, },
            new TextBlock() { Text = "Dec", TextAlignment = TextAlignment.Center, },
        };

        static Month()
        {
            MonthGrid.Children.Clear();
            var GridStyle = new Style(typeof(Grid));
            var GridTriger = new Trigger() { Property = UIElement.IsEnabledProperty, Value = false };
            var StartAni = new Storyboard();
            var Ani = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(.5))); 
            Ani.Completed += (sender, e) => Panel.SetZIndex(MonthGrid, -1);
            Storyboard.SetTargetProperty(Ani, new PropertyPath(UIElement.OpacityProperty));
            StartAni.Children.Add(Ani);
            GridTriger.EnterActions.Add(new BeginStoryboard() { Storyboard = StartAni });
            var EndAni = new Storyboard();
            var Ani2 = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(.5)));           
            Ani2.Completed += (sender, e) => Panel.SetZIndex(MonthGrid, 1);
            Storyboard.SetTargetProperty(Ani2, new PropertyPath(UIElement.OpacityProperty));
            EndAni.Children.Add(Ani2);
            GridTriger.ExitActions.Add(new BeginStoryboard() { Storyboard = EndAni });
            GridStyle.Triggers.Add(GridTriger);
            MonthGrid.Style = GridStyle;

            var L = -1;
            for (int I = 0; I < 4; I++) MonthGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int I = 0; I < 3; I++)
            {
                MonthGrid.RowDefinitions.Add(new RowDefinition());
                for (int J = 0; J < 4; J++)
                {
                    L++;
                    Grid.SetColumn(Values[L], J);
                    Grid.SetRow(Values[L], I);
                    MonthGrid.Children.Add(Values[L]);
                }
            }
        }

        public static int Find(string Text)
        {
            var edt = Values.FirstOrDefault(x => x.Text == Text);
            if (edt == null) return -1;
            return Values.IndexOf(edt) + 1;
        }

        public static void Select(int Month)
        {
            Values.ForEach(x => x.Background = Brushes.White);
            (Values.FirstOrDefault(x => x.Text == ExFun.ToMonth(Month)) ??
                new TextBlock()).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cce8ff"));
        }
    }

    internal class Year
    {
        public static Grid YearGrid = new Grid() { Margin = new Thickness(0, 15, 0, 0), IsEnabled = false };
        internal static MouseButtonEventHandler SelectCell { set { Values.ForEach(x => x.MouseDown += value); } }
        public static int Min { 
            get { return int.Parse(Values[1].Text); }
            set
            {
                var L = value - 2;
                for (int I = 0; I < 12; I++)
                {
                    L++;
                    Values[I].Text = L.ToString();
                    if (L <= value - 1 || L >= value + 10) Values[I].Foreground = Brushes.Gray;
                }
            }
        }

        static List<TextBlock> Values = new List<TextBlock>() {
            new TextBlock() { TextAlignment = TextAlignment.Center, },
            new TextBlock() { TextAlignment = TextAlignment.Center, },
            new TextBlock() { TextAlignment = TextAlignment.Center, },
            new TextBlock() { TextAlignment = TextAlignment.Center, },
            new TextBlock() { TextAlignment = TextAlignment.Center, },
            new TextBlock() { TextAlignment = TextAlignment.Center, },
            new TextBlock() { TextAlignment = TextAlignment.Center, },
            new TextBlock() { TextAlignment = TextAlignment.Center, },
            new TextBlock() { TextAlignment = TextAlignment.Center, },
            new TextBlock() { TextAlignment = TextAlignment.Center, },
            new TextBlock() { TextAlignment = TextAlignment.Center, },
            new TextBlock() { TextAlignment = TextAlignment.Center, },
        };

        static Year()
        {
            YearGrid.Children.Clear();
            var GridStyle = new Style(typeof(Grid));
            var GridTriger = new Trigger() { Property = UIElement.IsEnabledProperty, Value = false };
            var StartAni = new Storyboard();
            var Ani = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(.5)));
            Ani.Completed += (sender, e) => Panel.SetZIndex(YearGrid, -1);
            Storyboard.SetTargetProperty(Ani, new PropertyPath(UIElement.OpacityProperty));
            StartAni.Children.Add(Ani);
            GridTriger.EnterActions.Add(new BeginStoryboard() { Storyboard = StartAni });
            var EndAni = new Storyboard();
            var Ani2 = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(.5)));
            Ani2.Completed += (sender, e) => Panel.SetZIndex(YearGrid, 1);
            Storyboard.SetTargetProperty(Ani2, new PropertyPath(UIElement.OpacityProperty));
            EndAni.Children.Add(Ani2);
            GridTriger.ExitActions.Add(new BeginStoryboard() { Storyboard = EndAni });
            GridStyle.Triggers.Add(GridTriger);
            YearGrid.Style = GridStyle;

            var L = -1;
            for (int I = 0; I < 4; I++) YearGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int I = 0; I < 3; I++)
            {
                YearGrid.RowDefinitions.Add(new RowDefinition());
                for (int J = 0; J < 4; J++)
                {
                    L++;
                    Grid.SetColumn(Values[L], J);
                    Grid.SetRow(Values[L], I);
                    YearGrid.Children.Add(Values[L]);
                }
            }
        }

        public static int Find(string Text)
        {
            var edt = Values.FirstOrDefault(x => x.Text == Text);
            if (edt == null) return -1;
            return int.Parse(edt.Text);
        }

        public static void Select(int Year)
        {
            Values.ForEach(x => x.Background = Brushes.White);
            (Values.FirstOrDefault(x => x.Text == Year.ToString()) ??
                new TextBlock()).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cce8ff"));
        }
    }
}
