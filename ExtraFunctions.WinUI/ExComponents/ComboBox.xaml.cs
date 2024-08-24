using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// A somewhat better editable '<see cref="Microsoft.UI.Xaml.Controls.ComboBox"/>'.
    /// </summary>
    public sealed partial class ComboBox : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a editable <see cref="ComboBox"/>
        /// </summary>
        public ComboBox()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                lstDropDown.SetBinding(WidthProperty, new Binding() { Source = this, Path = new(nameof(ActualWidth)) });
                ChangeBackground();
            };

            lstDropDown = new ListView()
            {
                Margin = new(-15),
                IsItemClickEnabled = true,
                CornerRadius = new(5),
            };

            lstDropDown.SetBinding(MaxHeightProperty, new Binding() { Source = this, Path= new(nameof(MaxMenuHeight)) });
            lstDropDown.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = this, Path = new(nameof(ItemsSource)), Mode = BindingMode.OneWay });
            lstDropDown.ItemClick += SelectionClicked;

            DropDown = new()
            {
                Content = lstDropDown,
                ShouldConstrainToRootBounds = false,
                LightDismissOverlayMode = LightDismissOverlayMode.Off,
                Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom,
            };
        }

        private ListView lstDropDown;
        private Flyout DropDown;

        readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(ComboBox), new PropertyMetadata(null));

        readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ComboBox),
                new PropertyMetadata(new List<object>(), (s, e) => { 
                    if (s is ComboBox cmb) cmb.OnPropertyChanged(nameof(ItemsSource)); 
                }));

        readonly DependencyProperty MaxMenuHeightProperty =
            DependencyProperty.Register(nameof(MaxMenuHeight), typeof(double), typeof(ComboBox), new PropertyMetadata(double.MaxValue));

        readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(ComboBox), new PropertyMetadata(""));

        readonly DependencyProperty HideBackgroundProperty =
            DependencyProperty.Register(nameof(HideBackground), typeof(bool), typeof(ComboBox), new PropertyMetadata(false));

        private string Text { get => Value?.ToString() ?? ""; set {  } }
        /// <summary>
        /// The value entered or selected or mached.
        /// </summary>
        public object Value
        {
            get => GetValue(ValueProperty);
            set
            {
                var Val = Value;
                if (Val == value) { OnPropertyChanged(nameof(Text)); return; }
                SetValue(ValueProperty, value);

                OnPropertyChanged(nameof(Value)); 
                OnPropertyChanged(nameof(Text));
                OnSelectionChange(value, Val);
                if (lstDropDown.Items.Contains(value) && lstDropDown.SelectedItem != value) 
                    lstDropDown.SelectedItem = value;
                else lstDropDown.SelectedItem = null;
            }
        }

        /// <summary>
        /// The source of items to display as options.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set { SetValue(ItemsSourceProperty, value); OnPropertyChanged(nameof(ItemsSource)); }
        }

        /// <summary>
        /// Limit the dropdown menu height.
        /// </summary>
        public double MaxMenuHeight
        {
            get => (double)GetValue(MaxMenuHeightProperty);
            set { SetValue(MaxMenuHeightProperty, value); OnPropertyChanged(nameof(MaxMenuHeight)); }
        }

        /// <summary>
        /// The text to display above the component.
        /// </summary>
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set { SetValue(HeaderProperty, value); OnPropertyChanged(nameof(Header)); }
        }

        /// <summary>
        /// True to not display a background, otherwise false.
        /// </summary>
        public bool HideBackground
        {
            get => (bool)GetValue(HideBackgroundProperty);
            set { SetValue(HideBackgroundProperty, value); ChangeBackground(); OnPropertyChanged(nameof(HideBackground)); }
        }

        private bool? SkipChanged = false;

        private void ChangeBackground()
        {
            if(!HideBackground) return;
            edtValue.Background = null;
            edtValue.BorderBrush = null;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <inheritdoc/>
        public void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void SelectionClicked(object sender, ItemClickEventArgs e)
        {
            Value = e.ClickedItem;
            DropDown.Hide();// .IsOpen = false;
        }

        private void ShowMenu(object sender, RoutedEventArgs e)
        {
            edtValue.Focus(FocusState.Programmatic);
            DropDown.ShowAt(this);
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            var edt = sender as TextBox;
            if (string.IsNullOrEmpty(edt.Text)) Value = edt.Text;
            if (SkipChanged == null || string.IsNullOrEmpty(edt.Text)) { SkipChanged = false; return; }
            if (SkipChanged ?? true) { Value = edt.Text; SkipChanged = false; return; }
            var Item = ItemsSource.Cast<object>().Where(x => x.ToString().StartsWith(edt.Text, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefault();
            if (Item == null) { Value = edt.Text; return; }
            var index = edt.Text.Length;
            SkipChanged = null;
            Value = Item;
            edt.Select(index, Item.ToString().Length - index);
        }

        private void SkipTextChanged(object sender, KeyRoutedEventArgs e)
        {
            var KEYS = new[] { VirtualKey.Back, VirtualKey.Delete, VirtualKey.Clear };
            if (KEYS.Contains(e.Key)) SkipChanged = true;

            if (e.Key == VirtualKey.Up && lstDropDown.SelectedIndex > 0)
            {
                lstDropDown.SelectedIndex--;
                edtValue.Select(edtValue.Text.Length, 0);
            }
            if (e.Key == VirtualKey.Down && lstDropDown.SelectedIndex < ItemsSource.Cast<object>().Count() - 1)
            {
                lstDropDown.SelectedIndex++;
                edtValue.Select(edtValue.Text.Length, 0);
            }
        }

        private void SelectFocus(UIElement sender, GettingFocusEventArgs args)
        {
            if(args.InputDevice != FocusInputDeviceKind.Mouse) edtValue.SelectAll();
        }

        //NOTE: Update To Value Changed.
        //NOTE: Add Selection Change That Only Trigers When An Item Is Selected.
        /// <summary>
        /// Occurs when the value property changes.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChange;

        private void OnSelectionChange(object NewValue, object OldValue) => 
            SelectionChange?.Invoke(this, new(new List<object> { OldValue }, new List<object> { NewValue }));
    }
}
