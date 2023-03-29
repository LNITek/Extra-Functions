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
using static ExtraFunctions.ExComponents.Day;
using static ExtraFunctions.ExComponents.Week;
using static ExtraFunctions.ExComponents.Month;
using static ExtraFunctions.ExComponents.Year;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// New And Better Date Picker
    /// </summary>
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
        List<Day> DayGrid { get; set; } = new List<Day>();
        Week WeekGrid = new Week();
        Month MonthGrid = new Month();
        Year YearGrid = new Year();

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

            MonthGrid.SelectCell = SelectMonth;
            YearGrid.SelectCell = SelectYear;

            GridDisplay.Children.Add(WeekGrid.WeekGrid);
            GridDisplay.Children.Add(MonthGrid.MonthGrid);
            GridDisplay.Children.Add(YearGrid.YearGrid);

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
              new RoutedEventHandler(SelectAllContent), true);
            edtDate.AddHandler(MouseDoubleClickEvent,
              new RoutedEventHandler(SelectAllContent), true);

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
            DayGrid.ForEach(x => x.Select(Value.Day, ExComponents.Day.Tag.This));
            MonthGrid.Select(Value.Month);
            YearGrid.Select(Value.Year);

            YearGrid.Min = int.Parse(Value.Year.ToString().Substring(0, Value.Year.ToString().Length - 1) + "0");

            lblDisplay.Text = Mode == DisplayMode.Week ? ExFun.ToMonth(Value, true) + " " + Value.Year :
                                Mode == DisplayMode.Month ? Value.Year.ToString() :
                                YearGrid.Min + "-" + (YearGrid.Min + 9);

            WeekGrid.WeekGrid.IsEnabled = Mode == DisplayMode.Week;
            MonthGrid.MonthGrid.IsEnabled = Mode == DisplayMode.Month;
            YearGrid.YearGrid.IsEnabled = Mode == DisplayMode.Year;
        }

        void SetDayGrid()
        {
            if (WeekGrid.WeekGrid != null && DayGrid.Count > 0)
                DayGrid.ForEach(x => x.ToList().ForEach(X => WeekGrid.WeekGrid.Children.Remove(X)));
            DayGrid.Clear();
            var M = new DateTime(Value.Year, Value.Month, 1);
            if (M.DayOfWeek == DayOfWeek.Sunday) M = M.AddDays(-7);
            M = M.AddDays((int)M.DayOfWeek * -1);

            for (int J = 1; J < 7; J++)
            {
                var W = new Day() { SelectCell = SelectDay };
                for (int I = 0; I < 7; I++)
                {
                    var T = M == DateTime.Today ? ExComponents.Day.Tag.Today : M.Month == Value.Month ? ExComponents.Day.Tag.This :
                        (M.Month > Value.Month && !(M.Month == 12 && Value.Month == 1)) || (Value.Month == 12 && M.Month == 1) ?
                        ExComponents.Day.Tag.Next : ExComponents.Day.Tag.Pre;
                    W.Add(M.Day, M.DayOfWeek, J, T);
                    M = M.AddDays(1);
                }
                DayGrid.Add(W);
            }

            if (WeekGrid.WeekGrid != null)
                DayGrid.ForEach(x => x.ToList().ForEach(X => WeekGrid.WeekGrid.Children.Add(X)));

            OnPropertyChanged(nameof(DayGrid));
        }

        private void SelectDay(object sender, MouseButtonEventArgs e)
        {
            var lblCell = (Label)sender;
            var I = int.Parse(lblCell.Content as string);
            var Tag = (Day.Tag)(lblCell.Tag ?? ExComponents.Day.Tag.This);
            var Ofset = 0;
            switch (Tag)
            {
                case ExComponents.Day.Tag.Pre: Ofset = -1; break;
                case ExComponents.Day.Tag.Next: Ofset = 1; break;
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
            var lblCell = (Label)sender;
            var I = MonthGrid.Find(lblCell.Content as string);
            if(I < 0) return;

            Value = new DateTime(Value.Year, I, 1);

            Mode = DisplayMode.Week;
            Update();
        }

        private void SelectYear(object sender, MouseButtonEventArgs e)
        {
            var lblCell = (Label)sender;
            var I = YearGrid.Find(lblCell.Content as string);
            if (I < 0) return;

            Value = new DateTime(I, Value.Month, 1);

            Mode = DisplayMode.Month;
            Update();
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var TextBox = (TextBox)parent;
                if (!TextBox.IsKeyboardFocusWithin)
                {
                    // If the Content box is not yet focussed, give it the focus and
                    // stop further processing of this click event.
                    TextBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private void SelectAllContent(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox TextBox)
                TextBox.SelectAll();
        }
    }

    internal class Day
    {
        public Label Sun { get; private set; } = new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Right, Padding = new Thickness(0), Margin = new Thickness(0) };
        public Label Mon { get; private set; } = new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Right, Padding = new Thickness(0), Margin = new Thickness(0) };
        public Label Tue { get; private set; } = new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Right, Padding = new Thickness(0), Margin = new Thickness(0) };
        public Label Wed { get; private set; } = new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Right, Padding = new Thickness(0), Margin = new Thickness(0) };
        public Label Thu { get; private set; } = new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Right, Padding = new Thickness(0), Margin = new Thickness(0) };
        public Label Fri { get; private set; } = new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Right, Padding = new Thickness(0), Margin = new Thickness(0) };
        public Label Sat { get; private set; } = new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Right, Padding = new Thickness(0), Margin = new Thickness(0) };

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
                    Sun.Content = Day.ToString();
                    Sun.Foreground = Colour;
                    Sun.Tag = Tag;
                    Sun.MouseLeftButtonDown += SelectCell;
                    break;
                case DayOfWeek.Monday:
                    Grid.SetColumn(Mon, 1);
                    Grid.SetRow(Mon, Row);
                    Mon.Content = Day.ToString();
                    Mon.Foreground = Colour;
                    Mon.Tag = Tag;
                    Mon.MouseLeftButtonDown += SelectCell;
                    break;
                case DayOfWeek.Tuesday:
                    Grid.SetColumn(Tue, 2);
                    Grid.SetRow(Tue, Row);
                    Tue.Content = Day.ToString();
                    Tue.Foreground = Colour;
                    Tue.Tag = Tag;
                    Tue.MouseLeftButtonDown += SelectCell;
                    break;
                case DayOfWeek.Wednesday:
                    Grid.SetColumn(Wed, 3);
                    Grid.SetRow(Wed, Row);
                    Wed.Content = Day.ToString();
                    Wed.Foreground = Colour;
                    Wed.Tag = Tag;
                    Wed.MouseLeftButtonDown += SelectCell;
                    break;
                case DayOfWeek.Thursday:
                    Grid.SetColumn(Thu, 4);
                    Grid.SetRow(Thu, Row);
                    Thu.Content = Day.ToString();
                    Thu.Foreground = Colour;
                    Thu.Tag = Tag;
                    Thu.MouseLeftButtonDown += SelectCell;
                    break;
                case DayOfWeek.Friday:
                    Grid.SetColumn(Fri, 5);
                    Grid.SetRow(Fri, Row);
                    Fri.Content = Day.ToString();
                    Fri.Foreground = Colour;
                    Fri.Tag = Tag;
                    Fri.MouseLeftButtonDown += SelectCell;
                    break;
                case DayOfWeek.Saturday:
                    Grid.SetColumn(Sat, 6);
                    Grid.SetRow(Sat, Row);
                    Sat.Content = Day.ToString();
                    Sat.Foreground = Colour;
                    Sat.Tag = Tag;
                    Sat.MouseLeftButtonDown += SelectCell;
                    break;
            }
        }

        public void Select(int Day, Tag Tag)
        {
            ToList().ForEach(x => { x.Background = Brushes.White; x.BorderBrush = Brushes.White; });
            var lbl = ToList().FirstOrDefault(x => x.Content as string == Day.ToString() && (Tag)(x.Tag ?? Tag.This) == Tag) ??
                new Label();
            lbl.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cce8ff"));
            lbl.BorderBrush = Brushes.CornflowerBlue;
        }

        public List<Label> ToList() => new List<Label>() { Sun, Mon, Tue, Wed, Thu, Fri, Sat };
    }

    internal class Week 
    {
        public Grid WeekGrid = new Grid() { Margin = new Thickness(5, 0, 10, 0) };

        public Week()
        {
            WeekGrid.Children.Clear();
            List<TextBlock> Headers = new List<TextBlock>() {
                new TextBlock() { Text = "Sun", TextAlignment = TextAlignment.Center },
                new TextBlock() { Text = "Mon", TextAlignment = TextAlignment.Center },
                new TextBlock() { Text = "Tue", TextAlignment = TextAlignment.Center },
                new TextBlock() { Text = "Wed", TextAlignment = TextAlignment.Center },
                new TextBlock() { Text = "Thu", TextAlignment = TextAlignment.Center },
                new TextBlock() { Text = "Fri", TextAlignment = TextAlignment.Center },
                new TextBlock() { Text = "Sat", TextAlignment = TextAlignment.Center },
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
    }

    internal class Month
    {
        public Grid MonthGrid = new Grid() { Margin = new Thickness(0, 15, 0, 0), IsEnabled = false };
        internal MouseButtonEventHandler SelectCell { set { Values.ForEach(x => x.MouseDown += value); } }

        List<Label> Values = new List<Label>() {
            new Label() { Content = "Jan", BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { Content = "Feb", BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { Content = "Mar", BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { Content = "Apr", BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { Content = "May", BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { Content = "Jun", BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { Content = "Jul", BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { Content = "Aug", BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { Content = "Sep", BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { Content = "Oct", BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { Content = "Nov", BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { Content = "Dec", BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
        };

        public Month()
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

        public int Find(string Content)
        {
            var edt = Values.FirstOrDefault(x => x.Content as string == Content);
            if (edt == null) return -1;
            return Values.IndexOf(edt) + 1;
        }

        public void Select(int Month)
        {
            Values.ForEach(x => x.Background = Brushes.White);
            var lbl = Values.FirstOrDefault(x => x.Content as string == ExFun.ToMonth(Month)) ??
                new Label();
            lbl.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cce8ff"));
            lbl.BorderBrush = Brushes.CornflowerBlue;
        }
    }

    internal class Year
    {
        public Grid YearGrid = new Grid() { Margin = new Thickness(0, 15, 0, 0), IsEnabled = false };
        internal MouseButtonEventHandler SelectCell { set { Values.ForEach(x => x.MouseDown += value); } }
        public int Min { 
            get { return int.Parse(Values[1].Content as string); }
            set
            {
                var L = value - 2;
                for (int I = 0; I < 12; I++)
                {
                    L++;
                    Values[I].Content = L.ToString();
                    if (L <= value - 1 || L >= value + 10) Values[I].Foreground = Brushes.Gray;
                }
            }
        }

        List<Label> Values = new List<Label>() {
            new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
            new Label() { BorderThickness = new Thickness(1), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), Margin = new Thickness(0) },
        };

        public Year()
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

        public int Find(string Content)
        {
            var edt = Values.FirstOrDefault(x => x.Content as string == Content);
            if (edt == null) return -1;
            return int.Parse(edt.Content as string);
        }

        public void Select(int Year)
        {
            Values.ForEach(x => x.Background = Brushes.White);
            var lbl = Values.FirstOrDefault(x => x.Content as string == Year.ToString()) ??
                new Label();
            lbl.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cce8ff"));
            lbl.BorderBrush = Brushes.CornflowerBlue;
        }
    }
}
