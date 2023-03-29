using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// Opens A Search Popup In Its Parent Control
    /// </summary>
    public class FindBox : Control, INotifyPropertyChanged
    {
        static FindBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FindBox), new FrameworkPropertyMetadata(typeof(FindBox)));
        }

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            var Popup = (Popup)Template.FindName("PART_Popup", this);
            var edtFind = (TextBox)Template.FindName("PART_Search", this);
            var btnSearch = (ImageButton)Template.FindName("PART_Accept", this);
            var btnExit = (ImageButton)Template.FindName("PART_Cancel", this);
            var cmbCategory = (ComboBox)Template.FindName("PART_Cat", this);

            Popup.SetBinding(Popup.IsOpenProperty, new Binding(nameof(IsOpen)) { Source = this, Mode = BindingMode.TwoWay });
            edtFind.SetBinding(TextBox.TextProperty, new Binding(nameof(Value)){ Source = this, Mode = BindingMode.TwoWay });
            cmbCategory.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(ItemsSource)) { Source = this });
            cmbCategory.SetBinding(ComboBox.SelectedItemProperty, new Binding(nameof(SelectedItem)) { Source = this, Mode = BindingMode.TwoWay });
            cmbCategory.SetBinding(ComboBox.SelectedIndexProperty, new Binding(nameof(SelectedIndex)) { Source = this, Mode = BindingMode.TwoWay });

            btnSearch.Source = Imaging.CreateBitmapSourceFromHBitmap(ExComponentsRes.Search.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));
            btnExit.Source = Imaging.CreateBitmapSourceFromHBitmap(ExComponentsRes.Cancel.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));

            edtFind.KeyDown += (sender, e) => { if (e.Key == Key.Enter) Accept(); };
            btnSearch.Click += Accept;
            btnExit.Click += Exit;

            void Accept(object sender = null, RoutedEventArgs e = null) => IsOpen = false;
            void Exit(object sender = null, RoutedEventArgs e = null)
            {
                Value = "";
                SelectedIndex = -1;
                IsOpen = false;
            }

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Gets or sets the value of the Number Edit.
        /// </summary>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(FindBox),
                new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the value of the Number Edit.
        /// </summary>
        public static DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(FindBox),
                new PropertyMetadata(false, IsOpenPropertyChanged));

        private static void IsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FindBox SearchBox)
            {
                SearchBox.OnPropertyChanged(nameof(IsOpen));

                if(SearchBox.IsOpen)
                    SearchBox.RaiseEvent(new RoutedEventArgs(OnOpenEvent, SearchBox));
                else
                    SearchBox.RaiseEvent(new RoutedEventArgs(OnCloseEvent, SearchBox));
            }
        }

        /// <summary>
        /// Gets or sets the target in which the popup attaches to.
        /// </summary>
        public static DependencyProperty TargetControlProperty =
            DependencyProperty.Register(nameof(TargetControl), typeof(UIElement), typeof(FindBox));

        /// <summary>
        /// Items Source Property
        /// </summary>
        public static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<object>), typeof(FindBox));

        /// <summary>
        /// Event For When The Popup Opens
        /// </summary>
        public static RoutedEvent OnOpenEvent = EventManager.RegisterRoutedEvent(nameof(OnOpen),
            RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(FindBox));

        /// <summary>
        /// Event When The Popup Opens
        /// </summary>
        public event SelectionChangedEventHandler OnOpen
        {
            add { AddHandler(OnOpenEvent, value); }
            remove { RemoveHandler(OnOpenEvent, value); }
        }

        /// <summary>
        /// Event For When The Popup Closes
        /// </summary>
        public static RoutedEvent OnCloseEvent = EventManager.RegisterRoutedEvent(nameof(OnClose),
            RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(FindBox));

        /// <summary>
        /// Event When The Popup Closes
        /// </summary>
        public event SelectionChangedEventHandler OnClose
        {
            add { AddHandler(OnCloseEvent, value); }
            remove { RemoveHandler(OnCloseEvent, value); }
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
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value ); OnPropertyChanged(nameof(Value)); }
        }

        /// <summary>
        /// Is This Component Open
        /// </summary>
        [Description("Gets or sets whether the Popup is open (visible) or closed."), Category("Appearance")]
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Gets or sets the target in which the popup attaches to.
        /// </summary>
        [Description("Gets or sets the target in which the popup attaches to"), Category("Common")]
        public UIElement TargetControl
        {
            get { return (UIElement)GetValue(TargetControlProperty); }
            set { SetValue(TargetControlProperty, value); OnPropertyChanged(nameof(TargetControl)); }
        }

        /// <summary>
        /// Gets an object representing the collection of the items contained in the items control
        /// </summary>
        [Description("Gets an object representing the collection of the items contained in the items control"), Category("Common")]
        public IEnumerable<object> Items { get { return ItemsSource; } }

        /// <summary>
        /// Gets or sets a collection that is used to generate the content of the control
        /// </summary>
        [Description("Gets or sets a collection that is used to generate the content of the control"), Category("Common")]
        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); OnPropertyChanged(nameof(ItemsSource)); OnPropertyChanged(nameof(Items)); }
        }

        /// <summary>
        /// Selected Item
        /// </summary>
        public object SelectedItem { get; set; }

        /// <summary>
        /// Selected Items Indexed
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Opens The Popup
        /// </summary>
        public void Show() => IsOpen = true;
        /// <summary>
        /// Closes The Popup
        /// </summary>
        public void Close() => IsOpen = false;
    }
}
